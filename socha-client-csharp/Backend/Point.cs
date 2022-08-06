using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SochaClient
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
    }
}
