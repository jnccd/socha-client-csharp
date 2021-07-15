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

        public Point[] PossibleCoordsToMoveTo()
        {
            int xDir = Piece.Color == PieceColor.RED ? 1 : -1;

            var gotoCoords = new Point[0];
            switch (Piece.Kind) 
            {
                case PieceKind.Herzmuschel:
                    gotoCoords = new Point[] { new Point(xDir, -1), new Point(xDir, 1) };
                    break;
                case PieceKind.Möwe:
                    gotoCoords = new Point[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) };
                    break;
                case PieceKind.Robbe:
                    gotoCoords = new Point[] { new Point(-1, 2), new Point(1, 2), new Point(-2, 1), new Point(2, 1), new Point(-1, -2), new Point(1, -2), new Point(-2, -1), new Point(2, -1) };
                    break;
                case PieceKind.Seestern:
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
