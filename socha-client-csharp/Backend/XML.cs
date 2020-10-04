using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Xml
{
	[XmlRoot(ElementName = "startTeam")]
	public class StartTeam
	{
		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "field")]
	public class Field
	{
		[XmlAttribute(AttributeName = "x")]
		public int X { get; set; }
		[XmlAttribute(AttributeName = "y")]
		public int Y { get; set; }
		[XmlAttribute(AttributeName = "content")]
		public string Content { get; set; }
	}

	[XmlRoot(ElementName = "board")]
	public class Board
	{
		[XmlElement(ElementName = "field")]
		public List<Field> Field { get; set; }
	}

	[XmlRoot(ElementName = "blueShapes")]
	public class BlueShapes
	{
		[XmlElement(ElementName = "shape")]
		public List<string> Shape { get; set; }
		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }
	}

	[XmlRoot(ElementName = "yellowShapes")]
	public class YellowShapes
	{
		[XmlElement(ElementName = "shape")]
		public List<string> Shape { get; set; }
		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }
	}

	[XmlRoot(ElementName = "redShapes")]
	public class RedShapes
	{
		[XmlElement(ElementName = "shape")]
		public List<string> Shape { get; set; }
		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }
	}

	[XmlRoot(ElementName = "greenShapes")]
	public class GreenShapes
	{
		[XmlElement(ElementName = "shape")]
		public List<string> Shape { get; set; }
		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }
	}

	[XmlRoot(ElementName = "lastMoveMono")]
	public class LastMoveMono
	{
		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }
	}

	[XmlRoot(ElementName = "orderedColors")]
	public class OrderedColors
	{
		[XmlElement(ElementName = "color")]
		public List<string> Color { get; set; }
	}

	[XmlRoot(ElementName = "color")]
	public class Color
	{
		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "first")]
	public class First
	{
		[XmlElement(ElementName = "color")]
		public Color Color { get; set; }
		[XmlAttribute(AttributeName = "displayName")]
		public string DisplayName { get; set; }
	}

	[XmlRoot(ElementName = "second")]
	public class Second
	{
		[XmlElement(ElementName = "color")]
		public Color Color { get; set; }
		[XmlAttribute(AttributeName = "displayName")]
		public string DisplayName { get; set; }
	}

	[XmlRoot(ElementName = "position")]
	public class Position
	{
		[XmlAttribute(AttributeName = "x")]
		public int X { get; set; }
		[XmlAttribute(AttributeName = "y")]
		public int Y { get; set; }
	}

	[XmlRoot(ElementName = "piece")]
	public class Piece
	{
		[XmlElement(ElementName = "position")]
		public Position Position { get; set; }
		[XmlAttribute(AttributeName = "color")]
		public string Color { get; set; }
		[XmlAttribute(AttributeName = "kind")]
		public string Kind { get; set; }
		[XmlAttribute(AttributeName = "rotation")]
		public string Rotation { get; set; }
		[XmlAttribute(AttributeName = "isFlipped")]
		public string IsFlipped { get; set; }
	}

	[XmlRoot(ElementName = "lastMove")]
	public class LastMove
	{
		[XmlElement(ElementName = "piece")]
		public Piece Piece { get; set; }
		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }
	}

	[XmlRoot(ElementName = "state")]
	public class State
	{
		[XmlElement(ElementName = "startTeam")]
		public StartTeam StartTeam { get; set; }
		[XmlElement(ElementName = "board")]
		public Board Board { get; set; }
		[XmlElement(ElementName = "blueShapes")]
		public BlueShapes BlueShapes { get; set; }
		[XmlElement(ElementName = "yellowShapes")]
		public YellowShapes YellowShapes { get; set; }
		[XmlElement(ElementName = "redShapes")]
		public RedShapes RedShapes { get; set; }
		[XmlElement(ElementName = "greenShapes")]
		public GreenShapes GreenShapes { get; set; }
		[XmlElement(ElementName = "lastMoveMono")]
		public LastMoveMono LastMoveMono { get; set; }
		[XmlElement(ElementName = "orderedColors")]
		public OrderedColors OrderedColors { get; set; }
		[XmlElement(ElementName = "first")]
		public First First { get; set; }
		[XmlElement(ElementName = "second")]
		public Second Second { get; set; }
		[XmlElement(ElementName = "lastMove")]
		public LastMove LastMove { get; set; }
		[XmlElement(ElementName = "startColor")]
		public string StartColor { get; set; }
		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }
		[XmlAttribute(AttributeName = "currentColorIndex")]
		public string CurrentColorIndex { get; set; }
		[XmlAttribute(AttributeName = "turn")]
		public int Turn { get; set; }
		[XmlAttribute(AttributeName = "round")]
		public int Round { get; set; }
		[XmlAttribute(AttributeName = "startPiece")]
		public string StartPiece { get; set; }
	}

	[XmlRoot(ElementName = "data")]
	public class Data
	{
		[XmlElement(ElementName = "state")]
		public State State { get; set; }
		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }
		[XmlAttribute(AttributeName = "color")]
		public string Color { get; set; }
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

	[XmlRoot(ElementName = "joined")]
	public class Joined
	{
		[XmlAttribute(AttributeName = "roomId")]
		public string RoomId { get; set; }
	}

	[XmlRoot(ElementName = "received")]
	public class Received
	{
		[XmlElement(ElementName = "joined")]
		public Joined Joined { get; set; }
		[XmlElement(ElementName = "room")]
		public List<Room> Room { get; set; }
	}
}
