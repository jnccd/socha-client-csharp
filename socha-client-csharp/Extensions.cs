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
        /// <summary>
        /// Converts the Direction into a Point that is in the given Direction from the Point (0, 0)
        /// </summary>
        public static Point ToVector(this Direction Dir)
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

        /// <summary>
        /// Converts the Color into the Fieldstate of the Color
        /// </summary>
        public static FieldState ToFieldState(this PlayerColor Color)
        {
            if (Color == PlayerColor.RED)
                return FieldState.RED;
            else
                return FieldState.BLUE;
        }

        /// <summary>
        /// Gets the opposite Team
        /// </summary>
        public static PlayerColor OtherTeam(this PlayerColor Color)
        {
            if (Color == PlayerColor.RED)
                return PlayerColor.BLUE;
            else
                return PlayerColor.RED;
        }
    }
}
