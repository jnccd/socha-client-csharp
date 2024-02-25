using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private int _minQ = 0, _minR = 0, _maxQ = 0, _maxR = 0;

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
        public void SetField(CubeCoords coords, Field f)
        {
            if (f != null)
            {
                if (coords.q < _minQ)
                    _minQ = coords.q;
                if (coords.q > _maxQ)
                    _maxQ = coords.q;
                if (coords.r < _minR)
                    _minR = coords.r;
                if (coords.r > _maxR)
                    _maxR = coords.r;
            }
            Fields[coords.q + HalfSize, coords.r + HalfSize] = f;
        }

        /// <summary>
        /// Checks if the coord (X, Y) is in bounds
        /// </summary>
        public static bool IsInBounds(CubeCoords coords) => IsInBounds(coords.q, coords.r);
        public static bool IsInBounds(int q, int r) => q > -HalfSize && q < HalfSize && r > -HalfSize && r < HalfSize;

        /// <summary>
        /// Prints the board on the console
        /// </summary>
        public void Print(bool stdout = true, Player PlayerOne = null, Player PlayerTwo = null)
        {
            Action<object> write;
            if (stdout)
                write = (o) => Console.Write(o);
            else
                write = (o) => Debug.Write(o);
            
            for (int r = _minR; r < _maxR+1; r++)
            {
                var normR = r - _minR;
                write(Enumerable.Repeat(" ", normR).Combine());
                for (int q = _minQ; q < _maxQ+1; q++)
                {
                    var f = GetField(q, r);

                    if (PlayerOne?.Ship.Pos.q == q && PlayerOne?.Ship.Pos.r == r)
                        write("1|");
                    else if(PlayerTwo?.Ship.Pos.q == q && PlayerTwo?.Ship.Pos.r == r)
                        write("2|");
                    else if (f == null)
                        write("  ");
                    else
                        write($"{f}|");
                }
                write("\n");
            }
        }

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public object Clone()
        {
            Board b = new();
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (Fields[x, y] != null)
                        b.Fields[x, y] = Fields[x, y].CloneWParent(b);
            return b;
        }
    }
}
