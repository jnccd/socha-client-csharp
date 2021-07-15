using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SochaClient
{
    public class Field : ICloneable
    {
        public Piece Piece;
        public Board Parent { get; private set; }
        public readonly int X, Y;

        public Field(Piece piece, Board parent, int x, int y)
        {
            Piece = piece;
            Parent = parent;
            X = x;
            Y = y;
        }

        public bool Empty() => Piece == null;
        public Point Position() => new Point(X, Y);
        public Color ToColor() => Piece == null ? Color.Black : Piece.ToColor();

        public Point[] PossibleCoordsToMoveTo()
        {
            if (Piece == null)
                return new Point[0];

            int xDir = Piece.PColor == PieceColor.RED ? 1 : -1;

            var gotoCoords = new Point[0];
            switch (Piece.Type) 
            {
                case PieceType.Herzmuschel:
                    gotoCoords = new Point[] { new Point(xDir, -1), new Point(xDir, 1) };
                    break;
                case PieceType.Moewe:
                    gotoCoords = new Point[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) };
                    break;
                case PieceType.Robbe:
                    gotoCoords = new Point[] { new Point(-1, 2), new Point(1, 2), new Point(-2, 1), new Point(2, 1), new Point(-1, -2), new Point(1, -2), new Point(-2, -1), new Point(2, -1) };
                    break;
                case PieceType.Seestern:
                    gotoCoords = new Point[] { new Point(xDir, 0), new Point(1, 1), new Point(-1, 1), new Point(1, -1), new Point(-1, -1) };
                    break;
            }

            return gotoCoords.Select(x => new Point(x.X + X, x.Y + Y)).ToArray();
        }

        public object Clone() => (Field)MemberwiseClone();
        public Field CloneWParent(Board cloneParent)
        {
            Field f = (Field)Clone();
            f.Parent = cloneParent;
            return f;
        }
    }
}
