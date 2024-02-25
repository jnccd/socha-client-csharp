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

            // Perform actions
            foreach (var action in m.actions)
                action.PerformOn(this, modifyState: true);

            // Get passengers
            if (CurrentPlayer.Ship.Speed == 1 || 
                (Board.GetField(CurrentPlayer.Ship.Pos).IsMidstream && CurrentPlayer.Ship.Speed == 2))
                for (int i = 0; i < Enum.GetNames(typeof(Direction)).Length; i++)
                {
                    var passengerCheckPos = CurrentPlayer.Ship.Pos + CubeCoords.DirToOffset((Direction)i);
                    var pasengerCheckField = Board.GetField(passengerCheckPos);
                    if (pasengerCheckField.FType == FieldType.passenger && 
                        pasengerCheckField.Passengers > 0 && 
                        pasengerCheckField.Dir == ((Direction)i).Opposite())
                        Board.SetField(passengerCheckPos, new Field(FieldType.island, false, passengerCheckPos, Board));
                }

            // Update current player
            CurrentPlayer = GetOtherPlayer(CurrentPlayer);

            re.Turn++;

            return re;
        }

        /// <summary>
        /// Gets all relevant legal Moves for this State
        /// </summary>
        public List<Move> GetPossibleMoves() => GeneratePossibleAccelerations();
        private List<Move> GeneratePossibleAccelerations(Ship curShip = null, Ship otherShip = null)
        {
            curShip ??= (Ship)CurrentPlayer.Ship.Clone();
            otherShip ??= (Ship)this.GetOtherPlayer(CurrentPlayer).Ship.Clone();

            List <Move> re = new();

            // Add accel action
            for (int i = 1; i < 6 + 1; i++)
            {
                int acc = i - curShip.Speed;
                var newAction = new Acceleration(acc);

                if (newAction.IsLegalOn(this))
                {
                    var newCurShip = (Ship)curShip.Clone();
                    var newOtherShip = (Ship)otherShip.Clone();
                    newAction.PerformOn(this, newCurShip, newOtherShip);

                    // You're on a path in the woods, and at the end of that path is a cabin...
                    if (acc == 0)
                        re.AddRange(GeneratePossibleMoveEndings(new List<Action> { },           this, newCurShip, newOtherShip));
                    else
                        re.AddRange(GeneratePossibleMoveEndings(new List<Action> { newAction }, this, newCurShip, newOtherShip));
                }
            }

            return re;
        }
        private List<Move> GeneratePossibleMoveEndings(List<Action> pastActions, State curState, Ship curShip, Ship otherShip)
        {
            // Check for illegal state early
            if (curShip.Coal < 0 || 
                curShip.MovementPoints < 0 || 
                Board.GetField(curShip.Pos) == null ||
                (curShip.Pos == otherShip.Pos && curShip.MovementPoints <= 0))
                return new List<Move>();

            // Return early if done
            if (curShip.MovementPoints == 0)
                return new List<Move> { new(pastActions) };

            List<Move> re = new();

            if (otherShip.Pos == curShip.Pos)
            {
                // Add push action
                for (int i = 0; i < Enum.GetNames(typeof(Direction)).Length; i++)
                {
                    if ((Direction)i == curShip.Dir.Opposite())
                        continue;

                    var newAction = new Push((Direction)i);

                    if (newAction.IsLegalOn(this, curShip, otherShip))
                    {
                        var newCurShip = (Ship)curShip.Clone();
                        var newOtherShip = (Ship)otherShip.Clone();
                        var newActions = pastActions.Clone();
                        newActions.Add(newAction);
                        newAction.PerformOn(this, newCurShip, newOtherShip);

                        // You're on a path in the woods, and at the end of that path is a cabin...
                        re.AddRange(GeneratePossibleMoveEndings(newActions, curState, newCurShip, newOtherShip));
                    }
                }
            }
            else
            {
                // Add advance action
                if (curShip.MovementPoints > 0 && (!pastActions.Any() || pastActions.Last() is not Advance))
                {
                    for (int i = 1; i < curShip.MovementPoints + 1; i++)
                    {
                        var newAction = new Advance(i);

                        if (newAction.IsLegalOn(this, curShip, otherShip))
                        {
                            var newCurShip = (Ship)curShip.Clone();
                            var newOtherShip = (Ship)otherShip.Clone();
                            var newActions = pastActions.Clone();
                            newActions.Add(newAction);
                            newAction.PerformOn(curState, newCurShip, newOtherShip, false);

                            // You're on a path in the woods, and at the end of that path is a cabin...
                            re.AddRange(GeneratePossibleMoveEndings(newActions, curState, newCurShip, newOtherShip));
                        }
                    }
                }

                // Add turn action
                if ((curShip.Coal > 0 || curShip.FreeTurns > 0) && (!pastActions.Any() || pastActions.Last() is not Backend.Turn))
                {
                    for (int i = 0; i < Enum.GetNames(typeof(Direction)).Length; i++)
                    {
                        var newAction = new Backend.Turn((Direction)i);

                        if (newAction.IsLegalOn(this, curShip, otherShip))
                        {
                            var newCurShip = (Ship)curShip.Clone();
                            var newOtherShip = (Ship)otherShip.Clone();
                            var newActions = pastActions.Clone();
                            newActions.Add(newAction);
                            newAction.PerformOn(this, newCurShip, newOtherShip);

                            // You're on a path in the woods, and at the end of that path is a cabin...
                            re.AddRange(GeneratePossibleMoveEndings(newActions, curState, newCurShip, newOtherShip));
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
