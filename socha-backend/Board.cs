using System;
using System.Collections.Generic;
using System.Linq;

namespace SochaClient.Backend
{
    /// <summary>
    /// Represents the Board of the game
    /// </summary>
    public class Board : ICloneable
    {
        public const int Width = 150, Height = 150, HalfSize = 75;
        private readonly Field[,] Fields = new Field[Width, Height];

        /// <summary>
        /// Creates an empty board
        /// </summary>
        public Board()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Fields[x, y] = null;
        }

        /// <summary>
        /// Returns the field on coord (X, Y)
        /// </summary>
        public Field GetField(CubeCoords coords) => GetField(coords.q, coords.r);
        public Field GetField(int q, int r) => Fields[q + HalfSize, r + HalfSize];
        public void SetField(CubeCoords coords, Field f) => Fields[coords.q + HalfSize, coords.r + HalfSize] = f;

        /// <summary>
        /// Checks if the coord (X, Y) is in bounds
        /// </summary>
        public static bool IsInBounds(CubeCoords coords) => IsInBounds(coords.q, coords.r);
        public static bool IsInBounds(int q, int r) => q > -HalfSize && q < HalfSize && r > -HalfSize && r < HalfSize;

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public object Clone()
        {
            Board b = new();
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    b.Fields[x, y] = Fields[x, y].CloneWParent(b);
            return b;
        }
    }
}
