﻿using System;
using System.Collections.Generic;
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

        public State()
        {
            Board = new Board();
            PlayerOne = new Player("ONE", PlayerTeam.ONE, 0);
            PlayerTwo = new Player("TWO", PlayerTeam.TWO, 0);
        }

        public Player CurrentPlayer() => Turn % 2 == 0 ? PlayerOne : PlayerTwo;

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

            if (!targetField.Empty())
            {
                if (m.Piece != null)
                    m.Piece.Height++;

                if (m.Piece.Height >= 3)
                {
                    re.
                }
            }

            re.Turn++;
            re.CurrentTeam = re.CurrentTeam.OtherTeam();

            return re;
        }

        /// <summary>
        /// Gets all legal Moves for the current state
        /// </summary>
        public Move[] GetAllPossibleMoves()
        {
            List<Move> re = new List<Move>();

            for (int x = 0; x < Board.Width; x++)
                for (int y = 0; y < Board.Height; y++)
                    foreach (PieceKind k in Enum.GetValues(typeof(PieceKind)))
                        foreach (Rotation r in Enum.GetValues(typeof(Rotation)))
                        {
                            re.Add(new SetMove(CurrentColor, k, r, false, x, y));
                            re.Add(new SetMove(CurrentColor, k, r, true, x, y));
                        }
            re.Add(new SkipMove());

            var ree = re.Where(x => x.IsLegalOn(this)).ToArray();
            return ree;
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

            s.LastMove = (Move)LastMove.Clone();

            return s;
        }
    }
}
