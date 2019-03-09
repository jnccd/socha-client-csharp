using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareChallengeClient
{
    /// <summary>
    /// Represents the Board of the game
    /// </summary>
    public class Board
    {
        public const int BoardWidth = 10, BoardHeight = 10;
        public Field[,] Fields = new Field[BoardWidth, BoardHeight];

        /// <summary>
        /// Creates an empty board
        /// </summary>
        public Board()
        {
            for (int x = 0; x < BoardWidth; x++)
                for (int y = 0; y < BoardHeight; y++)
                    Fields[x, y] = new Field();
        }

        /// <summary>
        /// Checks how many fish are in the given Direction and the opposite direction
        /// </summary>
        /// <param name="X"> X value of the starting Location </param>
        /// <param name="Y"> Y value of the starting Location </param>
        /// <param name="Dir"> The Direction to check in, due to the nature of this Method putting opposite directions here will yield equal results </param>
        public int NumberOfFishInRow(int X, int Y, Direction Dir)
        {
            int num = 0;

            // Vertical
            if (Dir == Direction.DOWN || Dir == Direction.UP)
                for (int i = 0; i < BoardHeight; i++)
                    if (Fields[X, i].HasPiranha())
                        num++;

            // Horizontal
            if (Dir == Direction.RIGHT || Dir == Direction.LEFT)
                for (int i = 0; i < BoardWidth; i++)
                    if (Fields[i, Y].HasPiranha())
                        num++;

            // Aufsteigend
            if (Dir == Direction.UP_RIGHT || Dir == Direction.DOWN_LEFT)
            {
                int x = X, y = Y;
                while (x >= 0 && y >= 0)
                {
                    if (Fields[x, y].HasPiranha())
                        num++;
                    x--; y--;
                }
                x = X + 1; y = Y + 1;
                while (x < BoardWidth && y < BoardHeight)
                {
                    if (Fields[x, y].HasPiranha())
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
                    if (Fields[x, y].HasPiranha())
                        num++;
                    x--; y++;
                }
                x = X + 1; y = Y - 1;
                while (x < BoardWidth && y >= 0)
                {
                    if (Fields[x, y].HasPiranha())
                        num++;
                    x++; y--;
                }
            }

            return num;
        }

        /// <summary>
        /// Gets Num Fields in the given Direction
        /// <para> Caution: Contains the starting Field </para>
        /// </summary>
        /// <param name="X"> X value of the starting Location </param>
        /// <param name="Y"> Y value of the starting Location </param>
        /// <param name="Dir"> The Direction the Fields will be taken from </param>
        /// <param name="Num"> The Number of Fields that will be taken </param>
        public List<Field> GetFieldsInDir(int X, int Y, Direction Dir, int Num)
        {
            if (Num <= 0)
                return new List<Field>();
            else
            {
                List<Field> temp = new List<Field>();
                Point vec = Dir.ToVector();
                for (int i = 0; i < Num && (X + i * vec.X) >= 0 &&
                                           (X + i * vec.X) < BoardWidth &&
                                           (Y + i * vec.Y) >= 0 &&
                                           (Y + i * vec.Y) < BoardHeight; i++)
                    temp.Add(Fields[X + i * vec.X, Y + i * vec.Y]);
                return temp;
            }
        }

        /// <summary>
        /// Gets all the possible legal Moves for a Team
        /// </summary>
        public List<Move> GetAllPossibleMoves(PlayerColor Team)
        {
            List<Move> temp = new List<Move>();
            for (int x = 0; x < BoardWidth; x++)
                for (int y = 0; y < BoardHeight; y++)
                    if (Fields[x, y].State == Team.ToFieldState())
                        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                            if ((new Move(x, y, dir)) is Move m && m.IsLegalOn(this, Team))
                                temp.Add(m);
            return temp;
        }
    }
}
