﻿using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SochaClient.Backend.XML;

namespace SochaClient.Backend
{
    public static class Starter
    {
        // Start Arguments
        static string host = "127.0.0.1";
        static int port = 13050;
        static string reservation = "";
        public static string Strategy { get; private set; } = "";

#if DEBUG
        public static bool LogNetwork = true;
        public static bool DrawBoard = true;
        static readonly bool logToFile = true;
#else
        public static bool LogNetwork = false;
        public static bool DrawBoard = false;
        static readonly bool logToFile = false;
#endif
        static string instanceIdentifier;
        static string LogFileName { get => $"log{instanceIdentifier}.txt"; }
        static StreamWriter logWriter;

        public static string RoomID { get; private set; } = "";
        static Logic playerLogic;
        static State gameState;

        public static void Main(string[] args, Logic playerLogic)
        {
            if (!GotProperStartArguments(args)) return;

            // Setup object hooks
            Starter.playerLogic = playerLogic;
            playerLogic.GameState = new State();
            gameState = playerLogic.GameState;

            // Setup logging
            instanceIdentifier = DateTime.Now.ToBinary().ToString();
            if (logToFile)
                logWriter = File.CreateText(LogFileName);

            // Setup TCP communication tunnel to server
            TcpClient client = ConnectToServer();
            NetworkStream stream = client.GetStream();
            ConsoleWriteLine("Connected to the game server!", ConsoleColor.Green);

            ExecuteCommunationLoop(stream);

            // Cleanup
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
                Send(stream, $"<protocol><join gameType=\"swc_2024_mississippi_queen\" />");
            else
                Send(stream, $"<protocol><joinPrepared reservationCode=\"{reservation}\" />");

            while (true)
            {
                // Recieve text over netowrk and preprocess it
                string recieved = Recieve(stream);
                recieved = recieved.StartsWith("<protocol>") ? recieved.Remove(0, "<protocol>".Length) : recieved;
                recieved = "<received>" + recieved + "</received>";
                recieved = recieved.Replace("\"one\"", "\"ONE\"").Replace("\"two\"", "\"TWO\"");

                if (recieved.Contains("</protocol>"))
                    break;

                // Serialize preprocessed text
                var serializer = new XmlSerializer(typeof(Received));
                var stringReader = new StringReader(recieved);
                var recievedObjs = (Received)serializer.Deserialize(stringReader);

                // Handle XML object
                if (recievedObjs.Joined != null)
                    RoomID = recievedObjs.Joined.RoomId;
                if (recievedObjs.Left != null && recievedObjs.Left.RoomId == RoomID)
                    break;
                if (recievedObjs.Room != null)
                    foreach (var r in recievedObjs.Room)
                        if (r.Data != null && r.RoomId == RoomID)
                            if (r.Data.Class == "moveRequest")
                            {
                                Debug.WriteLine("Got MoveReq");

                                if (gameState.Turn == 0)
                                    gameState.MyselfPlayer = gameState.GetPlayer(gameState.StartTeam);
                                else if (gameState.Turn == 1)
                                    gameState.MyselfPlayer = gameState.GetOtherPlayer(gameState.GetPlayer(gameState.StartTeam));

                                gameState.CurrentPlayer = gameState.MyselfPlayer;

                                Send(stream, (LastMove = playerLogic.GetMove()).ToXML());
                            }
                            else if (r.Data.Class == "memento")
                            {
                                var inState = r.Data.State;

                                // Attr
                                gameState.Turn = inState.Turn;
                                gameState.StartTeam = inState.StartTeam;
                                gameState.CurrentPlayer = gameState.GetPlayer(inState.CurrentTeam);

                                // Segments
                                foreach (Segment s in inState.Board.Segment)
                                {
                                    var centerCoords = new CubeCoords(s.Center.Q, s.Center.R, s.Center.S);
                                    for (int x = 0; x < s.Fieldarray.Count; x++)
                                        for (int y = 0; y < s.Fieldarray[x].ChildNodes.Count; y++)
                                        {
                                            var curNode = s.Fieldarray[x].ChildNodes[y];
                                            if (!Enum.TryParse(curNode.Name, out FieldType ft))
                                                throw new Exception("Cant parse this field type " + curNode.Name);
                                            Direction? dirOut = null;
                                            int? passengers = null;
                                            if (ft == FieldType.passenger)
                                            {
                                                if (!Enum.TryParse(curNode.Attributes["direction"].Value, out Direction dir))
                                                    throw new Exception($"Cant parse this field types direction {curNode.Name}, {curNode.Attributes["direction"]?.Value}");
                                                dirOut = dir;
                                                passengers = Convert.ToInt32(curNode.Attributes["passenger"].Value);
                                            }
                                            var fieldCoords = centerCoords + Cache.segOffsets[y][x].RotateByDir(s.Direction);
                                            gameState.Board.SetField(fieldCoords, new Field(ft, y == 2, fieldCoords, gameState.Board, dirOut, passengers));
                                        }
                                }

                                // Ships
                                for (int i = 0; i < inState.Ship.Count; i++)
                                {
                                    var parsedShip = new Ship(
                                        inState.Ship[i].Coal, 
                                        inState.Ship[i].Direction, 
                                        inState.Ship[i].Passengers, 
                                        inState.Ship[i].Points, 
                                        inState.Ship[i].Speed, 
                                        new CubeCoords(inState.Ship[i].Position.Q, inState.Ship[i].Position.R, inState.Ship[i].Position.S)
                                        );

                                    if (i == 0)
                                        gameState.PlayerOne.Ship = parsedShip;
                                    else if (i == 1)
                                        gameState.PlayerTwo.Ship = parsedShip;
                                    else
                                        throw new Exception("Theres too many of them, what are we going to do?");
                                }

                                gameState.Board.Print(false, gameState.PlayerOne, gameState.PlayerTwo);

                                UpdateConsoleTitle();
                            }
                            else if (r.Data.Class == "welcomeMessage")
                                playerLogic.MyTeam = r.Data.Color;

                //if (LastMove != null && LastMove is SetMove && (LastMove as SetMove).DebugHints.Count > 0)
                //    ConsoleWriteLine("Debug Hints from the Last Move:\n" +
                //        (LastMove as SetMove).DebugHints.Aggregate((x, y) => x + "\n" + y), ConsoleColor.Magenta);
            }
        }
        static void UpdateConsoleTitle()
        {
            try { Console.Title = $"{playerLogic.MyTeam} in ONE vs TWO"; }
            catch { }
        }

        // Basic NetworkStream management
        static void Send(NetworkStream stream, string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            if (LogNetwork)
                ConsoleWriteLine($"Sent: {message}", ConsoleColor.Cyan);
        }
        static string Recieve(NetworkStream stream)
        {
            const int bufferlength = 256;
            string responseData = "";

            while (!responseData.EndsWith("</room>") && !responseData.EndsWith("</protocol>"))
            {
                byte[] data = new byte[bufferlength];
                int recievedBytes = stream.Read(data, 0, data.Length);
                responseData += Encoding.ASCII.GetString(data, 0, recievedBytes);
            }

            if (!string.IsNullOrWhiteSpace(responseData) && LogNetwork)
                ConsoleWriteLine($"Received: {responseData}", ConsoleColor.Yellow);
            return responseData;
        }

        // Wrapper for color management
        public static void ConsoleWriteLine(string text, ConsoleColor Color)
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