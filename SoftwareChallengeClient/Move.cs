using System;
using System.Collections.Generic;
using System.Drawing;
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

        public bool IsLegalOn(Board B, PlayerColor Team) // https://youtu.be/nz20lu2AM2k?t=8
        {
            int moveDistance = B.NumberOfFishInRow(X, Y, MoveDirection);
            Point endPoint = this.GetEndpointOn(B, moveDistance);
            return endPoint.X >= 0 &&
                   endPoint.X < Board.BoardWidth &&
                   endPoint.Y >= 0 &&
                   endPoint.Y < Board.BoardHeight &&
                   B.Fields[X, Y].HasPiranha() &&
                   B.Fields[X, Y].State == Team.ToFieldState() &&
                   B.GetFieldsInDir(X, Y, MoveDirection, moveDistance).
                    TrueForAll(x => x.State != Team.OtherTeam().ToFieldState()) &&
                   (B.Fields[endPoint.X, endPoint.Y].State == FieldState.EMPTY ||
                    B.Fields[endPoint.X, endPoint.Y].HasPiranha() &&
                    B.Fields[endPoint.X, endPoint.Y].State != Team.ToFieldState());
        }

        public Point GetEndpointOn(Board B)
        {
            int moveDistance = B.NumberOfFishInRow(X, Y, MoveDirection);
            return GetEndpointOn(B, moveDistance);
        }
        public Point GetEndpointOn(Board B, int MoveDistance)
        {
            switch (MoveDirection)
            {
                case Direction.DOWN:
                    return new Point(X, Y - MoveDistance);

                case Direction.DOWN_LEFT:
                    return new Point(X - MoveDistance, Y - MoveDistance);

                case Direction.DOWN_RIGHT:
                    return new Point(X + MoveDistance, Y - MoveDistance);

                case Direction.LEFT:
                    return new Point(X - MoveDistance, Y);

                case Direction.RIGHT:
                    return new Point(X + MoveDistance, Y);

                case Direction.UP:
                    return new Point(X, Y + MoveDistance);

                case Direction.UP_LEFT:
                    return new Point(X - MoveDistance, Y + MoveDistance);

                case Direction.UP_RIGHT:
                    return new Point(X + MoveDistance, Y + MoveDistance);

                default:
                    throw new Exception("That direction doesn't exist!");
            }
        }

        public string ToXML()
        {
            if (this == null)
                return "";
            
            return $"<room roomId=\"{Program.RoomID}\"><data class=\"move\" x=\"{X}\" y=\"{Y}\" direction=\"{MoveDirection}\">" +
                $"{(DebugHints.Count > 0 ? DebugHints.Select(x => $"<hint content=\"{x}\"/>").Aggregate((x, y) => x + y) : "")}</data></room>";
        }
    }

    public enum Direction { UP, UP_RIGHT, RIGHT, DOWN_RIGHT, DOWN, DOWN_LEFT, LEFT, UP_LEFT }
}
