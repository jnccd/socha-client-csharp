using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareChallengeClient
{
    public class Board
    {
        const int BoardWidth = 10, BoardHeight = 10;
        public Field[,] Fields = new Field[BoardWidth, BoardHeight];

        public Board()
        {
            for (int x = 0; x < BoardWidth; x++)
                for (int y = 0; y < BoardHeight; y++)
                    Fields[x, y] = new Field();
        }

        public int NumberOfFishInRow(int X, int Y, Direction Dir)
        {
            int num = 0;

            // Vertical
            if (Dir == Direction.DOWN || Dir == Direction.UP)
                for (int i = 0; i < BoardHeight; i++)
                    if (Fields[X, i].hasPiranha())
                        num++;

            // Horizontal
            if (Dir == Direction.DOWN || Dir == Direction.UP)
                for (int i = 0; i < BoardWidth; i++)
                    if (Fields[i, Y].hasPiranha())
                        num++;

            // Aufsteigend
            if (Dir == Direction.UP_RIGHT || Dir == Direction.DOWN_LEFT)
            {
                int x = X, y = Y;
                while (x >= 0 && y >= 0)
                {
                    if (Fields[x, y].hasPiranha())
                        num++;
                    x--; y--;
                }
                x = X + 1; y = Y + 1;
                while (x < BoardWidth && y < BoardHeight)
                {
                    if (Fields[x, y].hasPiranha())
                        num++;
                    x++; y++;
                }
            }

            // Fallend
            if (Dir == Direction.UP_LEFT || Dir == Direction.DOWN_RIGHT)
            {
                int x = X, y = Y;
                while (x >= 0 && y < BoardHeight)
                {
                    if (Fields[x, y].hasPiranha())
                        num++;
                    x--; y++;
                }
                x = X + 1; y = Y - 1;
                while (x < BoardWidth && y >= 0)
                {
                    if (Fields[x, y].hasPiranha())
                        num++;
                    x++; y--;
                }
            }

            return num;
        }

        public Point GetMoveEndpoint(Move M)
        {
            int moveDistance = NumberOfFishInRow(M.X, M.Y, M.MoveDirection);
            return GetMoveEndpoint(M, moveDistance);
        }
        public Point GetMoveEndpoint(Move M, int MoveDistance)
        {
            switch (M.MoveDirection)
            {
                case Direction.DOWN:
                    return new Point(M.X,                M.Y - MoveDistance);

                case Direction.DOWN_LEFT:
                    return new Point(M.X - MoveDistance, M.Y - MoveDistance);

                case Direction.DOWN_RIGHT:
                    return new Point(M.X + MoveDistance, M.Y - MoveDistance);

                case Direction.LEFT:
                    return new Point(M.X - MoveDistance, M.Y               );

                case Direction.RIGHT:
                    return new Point(M.X + MoveDistance, M.Y               );

                case Direction.UP:
                    return new Point(M.X,                M.Y + MoveDistance);

                case Direction.UP_LEFT:
                    return new Point(M.X - MoveDistance, M.Y + MoveDistance);

                case Direction.UP_RIGHT:
                    return new Point(M.X + MoveDistance, M.Y + MoveDistance);

                default:
                    throw new Exception("That direction doesn't exist!");
            }
        }

        public List<Field> GetFieldsInDir(int X, int Y, Direction Dir, int Num)
        {
            if (Num <= 0)
                return new List<Field>();
            else
            {
                List<Field> temp = new List<Field>();
                Point vec = Dir.toVector();
                for (int i = 0; i < Num && (X + i * vec.X) >= 0 &&
                                           (X + i * vec.X) < BoardWidth &&
                                           (Y + i * vec.Y) >= 0 &&
                                           (Y + i * vec.Y) < BoardHeight; i++)
                    temp.Add(Fields[X + i * vec.X, Y + i * vec.Y]);
                return temp;
            }
        }
        
        public bool IsLegal(Move M, PlayerColor Team) // https://youtu.be/nz20lu2AM2k?t=8
        {
            int moveDistance = NumberOfFishInRow(M.X, M.Y, M.MoveDirection);
            Point endPoint = GetMoveEndpoint(M, moveDistance);

            M.DebugHints.Add("Movedistance = " + moveDistance);
            M.DebugHints.Add("endPoint = " + endPoint);
            var fields = GetFieldsInDir(M.X, M.Y, M.MoveDirection, moveDistance - 1);
            if (moveDistance > 0)
            {
                try
                {
                    M.DebugHints.Add("Move Over Fields = " + fields.Select(x => x.X + " " + x.Y + " " + x.State).Aggregate((x, y) => x + " " + y));
                }
                catch { }
            }
            M.DebugHints.Add("No enemies in the way = " + (fields.Where(x => x.State == Team.otherTeam().toFieldState()).Count() == 0));

            return endPoint.X >= 0 &&
                   endPoint.X < BoardWidth &&
                   endPoint.Y >= 0 &&
                   endPoint.Y < BoardHeight && 
                   Fields[M.X, M.Y].hasPiranha() &&
                   Fields[M.X, M.Y].State == Team.toFieldState() &&
                   GetFieldsInDir(M.X, M.Y, M.MoveDirection, moveDistance - 1).
                    Where(x => x.State == Team.otherTeam().toFieldState()).Count() == 0 &&
                   (Fields[endPoint.X, endPoint.Y].State == FieldState.EMPTY || 
                    Fields[endPoint.X, endPoint.Y].hasPiranha() && 
                    Fields[endPoint.X, endPoint.Y].State != Team.toFieldState());
        }

        public List<Move> GetAllPossibleMoves(PlayerColor Team)
        {
            List<Move> temp = new List<Move>();
            for (int x = 0; x < BoardWidth; x++)
                for (int y = 0; y < BoardHeight; y++)
                    if (Fields[x, y].State == Team.toFieldState())
                        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                        {
                            Move m = new Move(x, y, dir);
                            if (IsLegal(m, Team))
                                temp.Add(m);
                        }
            return temp;
        }
    }
}
