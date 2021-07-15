using SochaClient;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Xml
{
	[XmlRoot(ElementName = "startTeam")]
	public class StartTeam
	{

		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }

		[XmlText]
		public PlayerTeam Text { get; set; }
	}

	[XmlRoot(ElementName = "coordinates")]
	public class Coordinates
	{

		[XmlAttribute(AttributeName = "x")]
		public int X { get; set; }

		[XmlAttribute(AttributeName = "y")]
		public int Y { get; set; }
	}

	[XmlRoot(ElementName = "piece")]
	public class Piece
	{

		[XmlAttribute(AttributeName = "type")]
		public PieceType Type { get; set; }

		[XmlAttribute(AttributeName = "team")]
		public PlayerTeam Team { get; set; }

		[XmlAttribute(AttributeName = "count")]
		public int Count { get; set; }
	}

	[XmlRoot(ElementName = "entry")]
	public class Entry
	{

		[XmlElement(ElementName = "coordinates")]
		public Coordinates Coordinates { get; set; }

		[XmlElement(ElementName = "piece")]
		public Piece Piece { get; set; }

		[XmlElement(ElementName = "team")]
		public string Team { get; set; }

		[XmlElement(ElementName = "int")]
		public int Int { get; set; }
	}

	[XmlRoot(ElementName = "pieces")]
	public class Pieces
	{

		[XmlElement(ElementName = "entry")]
		public List<Entry> Entry { get; set; }
	}

	[XmlRoot(ElementName = "board")]
	public class Board
	{

		[XmlElement(ElementName = "pieces")]
		public Pieces Pieces { get; set; }
	}

	[XmlRoot(ElementName = "from")]
	public class From
	{

		[XmlAttribute(AttributeName = "x")]
		public int X { get; set; }

		[XmlAttribute(AttributeName = "y")]
		public int Y { get; set; }
	}

	[XmlRoot(ElementName = "to")]
	public class To
	{

		[XmlAttribute(AttributeName = "x")]
		public int X { get; set; }

		[XmlAttribute(AttributeName = "y")]
		public int Y { get; set; }
	}

	[XmlRoot(ElementName = "lastMove")]
	public class LastMove
	{

		[XmlElement(ElementName = "from")]
		public From From { get; set; }

		[XmlElement(ElementName = "to")]
		public To To { get; set; }
	}

	[XmlRoot(ElementName = "ambers")]
	public class Ambers
	{

		[XmlElement(ElementName = "entry")]
		public List<Entry> Entry { get; set; }

		[XmlAttribute(AttributeName = "enum-type")]
		public string EnumType { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "state")]
	public class State
	{

		[XmlElement(ElementName = "startTeam")]
		public StartTeam StartTeam { get; set; }

		[XmlElement(ElementName = "board")]
		public Board Board { get; set; }

		[XmlElement(ElementName = "lastMove")]
		public LastMove LastMove { get; set; }

		[XmlElement(ElementName = "ambers")]
		public Ambers Ambers { get; set; }

		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }

		[XmlAttribute(AttributeName = "turn")]
		public int Turn { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "data")]
	public class Data
	{

		[XmlElement(ElementName = "state")]
		public State State { get; set; }

		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }

		[XmlAttribute(AttributeName = "color")]
		public PlayerTeam Color { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "room")]
	public class Room
	{

		[XmlElement(ElementName = "data")]
		public Data Data { get; set; }

		[XmlAttribute(AttributeName = "roomId")]
		public string RoomId { get; set; }

		[XmlText]
		public string Text { get; set; }
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
