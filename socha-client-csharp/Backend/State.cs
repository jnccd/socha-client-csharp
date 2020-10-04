using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socha_client_csharp
{
    public class IllegalMoveException : Exception { }

    /// <summary>
    /// Contains all the information of the current GameState
    /// </summary>
    public class State : ICloneable
    {
        public PieceColor CurrentColorIndex;
        public int Turn;
        public int Round;
        public PieceKind StartPiece;
        public PlayerTeam StartTeam;

        public Board CurrentBoard;
        public PlayerTeam CurrentTeam;
        public PieceColor CurrentColor;

        public List<PieceKind> BlueShapes;
        public List<PieceKind> YellowShapes;
        public List<PieceKind> RedShapes;
        public List<PieceKind> GreenShapes;

        public List<PieceColor> OrderedColors;

        public string FirstPlayerName;
        public string SecondPlayerName;

        public Move LastMove;

        public State()
        {
            CurrentBoard = new Board();
        }

        /// <summary>
        /// Returns a new State which represents the board after doing the given Move
        /// <para>Caution: This method will check if the Move is legal before performing it which results 
        /// in worse performance. If you are using this method a lot I recommend using PerformWithoutChecks</para>
        /// </summary>
        public State Perform(Move M)
        {
            if (M.IsLegalOn(this))
                return PerformWithoutChecks(M);
            else
                throw new IllegalMoveException();
        }
        /// <summary>
        /// Returns a new State which represents the board after doing the given Move
        /// <para>Caution: This method won't check if the move is legal before commiting it to the board. 
        /// Feeding illegal moves into this method may result in unexpected behavior</para>
        /// </summary>
        public State PerformWithoutChecks(Move M)
        {
            State re = (State)this.Clone();

            if (M is SetMove)
            {
                var setMove = M as SetMove;
                foreach (var pos in setMove.GetAffectedPositions())
                    re.CurrentBoard.Fields[pos.X, pos.Y].Value = setMove.Color;
            }

            re.Turn++;
            re.CurrentTeam = re.CurrentTeam.OtherTeam();
            re.CurrentColor = re.CurrentColor.Next(OrderedColors);

            return re;
        }

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public object Clone()
        {
            State s = (State)MemberwiseClone();
            s.CurrentBoard = (Board)CurrentBoard.Clone();
            return s;
        }
    }
}
