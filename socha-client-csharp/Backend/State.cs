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
        public int CurrentColorIndex;
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

        public bool IsStartTurn() => Turn <= 1;

        /// <summary>
        /// Returns a new State which represents the board after doing the given Move
        /// <para>Caution: This method will check if the Move is legal before performing it which results 
        /// in worse performance. If you are using this method a lot I recommend using PerformWithoutChecks</para>
        /// </summary>
        public State Perform(Move m)
        {
            if (m.IsLegalOn(this))
                return PerformWithoutChecks(m);
            else
                throw new IllegalMoveException();
        }
        /// <summary>
        /// Returns a new State which represents the board after doing the given Move
        /// <para>Caution: This method won't check if the move is legal before commiting it to the board. 
        /// Feeding illegal moves into this method may result in unexpected behavior</para>
        /// </summary>
        public State PerformWithoutChecks(Move m)
        {
            State re = (State)Clone();

            if (m is SetMove)
            {
                var setMove = m as SetMove;
                foreach (var p in setMove.AffectedPositions)
                    re.CurrentBoard.GetField(p).color = setMove.Color;
            }

            re.Turn++;
            re.CurrentTeam = re.CurrentTeam.OtherTeam();
            re.CurrentColor = re.CurrentColor.Next(OrderedColors);

            return re;
        }

        /// <summary>
        /// Gets all legal Moves for the current state
        /// </summary>
        public Move[] GetAllPossibleMoves()
        {
            List<Move> re = new List<Move>();

            re.Add(new SkipMove());
            for (int x = 0; x < Board.BoardWidth; x++)
                for (int y = 0; y < Board.BoardHeight; y++)
                    foreach (PieceKind k in Enum.GetValues(typeof(PieceKind)))
                        foreach (Rotation r in Enum.GetValues(typeof(Rotation)))
                        {
                            re.Add(new SetMove(CurrentColor, k, r, true, x, y));
                            re.Add(new SetMove(CurrentColor, k, r, false, x, y));
                        }

            return re.Where(x => x.IsLegalOn(this)).ToArray();
        }

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public object Clone()
        {
            State s = (State)MemberwiseClone();
            s.CurrentBoard = (Board)CurrentBoard.Clone();

            // Using ToList() for shallow list copies
            s.BlueShapes = BlueShapes.ToList();
            s.YellowShapes = YellowShapes.ToList();
            s.RedShapes = RedShapes.ToList();
            s.GreenShapes = GreenShapes.ToList();

            s.OrderedColors = OrderedColors.ToList();

            s.LastMove = (Move)LastMove.Clone();

            return s;
        }
    }
}
