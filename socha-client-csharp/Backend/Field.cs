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
        public bool Free() => Piece == null && fishes != 0;
        public Point Position() => new(X, Y);
        public Color ToColor() => Piece == null ? Color.FromArgb(0,0,fishes*80) : Piece.ToColor();

        public Point[] PossibleCoordsToMoveTo()
        {
            if (Piece == null)
                return new Point[0];

            var gotoCoords = new List<Point>();

            var dirs = (Direction[])Enum.GetValues(typeof(Direction));
            Point curPos = Position();
            for (int i = 0; i < dirs.Length; i++)
            {
                curPos += Board.GetDirectionDisplacement(dirs[i], curPos);
                while (Parent.GetField(curPos).Free() && Board.IsInBounds(curPos))
                {
                    gotoCoords.Add(curPos);
                    curPos += Board.GetDirectionDisplacement(dirs[i], curPos);
                }
            }

            return gotoCoords.ToArray();
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
