using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SochaClient.Backend
{
    public static class Cache
    {
        public static readonly CubeCoords[] segOffsetStarts;
        public static readonly CubeCoords[][] segOffsets;

        static Cache()
        {
            segOffsetStarts = new CubeCoords[] { new(-1, -2), new(-1, -1), new(-1, 0), new(-2, 1), new(-3, 2) };
            segOffsets = segOffsetStarts.
                Select(oStart => 
                    Enumerable.Range(0, 5).
                    Select(f => 
                        oStart + CubeCoords.DirToOffset(Direction.RIGHT) * f).
                        ToArray()).
                    ToArray();
        }
    }
}
