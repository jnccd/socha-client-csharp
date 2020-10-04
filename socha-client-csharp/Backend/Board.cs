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
        public PieceColor?[,] Fields = new PieceColor?[BoardWidth, BoardHeight];
        
        /// <summary>
        /// Creates an empty board
        /// </summary>
        public Board()
        {
            for (int x = 0; x < BoardWidth; x++)
                for (int y = 0; y < BoardHeight; y++)
                    Fields[x, y] = null;
        }

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
                    b.Fields[x, y] = Fields[x, y].Value;
            return b;
        }
    }
}
