﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public CubeCoords Add(CubeCoords x) => new CubeCoords(this.q + x.q, this.r + x.r);
        public CubeCoords MultScalar(float scalar) => new CubeCoords((int)(this.q * scalar), (int)(this.r * scalar));
        public CubeCoords Rotate(bool clockwise) => clockwise ? new CubeCoords(-r, -s, -q) : new CubeCoords(-s, -q, -r);
        public CubeCoords RotateByDir(Direction dir)
        {
            switch (dir)
            {
                case Direction.RIGHT:
                    return new CubeCoords(q, r, s);
                case Direction.DOWN_RIGHT:
                    return this.Rotate(true);
                case Direction.DOWN_LEFT:
                    return this.Rotate(true).Rotate(true);
                case Direction.LEFT:
                    return this.Rotate(true).Rotate(true).Rotate(true);
                case Direction.UP_LEFT:
                    return this.Rotate(false).Rotate(false);
                case Direction.UP_RIGHT:
                    return this.Rotate(false);
                default: 
                    throw new ArgumentException("wat");
            }
        }
        public CubeCoords dirToOffset(Direction dir)
        {
            switch (dir)
            {
                case Direction.RIGHT:
                    return new CubeCoords(1, 0, -1);
                case Direction.DOWN_RIGHT:
                    return new CubeCoords(0, 1, -1);
                case Direction.DOWN_LEFT:
                    return new CubeCoords(-1, 1, 0);
                case Direction.LEFT:
                    return new CubeCoords(-1, 0, 1);
                case Direction.UP_LEFT:
                    return new CubeCoords(0, -1, 1);
                case Direction.UP_RIGHT:
                    return new CubeCoords(1, -1, 0);
                default:
                    throw new ArgumentException("wat");
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
