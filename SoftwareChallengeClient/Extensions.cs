using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareChallengeClient
{
    public static class Extensions
    {
        public static Point toVector(this Direction Dir)
        {
            switch (Dir)
            {
                case Direction.DOWN:
                    return new Point(0, -1);

                case Direction.DOWN_LEFT:
                    return new Point(-1, -1);

                case Direction.DOWN_RIGHT:
                    return new Point(1, -1);

                case Direction.LEFT:
                    return new Point(-1, 0);

                case Direction.RIGHT:
                    return new Point(1, 0);

                case Direction.UP:
                    return new Point(0, 1);

                case Direction.UP_LEFT:
                    return new Point(-1, 1);

                case Direction.UP_RIGHT:
                    return new Point(1, 1);

                default:
                    throw new Exception("That direction doesn't exist!");
            }
        }

        public static FieldState toFieldState(this PlayerColor Color)
        {
            if (Color == PlayerColor.RED)
                return FieldState.RED;
            else
                return FieldState.BLUE;
        }

        public static PlayerColor otherTeam(this PlayerColor Color)
        {
            if (Color == PlayerColor.RED)
                return PlayerColor.BLUE;
            else
                return PlayerColor.RED;
        }
    }
}
