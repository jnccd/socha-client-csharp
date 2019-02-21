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
        public List<string> DebugHints;

        public Move(int X, int Y, Direction MoveDirection)
        {
            this.X = X;
            this.Y = Y;
            this.MoveDirection = MoveDirection;
            DebugHints = new List<string>();
        }

        public string ToXML()
        {
            if (this == null)
                return "";
            
            return $"<room roomId=\"{Program.RoomID}\"><data class=\"move\" x=\"{X}\" y=\"{Y}\" direction=\"{MoveDirection}\">" +
                $"{DebugHints.Select(x => $"<hint content=\"{x}\"/>").Aggregate((x, y) => x + y)}</data></room>";
        }
    }

    public enum Direction { UP, UP_RIGHT, RIGHT, DOWN_RIGHT, DOWN, DOWN_LEFT, LEFT, UP_LEFT }
}
