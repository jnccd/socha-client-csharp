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
            Point endPoint = B.GetMoveEndpoint(this, moveDistance);
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
