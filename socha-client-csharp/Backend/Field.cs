using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SochaClient
{
    public class Field : ICloneable
    {
        public int fishes = 0;
        public Piece Piece;
        public Board Parent { get; private set; }
        public readonly int X, Y;

        public Field(Piece piece, Board parent, int x, int y)
        {
            Piece = piece;
            Parent = parent;
            this.X = x;
            this.Y = y;
        }

        public bool Empty() => Piece == null && fishes == 0;
        public Point Position() => new(X, Y);
        public Color ToColor() => Empty() ? Color.Black : Piece.ToColor();

        public Point[] PossibleCoordsToMoveTo()
        {
            if (Piece == null)
                return new Point[0];

            int xDir = Piece.Team == PlayerTeam.ONE ? 1 : -1;

            var gotoCoords = new List<Point>();
            
            // TODO: Add logic

            return gotoCoords.Select(x => new Point(x.X + X, x.Y + Y)).ToArray();
        }

        public object Clone() => (Field)MemberwiseClone();
        public Field CloneWParent(Board cloneParent)
        {
            Field f = (Field)Clone();
            f.Parent = cloneParent;
            if (Piece != null)
                f.Piece = (Piece)Piece.Clone();
            return f;
        }
    }
}
