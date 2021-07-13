using System;
using System.Drawing;

namespace SochaClient
{
    /// <summary>
    /// Represents the Board of the game
    /// </summary>
    public class Board : ICloneable
    {
        public const int Width = 8, Height = 8;
        private readonly Field[,] Fields = new Field[Width, Height];
        
        /// <summary>
        /// Creates an empty board
        /// </summary>
        public Board()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Fields[x, y] = new Field(null, this, x, y);
        }

        /// <summary>
        /// Returns the field on coord (X, Y)
        /// </summary>
        public Field GetField(Point p) => GetField(p.X, p.Y);
        public Field GetField(int x, int y) => Fields[x, y];

        /// <summary>
        /// Checks if the coord (X, Y) is in bounds
        /// </summary>
        public bool IsInBounds(Point p) => IsInBounds(p.X, p.Y);
        public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public object Clone()
        {
            Board b = (Board)MemberwiseClone();
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    b.Fields[x, y] = Fields[x, y].CloneWParent(b);
            return b;
        }
    }
}
