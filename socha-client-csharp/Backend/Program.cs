using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Xml;

namespace socha_client_csharp
{
    public class Program
    {
        // Start Arguments
        static string Host = "127.0.0.1";
        static int Port = 13050;
        static string Reservation = "";
        public static string Strategy { get; private set; } = "";

        public static string RoomID { get; private set; } = "";
        public static bool LogNetwork = true;
        static Logic PlayerLogic;
        static State GameState;
        
        static void Main(string[] args)
        {
            if (!GotProperStartArguments(args)) return;

            GameState = new State();
            PlayerLogic = new Logic { GameState = GameState };

            TcpClient client = ConnectToServer();
            NetworkStream stream = client.GetStream();
            ConsoleWriteLine("Connected to the game server!", ConsoleColor.Green);
            
            ExecuteCommunationLoop(stream);

            stream.Close();
            client.Close();
            ConsoleWriteLine("End of communication!", ConsoleColor.Red);
            Console.ReadLine();
            ConsoleWriteLine("Terminating the client!", ConsoleColor.Red);
        }
        static bool GotProperStartArguments(string[] args)
        {
            try
            {
                for (int i = 0; i < args.Length; i++)
                    switch (args[i])
                    {
                        case "--help":
                            ConsoleWriteLine($@"
        Usage: start.sh [options]
          -h, --host:
              The IP address of the host to connect to (default: {Host}).
          -p, --port:
              The port used for the connection (default: {Port}).
          -r, --reservation:
              The reservation code to join a prepared game.
          -s, --strategy:
              The strategy used for the game.
          --help:
              Print this help message.", ConsoleColor.Cyan);
                            return false;

                        case "-h":
                        case "--host":
                            if (args.Length > i + 1)
                                Host = args[i + 1];
                            break;

                        case "-p":
                        case "--port":
                            if (args.Length > i + 1)
                                Port = Convert.ToInt32(args[i + 1]);
                            break;

                        case "-r":
                        case "--reservation":
                            if (args.Length > i + 1)
                                Reservation = args[i + 1];
                            break;

                        case "-s":
                        case "--strategy":
                            if (args.Length > i + 1)
                                Strategy = args[i + 1];
                            break;
                    }
            }
            catch (Exception e)
            {
                ConsoleWriteLine("The arguments Mason, what do they mean!?\n" +
                                 "Tip: Try 'start --help' if you need help with the argument syntax\n\n" + e, ConsoleColor.Red);
                Console.Read();
                return false;
            }

            return true;
        }
        static TcpClient ConnectToServer()
        {
            try
            {
                return new TcpClient(Host, Port);
            }
            catch (Exception e)
            {
                ConsoleWriteLine("Couldn't connect to the game server!\n\n" + e, ConsoleColor.Red);
                Console.Read();
                Environment.Exit(1);
                return null;
            }
        }
        static void ExecuteCommunationLoop(NetworkStream stream)
        {
            Move LastMove = null;

            if (string.IsNullOrWhiteSpace(Reservation))
                Send(stream, $"<protocol><join gameType=\"swc_2021_blokus\" />");
            else
                Send(stream, $"<protocol><joinPrepared reservationCode=\"{Reservation}\" />");
            
            while (true)
            {
                string recieved = Recieve(stream);
                recieved = recieved.StartsWith("<protocol>") ? recieved.Remove(0, "<protocol>".Length) : recieved;
                recieved = "<received>" + recieved + "</received>";
                recieved = recieved.Replace("\"one\"", "\"ONE\"").Replace("\"two\"", "\"TWO\"");

                if (recieved.Contains("</protocol>"))
                    break;

                var serializer = new XmlSerializer(typeof(Received));
                StringReader stringReader = new StringReader(recieved);
                var recievedObjs = (Received)serializer.Deserialize(stringReader);

                if (recievedObjs.Joined != null)
                    RoomID = recievedObjs.Joined.RoomId;
                if (recievedObjs.Left != null && recievedObjs.Left.RoomId == RoomID)
                    break;
                if (recievedObjs.Room != null)
                    foreach (var r in recievedObjs.Room)
                        if (r.Data != null && r.RoomId == RoomID)
                            if (r.Data.Class == "sc.framework.plugins.protocol.MoveRequest")
                                Send(stream, (LastMove = PlayerLogic.GetMove()).ToXML());
                            else if (r.Data.Class == "memento")
                            {
                                var inState = r.Data.State;

                                // Attr
                                GameState.Turn = inState.Turn;
                                GameState.Round = inState.Round;
                                GameState.StartPiece = inState.StartPiece;
                                GameState.StartTeam = inState.StartTeam.Text;
                                GameState.OrderedColors = inState.OrderedColors.Color;
                                GameState.CurrentColorIndex = inState.CurrentColorIndex;
                                GameState.CurrentColor = (PieceColor)((GameState.CurrentColorIndex % Enum.GetValues(typeof(PieceColor)).Length) + 1);

                                // Board
                                GameState.CurrentBoard = new Board();
                                foreach (var f in inState.Board.Field)
                                    GameState.CurrentBoard.GetField(f.X, f.Y).color = f.Content;

                                // Piece Inventories
                                GameState.BlueShapes = inState.BlueShapes.Shape;
                                GameState.YellowShapes = inState.YellowShapes.Shape;
                                GameState.RedShapes = inState.RedShapes.Shape;
                                GameState.GreenShapes = inState.GreenShapes.Shape;

                                // Last moves
                                if (inState.LastMove?.Class == "sc.plugin2021.SkipMove")
                                    GameState.LastMove = new SkipMove();
                                else if (inState.LastMove?.Class == "sc.plugin2021.SetMove")
                                    GameState.LastMove = new SetMove(
                                        inState.LastMove.Piece.Color,
                                        inState.LastMove.Piece.Kind,
                                        inState.LastMove.Piece.Rotation,
                                        inState.LastMove.Piece.IsFlipped,
                                        inState.LastMove.Piece.Position.X,
                                        inState.LastMove.Piece.Position.Y);

                                // Names
                                GameState.FirstPlayerName = inState.First.DisplayName;
                                GameState.SecondPlayerName = inState.Second.DisplayName;
                                UpdateConsoleTitle();
                            }
                            else if (r.Data.Class == "welcomeMessage")
                                PlayerLogic.MyTeam = r.Data.Color;

                if (LastMove != null && LastMove is SetMove && (LastMove as SetMove).DebugHints.Count > 0)
                    ConsoleWriteLine("Debug Hints from the Last Move:\n" +
                        (LastMove as SetMove).DebugHints.Aggregate((x, y) => x + "\n" + y), ConsoleColor.Magenta);
            }
        }
        static void UpdateConsoleTitle()
        {
            try { Console.Title = PlayerLogic.MyTeam.ToString() + " in " + GameState.FirstPlayerName + " vs " + GameState.SecondPlayerName; } catch { }
        }

        static void Send(NetworkStream stream, string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            if (LogNetwork)
                ConsoleWriteLine($"Sent: {message}", ConsoleColor.Cyan);
        }
        static string Recieve(NetworkStream stream)
        {
            int bufferlength = 256;
            string responseData = "";
            int recievedBytes = bufferlength;

            while (recievedBytes == bufferlength)
            {
                Thread.Sleep(3); // Without this it stops reading at a certain point

                byte[] data = new byte[bufferlength];
                recievedBytes = stream.Read(data, 0, data.Length);
                responseData += Encoding.ASCII.GetString(data, 0, recievedBytes);
            }

            if (!string.IsNullOrWhiteSpace(responseData) && LogNetwork)
                ConsoleWriteLine($"Received: {responseData}", ConsoleColor.Yellow);
            return responseData;
        }

        public static void ConsoleWriteLine(string text, ConsoleColor Color)
        {
            lock (Console.Title)
            {
                ConsoleColor defaultC = Console.ForegroundColor;
                Console.ForegroundColor = Color;
                Console.WriteLine(text);
                Console.ForegroundColor = defaultC;
            }
        }
    }
}
