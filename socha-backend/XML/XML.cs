using System.Collections.Generic;
using System.Xml.Serialization;

// Generated using: https://json2csharp.com/code-converters/xml-to-csharp

namespace SochaClient.Backend.XML
{
	[XmlRoot(ElementName = "data")]
	public class Data
	{

		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }

		[XmlAttribute(AttributeName = "color")]
		public PlayerTeam Color { get; set; }

		[XmlElement(ElementName = "state")]
		public State State { get; set; }

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

	[XmlRoot(ElementName = "list")]
	public class List
	{

		[XmlElement(ElementName = "field")]
		public List<string> Field { get; set; }
	}

	[XmlRoot(ElementName = "board")]
	public class Board
	{

		[XmlElement(ElementName = "list")]
		public List<List> List { get; set; }
	}

	[XmlRoot(ElementName = "fishes")]
	public class Fishes
	{

		[XmlElement(ElementName = "int")]
		public List<int> Int { get; set; }
	}

	[XmlRoot(ElementName = "state")]
	public class State
	{

		[XmlElement(ElementName = "startTeam")]
		public PlayerTeam StartTeam { get; set; }

		[XmlElement(ElementName = "board")]
		public Board Board { get; set; }

		[XmlElement(ElementName = "fishes")]
		public Fishes Fishes { get; set; }

		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }

		[XmlAttribute(AttributeName = "turn")]
		public int Turn { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "Root")]
	public class Root
	{

		[XmlElement(ElementName = "joined")]
		public Joined Joined { get; set; }

		[XmlElement(ElementName = "room")]
		public List<Room> Room { get; set; }
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
