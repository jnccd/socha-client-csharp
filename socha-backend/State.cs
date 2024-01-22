using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public State Perform(Move m, bool cloneState = true)
        {
            if (m.IsLegalOn(this))
                return PerformWithoutChecks(m, cloneState);
            else
                throw new IllegalMoveException();
        }
        /// <summary>
        /// Returns a new State which represents the board after doing the given Move
        /// <para>Caution: This method won't check if the move is legal before commiting it to the board. 
        /// Feeding illegal moves into this method may result in unexpected behavior.
        /// However, it should run faster that Perform()</para>
        /// </summary>
        public State PerformWithoutChecks(Move m, bool cloneState = true)
        {
            State re = cloneState ? (State)Clone() : this;

            foreach (var action in m.actions)
                action.PerformOn(this);

            // Update current player
            CurrentPlayer = GetOtherPlayer(CurrentPlayer);

            re.Turn++;

            return re;
        }

        /// <summary>
        /// Gets all relevant legal Moves for this State
        /// </summary>
        public List<Move> GetPossibleMoves() => GeneratePossibleMoves();
        private List<Move> GeneratePossibleMoves(List<Action> pastActions = null, Ship curShip = null, Ship otherShip = null)
        {
            curShip ??= (Ship)CurrentPlayer.Ship.Clone();
            otherShip ??= (Ship)this.GetOtherPlayer(CurrentPlayer).Ship.Clone();

            List <Move> re = new();

            if (pastActions == null)
            {
                // Add accel action
                for (int i = 1; i < 6+1; i++)
                {
                    int acc = i - curShip.Speed;
                    var newAction = new Acceleration(acc);
                    
                    if (newAction.IsLegalOn(this))
                    {
                        var newCurShip = (Ship)curShip.Clone();
                        var newOtherShip = (Ship)otherShip.Clone();
                        newAction.PerformOn(this, newCurShip, newOtherShip);

                        // You're on a path in the woods, and at the end of that path is a cabin...
                        re.AddRange(GeneratePossibleMoves(new List<Action> { newAction }, newCurShip, newOtherShip));
                    }
                }
            }
            else
            {
                // Check for illegal state early
                if (curShip.Coal < 0 || curShip.MovementPoints < 0 || Board.GetField(curShip.Pos) == null)
                    return new List<Move>();

                // Return early if done
                if (curShip.MovementPoints == 0)
                    return new List<Move> { new Move(pastActions) };

                
                if (curShip.MovementPoints > 0 && pastActions.Last() is not Advance)
                {
                    // Add advance action
                    for (int i = 1; i < curShip.MovementPoints + 1; i++)
                    {
                        var newAction = new Advance(i);

                        if (newAction.IsLegalOn(this))
                        {
                            var newCurShip = (Ship)curShip.Clone();
                            var newOtherShip = (Ship)otherShip.Clone();
                            newAction.PerformOn(this, newCurShip, newOtherShip);

                            // You're on a path in the woods, and at the end of that path is a cabin...
                            re.AddRange(GeneratePossibleMoves(new List<Action> { newAction }, newCurShip, newOtherShip));
                        }
                    }
                }

                if ((curShip.Coal > 0 || curShip.FreeTurns > 0) && pastActions.Last() is not Backend.Turn)
                {
                    // Add turn action
                    for (int i = 0; i < Enum.GetNames(typeof(Direction)).Length; i++)
                    {
                        var newAction = new Backend.Turn((Direction)i);

                        if (newAction.IsLegalOn(this))
                        {
                            var newCurShip = (Ship)curShip.Clone();
                            var newOtherShip = (Ship)otherShip.Clone();
                            newAction.PerformOn(this, newCurShip, newOtherShip);

                            // You're on a path in the woods, and at the end of that path is a cabin...
                            re.AddRange(GeneratePossibleMoves(new List<Action> { newAction }, newCurShip, newOtherShip));
                        }
                    }
                }
            }

            return re;
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
