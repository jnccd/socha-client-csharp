using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socha_client_csharp
{
    /// <summary>
    /// Represents the Board of the game
    /// </summary>
    public class Board : ICloneable
    {
        public const int BoardWidth = 20, BoardHeight = 20;
        private Field[,] Fields = new Field[BoardWidth, BoardHeight];
        
        /// <summary>
        /// Creates an empty board
        /// </summary>
        public Board()
        {
            for (int x = 0; x < BoardWidth; x++)
                for (int y = 0; y < BoardHeight; y++)
                    Fields[x, y] = new Field(null, this, x, y);
        }

        /// <summary>
        /// Returns the field on coord (X, Y)
        /// </summary>
        public Field GetField(Point p) => GetField(p.X, p.Y);
        public Field GetField(int x, int y) => Fields[x, y];

        /// <summary>
        /// Checks if the coord (X, Y) is in bounds
        /// </summary>
        public bool IsInBounds(Point p) => IsInBounds(p.X, p.Y);
        public bool IsInBounds(int x, int y) => x >= 0 && x < BoardWidth && y >= 0 && y < BoardHeight;

        /// <summary>
        /// Gets all legal Moves for a Team
        /// </summary>
        public List<SetMove> GetAllPossibleMoves(PlayerTeam Team)
        {
            //List<SetMove> temp = new List<SetMove>();
            //for (int x = 0; x < BoardWidth; x++)
            //    for (int y = 0; y < BoardHeight; y++)
            //        if (Fields[x, y].State == Team.ToFieldState())
            //            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            //                if ((new SetMove(x, y, dir)) is SetMove m && m.IsLegalOn(this, Team))
            //                    temp.Add(m);
            //return temp;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public object Clone()
        {
            Board b = (Board)MemberwiseClone();
            for (int x = 0; x < BoardWidth; x++)
                for (int y = 0; y < BoardHeight; y++)
                    b.Fields[x, y] = Fields[x, y].CloneWParent(b);
            return b;
        }
    }
}
