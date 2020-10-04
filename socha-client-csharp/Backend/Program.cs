using SoftwareChallengeClient;
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
            Console.Read();
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
            SetMove LastMove = null;

            if (string.IsNullOrWhiteSpace(Reservation))
                Send(stream, $"<protocol><join gameType=\"swc_2021_blokus\" />");
            else
                Send(stream, $"<protocol><joinPrepared reservationCode=\"{Reservation}\" />");
            
            while (true)
            {
                string recieved = Recieve(stream);
                if (recieved.Contains("</protocol>") || recieved.Contains("left room"))
                    break;
                recieved = recieved.StartsWith("<protocol>") ? recieved.Remove(0, "<protocol>".Length) : recieved;
                recieved = "<received>" + recieved + "</received>";

                var serializer = new XmlSerializer(typeof(Received));
                StringReader stringReader = new StringReader(recieved);
                var recievedObjs = (Received)serializer.Deserialize(stringReader);

                if (recievedObjs.Joined != null)
                    RoomID = recievedObjs.Joined.RoomId;
                if (recievedObjs.Room != null)
                    foreach (var r in recievedObjs.Room)
                        if (r.Data != null)
                        {
                            if (r.Data.Class == "sc.framework.plugins.protocol.MoveRequest")
                            {
                                LastMove = PlayerLogic.GetMove();
                                Send(stream, LastMove.ToXML());
                            }
                            else if (r.Data.Class == "memento")
                            {
                                var inState = r.Data.State;

                                // Attr Parse
                                GameState.CurrentColorIndex = (PieceColor)Enum.Parse(typeof(PieceColor), inState.CurrentColorIndex);
                                GameState.Turn = inState.Turn;
                                GameState.Round = inState.Round;
                                GameState.StartPiece = (PieceKind)Enum.Parse(typeof(PieceKind), inState.StartPiece);

                                // Board Parse
                                GameState.CurrentBoard = new Board();
                                foreach (var f in inState.Board.Field)
                                    GameState.CurrentBoard.Fields[f.X, f.Y] = (PieceColor)Enum.Parse(typeof(PieceColor), f.Content);

                                // Piece Inventories Parse
                                GameState.BlueShapes = inState.BlueShapes.Shape.Select(x => (PieceKind)Enum.Parse(typeof(PieceKind), x)).ToList();
                                GameState.YellowShapes = inState.YellowShapes.Shape.Select(x => (PieceKind)Enum.Parse(typeof(PieceKind), x)).ToList();
                                GameState.RedShapes = inState.RedShapes.Shape.Select(x => (PieceKind)Enum.Parse(typeof(PieceKind), x)).ToList();
                                GameState.GreenShapes = inState.GreenShapes.Shape.Select(x => (PieceKind)Enum.Parse(typeof(PieceKind), x)).ToList();

                                if (inState.LastMove.Class == "sc.plugin2021.SkipMove")
                                    GameState.LastMove = new SkipMove();
                                else if (inState.LastMove.Class == "sc.plugin2021.SetMove")
                                    GameState.LastMove = new SetMove(
                                        (PieceColor)Enum.Parse(typeof(PieceColor), inState.LastMove.Piece.Color),
                                        (PieceKind)Enum.Parse(typeof(PieceKind), inState.LastMove.Piece.Kind),
                                        (Rotation)Enum.Parse(typeof(Rotation), inState.LastMove.Piece.Rotation),
                                        inState.LastMove.Piece.IsFlipped,
                                        inState.LastMove.Piece.Position.X,
                                        inState.LastMove.Piece.Position.Y);

                                GameState.FirstPlayerName = inState.First.DisplayName;
                                GameState.SecondPlayerName = inState.Second.DisplayName;

                                UpdateConsoleTitle();
                            }
                            else if (r.Data.Class == "welcomeMessage")
                            {
                                if (!Enum.TryParse(r.Data.Color, out PlayerLogic.MyTeam))
                                    throw new XmlException("Couldn't parse player team!");
                            }
                        }
            }

            if (LastMove != null && LastMove.DebugHints.Count > 0)
                ConsoleWriteLine("Debug Hints from the Last Move:\n" + 
                    LastMove.DebugHints.Aggregate((x, y) => x + "\n" + y), ConsoleColor.Magenta);
        }
        static void UpdateConsoleTitle()
        {
            try { Console.Title = PlayerLogic.MyColor.ToString() + " in " + GameState.RedDisplayName + " vs " + GameState.BlueDisplayName; } catch { }
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
