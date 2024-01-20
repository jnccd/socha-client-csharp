using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace SochaClient.Backend
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
        public Player CurrentPlayer, MyselfPlayer;

        public State()
        {
            Board = new Board();
            PlayerOne = new Player(PlayerTeam.ONE, new Ship());
            PlayerTwo = new Player(PlayerTeam.TWO, new Ship());
        }

        /// <summary>
        /// Returns a new State which represents the board after doing the given Move
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
        /// Feeding illegal moves into this method may result in unexpected behavior.
        /// However, it should run faster that Perform()</para>
        /// </summary>
        public State PerformWithoutChecks(Move m)
        {
            State re = (State)Clone();

            var startField = Board.GetField(m.From);
            var targetField = Board.GetField(m.To);

            CurrentPlayer.Fishes += startField.fishes;
            startField.fishes = 0;

            // Update board fields
            targetField.Piece = startField.Piece;
            startField.Piece = null;

            // Update current player
            var otherPlayer = GetOtherPlayer(CurrentPlayer);
            if (CanMove(otherPlayer)) 
                CurrentPlayer = otherPlayer;

            re.Turn++;

            return re;
        }

        public void PerformAction(Move.Action a, State s)
        {
            if (a is Move.Advance)
            {
                s.CurrentPlayer.Ship.Pos.
            }
            else if (a is Move.Acceleration)
            {

            }
            else if (a is Move.Push)
            {

            }
            else if (a is Move.Turn)
            {

            }
        }

        /// <summary>
        /// Gets all legal Moves for this state
        /// </summary>
        public Move[] GetAllPossibleMoves()
        {
            var re = new List<Move>();

            if (Turn < 8)
            {
                // Initial moves
                for (int x = 0; x < Board.Width; x++)
                    for (int y = 0; y < Board.Height; y++)
                        if (Board.GetField(x, y).fishes == 1)
                            re.Add(new Move(null, new(x, y)));

                Debug.WriteLine($"Found {re} possible place moves");
            }
            else
            {
                // Normal moves
                for (int x = 0; x < Board.Width; x++)
                    for (int y = 0; y < Board.Height; y++)
                        if (Board.GetField(x, y).Piece?.Team == CurrentPlayer.Team)
                            foreach (Point p in Board.GetField(x, y).PossibleCoordsToMoveTo(this))
                                re.Add(new Move(new Point(x, y), p));
            }

            var ree = re.Where(x => x.IsLegalOn(this)).ToArray();
            return ree;
        }

        /// <summary>
        /// Checks whether the given player can move
        /// </summary>
        public bool CanMove(Player player)
        {
            var can = false;

            foreach (var f in Board.GetFieldsOfPlayer(player.Team))
            {
                var n = Board.GetNeighborFields(f);
                can &= n.Any(x => x.Free());
            }

            return can;
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
        /// Get the other Player from a player
        /// </summary>
        public Player GetOtherPlayer(Player player) => GetPlayer(player.Team.OtherTeam());

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
