﻿using SoftwareChallengeClient;
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
        static string Host = "127.0.0.1";
        static int Port = 13050;
        static string Reservation = "";
        static string Strategy = "";

        static string RoomID = "";
        static Logic PlayerLogic;
        static State GameState;

        public static string RoomIDread
        {
            get
            {
                return RoomID;
            }
        }

        static void Main(string[] args)
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
                        return;

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

            PlayerLogic = new Logic();
            GameState = new State();
            PlayerLogic.GameState = GameState;

            TcpClient client = new TcpClient(Host, Port);
            NetworkStream stream = client.GetStream();

            ConsoleWriteLine("Connected to the game server!", ConsoleColor.Green);

            if (string.IsNullOrWhiteSpace(Reservation))
                Send(stream, $"<protocol><join gameType=\"swc_2019_piranhas\" />");
            else
                Send(stream, $"<protocol><joinPrepared reservationCode=\"{Reservation}\" />");

            try
            {
                bool running = true;
                while (running)
                {
                    string[] rawAnswers = Recieve(stream).
                        Replace("</room>\n  <room", "༄༅༆").
                        Split('༅').
                        Select(x => x = x.Replace("༄", "</room>\n").Replace("༆", "<room").Trim(' ')).
                        Select(x => x.StartsWith("<protocol>") ? x.Remove(0, "<protocol>".Length) : x).
                        ToArray();

                    foreach (string rawAnswer in rawAnswers)
                    {
                        if (string.IsNullOrWhiteSpace(rawAnswer))
                            continue;
                        if (rawAnswer.Contains("</protocol>"))
                        {
                            running = false;
                            break;
                        }

                        try
                        {
                            XElement xmlAnswer = XElement.Parse(rawAnswer);

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
                                                        Enum.TryParse(first.Attribute(XName.Get("color")).Value, out PlayerLogic.MyColor);
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

                                                                XElement displayNameRed = state.Nodes().FirstOrDefault(x => x is XElement && (x as XElement).Name.LocalName == "red") as XElement;
                                                                XElement displayNameBlue = state.Nodes().FirstOrDefault(x => x is XElement && (x as XElement).Name.LocalName == "blue") as XElement;
                                                                if (displayNameRed != null)
                                                                    GameState.RedDisplayName = displayNameRed.Value;
                                                                if (displayNameBlue != null)
                                                                    GameState.BlueDisplayName = displayNameBlue.Value;

                                                                XElement board = state.Nodes().FirstOrDefault(x => x is XElement && (x as XElement).Name.LocalName == "board") as XElement;
                                                                string[] fields = board.ToString().Split('\n').Select(x => x.Trim(' ').Trim('\t')).Where(x => x.StartsWith("<field ")).ToArray();
                                                                foreach (string field in fields)
                                                                {
                                                                    XElement fieldElement = XElement.Parse(field);
                                                                    int x = Convert.ToInt32(fieldElement.Attribute(XName.Get("x")).Value),
                                                                        y = Convert.ToInt32(fieldElement.Attribute(XName.Get("y")).Value);
                                                                    FieldState newState;
                                                                    Enum.TryParse(fieldElement.Attribute(XName.Get("state")).Value, out newState);
                                                                    GameState.BoardState.Fields[x, y].Update(x, y, newState);
                                                                }
                                                            }
                                                        }
                                                        break;

                                                    case "sc.framework.plugins.protocol.MoveRequest":
                                                        Send(stream, PlayerLogic.GetMove().toXML());
                                                        break;
                                                }
                                                break;
                                        }
                                    }
                                    break;
                            }
                        } catch (Exception e) {
                            ConsoleWriteLine("Parse-Error:\n" + e, ConsoleColor.Red);
                        }
                    }
                }
            } catch (Exception e) {
                ConsoleWriteLine("Error:\n" + e, ConsoleColor.Red);
            }

            stream.Close();
            client.Close();
            ConsoleWriteLine("End of Communication!", ConsoleColor.Red);
            Console.ReadKey();
            ConsoleWriteLine("Terminating the client!", ConsoleColor.Red);
        }

        static void Send(NetworkStream stream, string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            ConsoleWriteLine($"Sent: {message}", ConsoleColor.Yellow);
        }
        static string Recieve(NetworkStream stream)
        {
            int bufferlength = 256;
            string responseData = "";
            int recievedBytes = bufferlength;

            while (recievedBytes == bufferlength)
            {
                byte[] data = new byte[bufferlength];
                recievedBytes = stream.Read(data, 0, data.Length);
                responseData += Encoding.ASCII.GetString(data, 0, recievedBytes);

                Thread.Sleep(3); // Without this it stops reading at a certain point because it reads too fast
            }

            if (!string.IsNullOrWhiteSpace(responseData))
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
