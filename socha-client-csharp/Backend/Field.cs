using System;
using System.Drawing;
using System.Linq;

namespace SochaClient
{
    public class Field : ICloneable
    {
        public PieceColor? color;
        Board parent;
        readonly int x, y;

        public Field(PieceColor? color, Board parent, int x, int y)
        {
            this.color = color;
            this.parent = parent;
            this.x = x;
            this.y = y;
        }

        public Field[] Get4Neighborhood() => (
                from p in new Point[] { new Point(x - 1, y), new Point(x + 1, y), new Point(x, y - 1), new Point(x, y + 1) }
                where parent.IsInBounds(p)
                select parent.GetField(p)
            ).ToArray();

        public Field[] Get8Neighborhood() => (
                from p in new Point[] { new Point(x - 1, y), new Point(x + 1, y), new Point(x, y - 1), new Point(x, y + 1),
                                        new Point(x - 1, y + 1), new Point(x - 1, y - 1), new Point(x + 1, y + 1), new Point(x + 1, y - 1) }
                where parent.IsInBounds(p)
                select parent.GetField(p)
            ).ToArray();

        public object Clone() => (Field)MemberwiseClone();
        public Field CloneWParent(Board cloneParent)
        {
            Field f = (Field)Clone();
            f.parent = cloneParent;
            return f;
        }
    }
}
