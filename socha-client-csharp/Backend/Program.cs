using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Xml;

namespace SochaClient
{
    public class Program
    {
        // Start Arguments
        static string host = "127.0.0.1";
        static int port = 13050;
        static string reservation = "";
        public static string Strategy { get; private set; } = "";

        public static bool LogNetwork = true;
        public static bool DrawBoard = true;
        static readonly bool logToFile = true;
        static string instanceIdentifier;
        static string LogFileName { get => $"log{instanceIdentifier}.txt"; }
        static StreamWriter logWriter;

        public static string RoomID { get; private set; } = "";
        static Logic playerLogic;
        static State gameState;
        
        static void Main(string[] args)
        {
            if (!GotProperStartArguments(args)) return;

            gameState = new State();
            playerLogic = new Logic { GameState = gameState };
            instanceIdentifier = DateTime.Now.ToBinary().ToString();
            if (logToFile)
                logWriter = File.CreateText(LogFileName);

            TcpClient client = ConnectToServer();
            NetworkStream stream = client.GetStream();
            ConsoleWriteLine("Connected to the game server!", ConsoleColor.Green);
            
            ExecuteCommunationLoop(stream);

            stream.Close();
            client.Close();
            ConsoleWriteLine("End of communication!", ConsoleColor.Red);
            Console.ReadLine();
            ConsoleWriteLine("Terminating the client!", ConsoleColor.Red);

            if (logToFile)
                logWriter.Close();
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
        The IP address of the host to connect to (default: {host}).
    -p, --port:
        The port used for the connection (default: {port}).
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
                                host = args[i + 1];
                            break;

                        case "-p":
                        case "--port":
                            if (args.Length > i + 1)
                                port = Convert.ToInt32(args[i + 1]);
                            break;

                        case "-r":
                        case "--reservation":
                            if (args.Length > i + 1)
                                reservation = args[i + 1];
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
                return new TcpClient(host, port);
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

            if (string.IsNullOrWhiteSpace(reservation))
                Send(stream, $"<protocol><join gameType=\"swc_2021_blokus\" />");
            else
                Send(stream, $"<protocol><joinPrepared reservationCode=\"{reservation}\" />");
            
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
                                Send(stream, (LastMove = playerLogic.GetMove()).ToXML());
                            else if (r.Data.Class == "memento")
                            {
                                var inState = r.Data.State;

                                // Attr
                                gameState.Turn = inState.Turn;
                                gameState.Round = inState.Round;
                                gameState.StartPiece = inState.StartPiece;
                                gameState.StartTeam = inState.StartTeam.Text;
                                gameState.OrderedColors = inState.OrderedColors.Color;
                                gameState.CurrentColorIndex = inState.CurrentColorIndex;
                                gameState.CurrentColor = (PieceColor)((gameState.CurrentColorIndex % Enum.GetValues(typeof(PieceColor)).Length) + 1);

                                // Board
                                gameState.CurrentBoard = new Board();
                                foreach (var f in inState.Board.Field)
                                    gameState.CurrentBoard.GetField(f.X, f.Y).color = f.Content;

                                // Piece Inventories
                                gameState.BlueShapes = inState.BlueShapes.Shape;
                                gameState.YellowShapes = inState.YellowShapes.Shape;
                                gameState.RedShapes = inState.RedShapes.Shape;
                                gameState.GreenShapes = inState.GreenShapes.Shape;

                                // Last moves
                                if (inState.LastMove?.Class == "sc.plugin2021.SkipMove")
                                    gameState.LastMove = new SkipMove();
                                else if (inState.LastMove?.Class == "sc.plugin2021.SetMove")
                                    gameState.LastMove = new SetMove(
                                        inState.LastMove.Piece.Color,
                                        inState.LastMove.Piece.Kind,
                                        inState.LastMove.Piece.Rotation,
                                        inState.LastMove.Piece.IsFlipped,
                                        inState.LastMove.Piece.Position.X,
                                        inState.LastMove.Piece.Position.Y);

                                // Names
                                gameState.FirstPlayerName = inState.First.DisplayName;
                                gameState.SecondPlayerName = inState.Second.DisplayName;
                                UpdateConsoleTitle();

                                if (DrawBoard)
                                    DrawBoardPng();
                            }
                            else if (r.Data.Class == "welcomeMessage")
                                playerLogic.MyTeam = r.Data.Color;

                if (LastMove != null && LastMove is SetMove && (LastMove as SetMove).DebugHints.Count > 0)
                    ConsoleWriteLine("Debug Hints from the Last Move:\n" +
                        (LastMove as SetMove).DebugHints.Aggregate((x, y) => x + "\n" + y), ConsoleColor.Magenta);
            }
        }
        static void UpdateConsoleTitle()
        {
            try { Console.Title = playerLogic.MyTeam.ToString() + " in " + gameState.FirstPlayerName + " vs " + gameState.SecondPlayerName; } 
            catch { }
        }
        static void DrawBoardPng()
        {
            Bitmap b = new Bitmap(Board.Width, Board.Height);

            for (int x = 0; x < Board.Width; x++)
                for (int y = 0; y < Board.Height; y++)
                    b.SetPixel(x, y, gameState.CurrentBoard.GetField(x, y).Color.ToColor());

            try { b.Save("board.png"); } catch {}
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

                if (logToFile && logWriter != null)
                    logWriter.WriteLine(text);
            }
        }
    }
}
