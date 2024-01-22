using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

// Generated using: https://json2csharp.com/code-converters/xml-to-csharp

namespace SochaClient.Backend.XML
{
    [XmlRoot(ElementName = "center")]
    public class Center
    {

        [XmlAttribute(AttributeName = "q")]
        public int Q { get; set; }

        [XmlAttribute(AttributeName = "r")]
        public int R { get; set; }

        [XmlAttribute(AttributeName = "s")]
        public int S { get; set; }
    }

    [XmlRoot(ElementName = "segment")]
    public class Segment
    {

        [XmlElement(ElementName = "center")]
        public Center Center { get; set; }

        [XmlAnyElement("field-array")]
        public XmlElement Fieldarray { get; set; }

        [XmlAttribute(AttributeName = "direction")]
        public Direction Direction { get; set; }
    }

    [XmlRoot(ElementName = "board")]
    public class Board
    {

        [XmlElement(ElementName = "segment")]
        public List<Segment> Segment { get; set; }

        [XmlAttribute(AttributeName = "nextDirection")]
        public Direction NextDirection { get; set; }
    }

    [XmlRoot(ElementName = "position")]
    public class Position
    {

        [XmlAttribute(AttributeName = "q")]
        public int Q { get; set; }

        [XmlAttribute(AttributeName = "r")]
        public int R { get; set; }

        [XmlAttribute(AttributeName = "s")]
        public int S { get; set; }
    }

    [XmlRoot(ElementName = "ship")]
    public class Ship
    {

        [XmlElement(ElementName = "position")]
        public Position Position { get; set; }

        [XmlAttribute(AttributeName = "team")]
        public string Team { get; set; }

        [XmlAttribute(AttributeName = "direction")]
        public string Direction { get; set; }

        [XmlAttribute(AttributeName = "speed")]
        public int Speed { get; set; }

        [XmlAttribute(AttributeName = "coal")]
        public int Coal { get; set; }

        [XmlAttribute(AttributeName = "passengers")]
        public int Passengers { get; set; }

        [XmlAttribute(AttributeName = "freeTurns")]
        public int FreeTurns { get; set; }

        [XmlAttribute(AttributeName = "points")]
        public int Points { get; set; }
    }

    [XmlRoot(ElementName = "state")]
    public class State
    {

        [XmlElement(ElementName = "board")]
        public Board Board { get; set; }

        [XmlElement(ElementName = "ship")]
        public List<Ship> Ship { get; set; }

        [XmlAttribute(AttributeName = "class")]
        public string Class { get; set; }

        [XmlAttribute(AttributeName = "startTeam")]
        public PlayerTeam StartTeam { get; set; }

        [XmlAttribute(AttributeName = "turn")]
        public int Turn { get; set; }

        [XmlAttribute(AttributeName = "currentTeam")]
        public PlayerTeam CurrentTeam { get; set; }
    }

    [XmlRoot(ElementName = "data")]
    public class Data
    {

        [XmlElement(ElementName = "state")]
        public State State { get; set; }

        [XmlAttribute(AttributeName = "color")]
        public PlayerTeam Color { get; set; }

        [XmlAttribute(AttributeName = "class")]
        public string Class { get; set; }
    }

    [XmlRoot(ElementName = "room")]
    public class Room
    {

        [XmlElement(ElementName = "data")]
        public Data Data { get; set; }

        [XmlAttribute(AttributeName = "roomId")]
        public string RoomId { get; set; }
    }

    [XmlRoot(ElementName = "left")]
	public class Left
	{

		[XmlAttribute(AttributeName = "roomId")]
		public string RoomId { get; set; }
	}

	[XmlRoot(ElementName = "joined")]
	public class Joined
	{

		[XmlAttribute(AttributeName = "roomId")]
		public string RoomId { get; set; }
	}

	[XmlRoot(ElementName = "received")]
	public class Received
	{

		[XmlElement(ElementName = "left")]
		public Left Left { get; set; }
		[XmlElement(ElementName = "joined")]
		public Joined Joined { get; set; }
		[XmlElement(ElementName = "room")]
		public List<Room> Room { get; set; }
	}
}
