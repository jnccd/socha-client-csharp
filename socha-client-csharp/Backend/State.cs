using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SochaClient
{
    public class IllegalMoveException : Exception { }

    /// <summary>
    /// Contains all the information of the current GameState
    /// </summary>
    public class State : ICloneable
    {
        public int Turn;
        public PlayerTeam StartTeam;

        public Board Board;
        public Player PlayerOne, PlayerTwo;
        public Move LastMove;
        public Player CurrentPlayer { get => Turn % 2 == 0 ? PlayerOne : PlayerTwo; }

        public State()
        {
            Board = new Board();
            PlayerOne = new Player(PlayerTeam.ONE, 0);
            PlayerTwo = new Player(PlayerTeam.TWO, 0);
        }

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

            var targetField = Board.GetField(m.To);

            // TODO: Add logic

            // Update board fields
            re.Board.GetField(m.From).Piece = null;
            if (m.Piece != null)
                re.Board.GetField(m.To).Piece = m.Piece;

            re.Turn++;

            return re;
        }

        /// <summary>
        /// Gets all legal Moves for this state
        /// </summary>
        public Move[] GetAllPossibleMoves()
        {
            List<Move> re = new List<Move>();

            Field tmp;
            for (int x = 0; x < Board.Width; x++)
                for (int y = 0; y < Board.Height; y++)
                    if ((tmp = Board.GetField(x, y)) != null)
                        foreach (Point p in tmp.PossibleCoordsToMoveTo())
                            re.Add(new Move(tmp.Position(), p, tmp.Piece));

            var ree = re.Where(x => x.IsLegalOn(this)).ToArray();
            return ree;
        }

        /// <summary>
        /// Get a Player object from the PlayerTeam enum
        /// </summary>
        public Player GetPlayer(PlayerTeam team)
        {
            if (PlayerOne.Team == team)
                return PlayerOne;
            else
                return PlayerTwo;
        }

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public object Clone()
        {
            State s = (State)MemberwiseClone();

            s.Board = (Board)Board.Clone();

            s.PlayerOne = (Player)PlayerOne.Clone();
            s.PlayerTwo = (Player)PlayerTwo.Clone();

            if (LastMove != null)
                s.LastMove = (Move)LastMove.Clone();

            return s;
        }
    }
}
