using System;
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

        public object Clone() => (Field)MemberwiseClone();
        public Field CloneWParent(Board cloneParent)
        {
            Field f = (Field)Clone();
            f.Parent = cloneParent;
            return f;
        }

        internal object MoveToCoords()
        {
            throw new NotImplementedException();
        }
    }
}
