using System;
using System.Drawing;
using System.Linq;

namespace SochaClient
{
    public class Field : ICloneable
    {
        public PieceColor? Color { get; private set; }
        public PieceKind? Kind { get; private set; }
        public Board Parent { get; private set; }
        public readonly int X, Y;

        public Field(PieceColor? color, PieceKind? kind, Board parent, int x, int y)
        {
            Color = color;
            Kind = kind;
            Parent = parent;
            X = x;
            Y = y;
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
