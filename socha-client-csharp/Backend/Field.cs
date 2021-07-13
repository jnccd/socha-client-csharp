using System;
using System.Drawing;
using System.Linq;

namespace SochaClient
{
    public class Field : ICloneable
    {
        public PieceColor? color { get; private set; }
        public PieceKind? kind { get; private set; }
        public Board parent { get; private set; }
        public readonly int x, y;

        public Field(PieceColor? color, PieceKind? kind, Board parent, int x, int y)
        {
            this.color = color;
            this.kind = kind;
            this.parent = parent;
            this.x = x;
            this.y = y;
        }

        public object Clone() => (Field)MemberwiseClone();
        public Field CloneWParent(Board cloneParent)
        {
            Field f = (Field)Clone();
            f.parent = cloneParent;
            return f;
        }
    }
}
