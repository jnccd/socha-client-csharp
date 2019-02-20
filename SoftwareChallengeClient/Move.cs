using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareChallengeClient
{
    public class Move
    {
        public int X, Y;
        public Direction MoveDirection;

        public Move(int X, int Y, Direction MoveDirection)
        {
            this.X = X;
            this.Y = Y;
            this.MoveDirection = MoveDirection;
        }

        public string toXML()
        {
            if (this == null)
                return "";

            return $"<room roomId=\"{Program.RoomIDread}\"><data class=\"move\" x=\"{X}\" y=\"{Y}\" direction=\"{MoveDirection}\"></data></room>";
        }
    }

    public enum Direction { UP, UP_RIGHT, RIGHT, DOWN_RIGHT, DOWN, DOWN_LEFT, LEFT, UP_LEFT }
}
