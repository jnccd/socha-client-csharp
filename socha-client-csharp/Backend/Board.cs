using System;
using System.Linq;

namespace SochaClient
{
    /// <summary>
    /// Represents the Board of the game
    /// </summary>
    public class Board : ICloneable
    {
        public const int Width = 8, Height = 8;
        private readonly Field[,] Fields = new Field[Width, Height];
        private readonly Field[] FlatFields = new Field[Width * Height];

        public static readonly Point[] EvenHexNeighbors = { new(-1), new(0, -1), new(1, 0), new(0, 1), new(-1, 1), new(-1, 0), };
        public static readonly Point[] OddHexNeighbors = { new(0, -1), new(1, -1), new(1, 0), new(1), new(0, 1), new(-1, 0), };

        /// <summary>
        /// Creates an empty board
        /// </summary>
        public Board()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    Fields[x, y] = new Field(null, this, x, y);
                    FlatFields[x * Height + y] = Fields[x, y];
                }
        }

        /// <summary>
        /// Returns the field on coord (X, Y)
        /// </summary>
        public Field GetField(Point p) => GetField(p.X, p.Y);
        public Field GetField(int x, int y) => Fields[x, y];
        public Field[] GetFlatFields() => (Field[])FlatFields.Clone();

        /// <summary>
        /// Checks if the coord (X, Y) is in bounds
        /// </summary>
        public static bool IsInBounds(Point p) => IsInBounds(p.X, p.Y);
        public static bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

        /// <summary>
        /// Returns possible vectors for all Directions in hexspace
        /// </summary>
        public static Point[] GetNeighbors(Point pos) => pos.Y % 2 == 0 ? EvenHexNeighbors : OddHexNeighbors;
        /// <summary>
        /// Returns a vector that points in the given Direction in hexspace
        /// </summary>
        public static Point GetDirectionDisplacement(Direction d, Point pos) => GetNeighbors(pos)[(int)d];

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public object Clone()
        {
            Board b = new();
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    b.Fields[x, y] = Fields[x, y].CloneWParent(b);
                    b.FlatFields[x * Height + y] = b.Fields[x, y];
                }
            return b;
        }
    }
}
