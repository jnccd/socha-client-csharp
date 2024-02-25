using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SochaClient.Backend.Move;

namespace SochaClient.Backend
{
    public class CubeCoords : ICloneable
    {
        public int q, r;
        public int s { get => -(q+r); set { } }

        public CubeCoords(int q, int r, int? s = null)
        {
            this.q = q;
            this.r = r;

            if (s != null && s != this.s)
                throw new ArgumentException("Incorrect s");
        }

        public CubeCoords Rotate(bool clockwise) => 
            clockwise ? new CubeCoords(-r, -s, -q) : new CubeCoords(-s, -q, -r);
        public CubeCoords RotateByDir(Direction dir) =>
            dir switch
            {
                Direction.RIGHT => new CubeCoords(q, r, s),
                Direction.DOWN_RIGHT => this.Rotate(true),
                Direction.DOWN_LEFT => this.Rotate(true).Rotate(true),
                Direction.LEFT => this.Rotate(true).Rotate(true).Rotate(true),
                Direction.UP_LEFT => this.Rotate(false).Rotate(false),
                Direction.UP_RIGHT => this.Rotate(false),
                _ => throw new ArgumentException("wat"),
            };
        public static CubeCoords DirToOffset(Direction dir) =>
            dir switch
            {
                Direction.RIGHT => new CubeCoords(1, 0, -1),
                Direction.DOWN_RIGHT => new CubeCoords(0, 1, -1),
                Direction.DOWN_LEFT => new CubeCoords(-1, 1, 0),
                Direction.LEFT => new CubeCoords(-1, 0, 1),
                Direction.UP_LEFT => new CubeCoords(0, -1, 1),
                Direction.UP_RIGHT => new CubeCoords(1, -1, 0),
                _ => throw new ArgumentException("wat"),
            };

        public static CubeCoords operator +(CubeCoords a, CubeCoords b)
            => new(a.q + b.q, a.r + b.r);
        public static CubeCoords operator *(CubeCoords a, float scalar)
            => new((int)(a.q * scalar), (int)(a.r * scalar));
        public static bool operator ==(CubeCoords a, CubeCoords b)
            => (a is null && b is null) || (a is not null && a.Equals(b));
        public static bool operator !=(CubeCoords a, CubeCoords b)
            => (a is not null || b is not null) && (a is null || !a.Equals(b));

        public override int GetHashCode() => q + r * 10000;
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(CubeCoords))
                return false;
            var cobj = obj as CubeCoords;
            return cobj.q == q && cobj.r == r;
        }
        public object Clone() => MemberwiseClone();
    }
}
