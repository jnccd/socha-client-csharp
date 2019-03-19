using SoftwareChallengeClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SoftwareChallengeClient
{
    public class Program
    {
        // Start Arguments
        static string Host = "127.0.0.1";
        static int Port = 13050;
        static string Reservation = "";
        public static string Strategy { get; private set; } = "";
        
        public static string RoomID { get; private set; } = "";
        public static bool LogNetwork;
        static Logic PlayerLogic;
        static State GameState;
        
        static void Main(string[] args)
        {
            if (!GotProperStartArguments(args)) return;

            GameState = new State();
            PlayerLogic = new Logic { GameState = GameState };

            TcpClient client = ConnectToServer();
            if (client == null) return;
            NetworkStream stream = client.GetStream();
            ConsoleWriteLine("Connected to the game server!", ConsoleColor.Green);
            
            ExecuteCommunationLoop(stream);

            stream.Close();
            client.Close();
            ConsoleWriteLine("End of Communication!", ConsoleColor.Red);
            Console.Read();
            ConsoleWriteLine("Terminating the client!", ConsoleColor.Red);
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
                return null;
            }
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
                ConsoleWriteLine("The arguments Mason, what do they mean!?\n\n" + e, ConsoleColor.Red);
                Console.Read();
                return false;
            }

            return true;
        }
        static void ExecuteCommunationLoop(NetworkStream stream)
        {
            Move LastMove = null;

            if (string.IsNullOrWhiteSpace(Reservation))
                Send(stream, $"<protocol><join gameType=\"swc_2019_piranhas\" />");
            else
                Send(stream, $"<protocol><joinPrepared reservationCode=\"{Reservation}\" />");
            
            while (true)
            {
                string raw = Recieve(stream);
                if (raw.Contains("</protocol>") || raw.Contains("left room"))
                    break;
                raw = raw.StartsWith("<protocol>") ? raw.Remove(0, "<protocol>".Length) : raw;
                XElement recievedElement = XElement.Parse("<received>" + raw + "</received>");
                string[] nodes = recievedElement.Nodes().Where(x => x is XElement).Select(x => x.ToString()).ToArray();

                foreach (string node in nodes)
                {
                    if (string.IsNullOrWhiteSpace(node))
                        continue;

                    try
                    {
                        XElement xmlAnswer = XElement.Parse(node);

                        switch (xmlAnswer.Name.LocalName)
                        {
                            case "joined":
                                RoomID = xmlAnswer.FirstAttribute.Value;
                                break;

                            case "room":
                                if (xmlAnswer.HasElements && xmlAnswer.HasAttributes && xmlAnswer.FirstAttribute.Value == RoomID)
                                {
                                    XElement first = xmlAnswer.Elements().First();
                                    switch (first.Name.LocalName)
                                    {
                                        case "data":
                                            XAttribute classAttr = first.Attribute(XName.Get("class"));
                                            switch (classAttr.Value)
                                            {
                                                case "welcomeMessage":
                                                    Enum.TryParse(first.Attribute(XName.Get("color")).Value.ToUpper(), out PlayerLogic.MyColor);
                                                    UpdateConsoleTitle();
                                                    break;

                                                case "memento":
                                                    if (first.FirstNode is XElement)
                                                    {
                                                        XElement state = first.FirstNode as XElement;
                                                        if (state.Name.LocalName == "state")
                                                        {
                                                            Enum.TryParse(state.Attribute(XName.Get("startPlayerColor")).Value, out GameState.StartPlayerColor);
                                                            Enum.TryParse(state.Attribute(XName.Get("currentPlayerColor")).Value, out GameState.CurrentPlayerColor);

                                                            GameState.Turn = Convert.ToInt32(state.Attribute(XName.Get("turn")).Value);

                                                            if (state.Nodes().FirstOrDefault(x => x is XElement && (x as XElement).Name.LocalName == "red") is XElement displayNameRed)
                                                                GameState.RedDisplayName = displayNameRed.Attribute(XName.Get("displayName")).Value;
                                                            if (state.Nodes().FirstOrDefault(x => x is XElement && (x as XElement).Name.LocalName == "blue") is XElement displayNameBlue)
                                                                GameState.BlueDisplayName = displayNameBlue.Attribute(XName.Get("displayName")).Value;
                                                            UpdateConsoleTitle();

                                                            XElement board = state.Nodes().FirstOrDefault(x => x is XElement && (x as XElement).Name.LocalName == "board") as XElement;
                                                            string[] fields = board.ToString().Split('\n').Select(x => x.Trim(' ').Trim('\t')).Where(x => x.StartsWith("<field ")).ToArray();
                                                            foreach (string field in fields)
                                                            {
                                                                XElement fieldElement = XElement.Parse(field);
                                                                int x = Convert.ToInt32(fieldElement.Attribute(XName.Get("x")).Value),
                                                                    y = Convert.ToInt32(fieldElement.Attribute(XName.Get("y")).Value);
                                                                Enum.TryParse(fieldElement.Attribute(XName.Get("state")).Value, out FieldState newState);
                                                                GameState.CurrentBoard.Fields[x, y].Update(x, y, newState);
                                                            }
                                                        }
                                                    }
                                                    break;

                                                case "sc.framework.plugins.protocol.MoveRequest":
                                                    LastMove = PlayerLogic.GetMove();
                                                    Send(stream, LastMove.ToXML());
                                                    break;
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        ConsoleWriteLine("Parse-Error:\n" + e, ConsoleColor.Red);
                    }
                }
            }

            if (LastMove != null && LastMove.DebugHints.Count > 0)
                ConsoleWriteLine("Debug Hints from the Last Move:\n" + LastMove.DebugHints.Aggregate((x, y) => x + "\n" + y), ConsoleColor.Magenta);
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
                Thread.Sleep(3); // Without this it stops reading at a certain point because it reads too fast

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
