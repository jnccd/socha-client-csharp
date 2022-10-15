using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SochaClient.Backend
{
    public class Point
    {
        public int X, Y;

        public Point(int a)
        {
            X = a;
            Y = a;
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);

        public override string ToString() => $"[{X}, {Y}]";
        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();
        public override bool Equals(object obj) => obj is Point point && point.X == X && point.Y == Y;
    }
}
