using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SochaClient.Backend
{
    /// <summary>
    /// This is a generic move class
    /// </summary>
    public class Move : ICloneable
    {
        readonly public List<Action> actions;

        public Move(List<Action> actions)
        {
            this.actions = actions;
        }

        /// <summary>
        /// Checks if this move can be performed on game State S
        /// </summary>
        /// <param name="S"> The game State this move should be performed on </param> 
        public bool IsLegalOn(State s) // https://youtu.be/nz20lu2AM2k?t=8
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));

            // Perform and check actions
            var sc = (State)s.Clone();
            foreach (var action in actions)
            {
                if (!action.IsLegalOn(sc))
                    return false;
                action.PerformOn(sc);
            }

            // Check final state
            if (sc.CurrentPlayer.Ship.Coal < 0 || sc.CurrentPlayer.Ship.MovementPoints != 0)
                return false;
            // Check for correct Acceleration placement
            bool encoutneredNonAcc = false;
            foreach (var action in actions)
            {
                if (action is not Acceleration)
                    encoutneredNonAcc = true;
                if (encoutneredNonAcc && action is Acceleration)
                    return false;
            }

            return true;
        }

        public void PerformOn(State s) => s.Perform(this);

        /// <summary>
        /// Converts this Move to XML
        /// <para>This is used to pack the Move into a format that can be send to the Server</para> 
        /// <para>You usually wont need this Method if you are programming your Client Logic</para> 
        /// </summary>
        public string ToXML() =>    $"<room roomId=\"{Starter.RoomID}\">\n" +
                                        $"<data class=\"move\">\n" +
                                            $"<actions>\n" +
                                                actions.Select(action => action.ToXML()).Combine() +
                                            $"</actions>\n" +
                                        $"</data>\n" +
                                    $"</room>";

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public object Clone() => MemberwiseClone();
    }

    // -- Action class structure

    public abstract class Action : ICloneable
    {
        abstract public bool IsLegalOn(State s, Ship curShip = null, Ship otherShip = null);

        abstract public void PerformOn(State s, Ship curShip = null, Ship otherShip = null, bool modifyState = false);

        abstract public string ToXML();

        public object Clone() => MemberwiseClone();
    }

    public class Advance : Action
    {
        readonly int distance;

        public Advance(int distance)
        {
            this.distance = distance;
        }

        public override bool IsLegalOn(State s, Ship curShip = null, Ship otherShip = null)
        {
            curShip ??= s.CurrentPlayer.Ship;
            otherShip ??= s.GetOtherPlayer(s.CurrentPlayer).Ship;

            var pos = (CubeCoords)curShip.Pos.Clone();
            for (int i = 0; i < distance; i++)
            {
                pos += CubeCoords.DirToOffset(curShip.Dir);
                
                if (s.Board.GetField(pos) == null || 
                    s.Board.GetField(pos).FType != FieldType.water ||
                    otherShip.Pos == pos)
                    return false;
            }

            return true;
        }

        public override void PerformOn(State s, Ship curShip = null, Ship otherShip = null, bool modifyState = false)
        {
            curShip ??= s.CurrentPlayer.Ship;
            otherShip ??= s.GetOtherPlayer(s.CurrentPlayer).Ship;

            var shipLookVector = CubeCoords.DirToOffset(curShip.Dir);
            if (Enumerable.Range(1, distance).
                Select(x => curShip.Pos + shipLookVector * x).
                Any(x => s.Board.GetField(x).IsMidstream))
                curShip.MovementPoints -= 1;
            curShip.Pos += shipLookVector * distance;
            curShip.MovementPoints -= distance;
        }

        public override string ToXML() => $"<advance distance=\"{distance}\" />\n";
    }

    public class Acceleration : Action
    {
        readonly int acc;

        public Acceleration(int acc)
        {
            this.acc = acc;
        }

        public override bool IsLegalOn(State s, Ship curShip = null, Ship otherShip = null)
        {
            curShip ??= s.CurrentPlayer.Ship;
            otherShip ??= s.GetOtherPlayer(s.CurrentPlayer).Ship;

            var coalCost = Math.Max(0, Math.Abs(acc) - 1);
            if (coalCost > curShip.Coal)
                return false;

            return true;
        }

        public override void PerformOn(State s, Ship curShip = null, Ship otherShip = null, bool modifyState = false)
        {
            curShip ??= s.CurrentPlayer.Ship;
            otherShip ??= s.GetOtherPlayer(s.CurrentPlayer).Ship;

            var coalCost = Math.Max(0, Math.Abs(acc) - 1);
            curShip.Coal -= coalCost;
            curShip.Speed += acc;
            curShip.MovementPoints = curShip.Speed;
            curShip.FreeTurns = 1;
        }

        public override string ToXML() => $"<acceleration acc=\"{acc}\" />\n";
    }

    public class Turn : Action
    {
        readonly Direction direction;

        public Turn(Direction direction)
        {
            this.direction = direction;
        }

        public override bool IsLegalOn(State s, Ship curShip = null, Ship otherShip = null)
        {
            curShip ??= s.CurrentPlayer.Ship;
            otherShip ??= s.GetOtherPlayer(s.CurrentPlayer).Ship;

            var dirDiff = curShip.Dir.Difference(direction);
            if (dirDiff > curShip.Coal + curShip.FreeTurns) 
                return false;
            return true;
        }

        public override void PerformOn(State s, Ship curShip = null, Ship otherShip = null, bool modifyState = false)
        {
            curShip ??= s.CurrentPlayer.Ship;
            otherShip ??= s.GetOtherPlayer(s.CurrentPlayer).Ship;

            curShip.Dir = direction;

            var cost = curShip.Dir.Difference(direction);
            if (curShip.FreeTurns > 0)
            {
                var freeTurnsUsed = Math.Min(cost, curShip.FreeTurns.Value);
                cost -= freeTurnsUsed;
                curShip.FreeTurns -= freeTurnsUsed;
            }
            if (cost > 0)
                curShip.Coal -= cost;
        }

        public override string ToXML() => $"<turn direction=\"{direction}\" />\n";
    }

    public class Push : Action
    {
        readonly Direction direction;

        public Push(Direction direction)
        {
            this.direction = direction;
        }

        public override bool IsLegalOn(State s, Ship curShip = null, Ship otherShip = null)
        {
            curShip ??= s.CurrentPlayer.Ship;
            otherShip ??= s.GetOtherPlayer(s.CurrentPlayer).Ship;

            // TODO: idk
            return true;
        }

        public override void PerformOn(State s, Ship curShip = null, Ship otherShip = null, bool modifyState = false)
        {
            curShip ??= s.CurrentPlayer.Ship;
            otherShip ??= s.GetOtherPlayer(s.CurrentPlayer).Ship;

            otherShip.Pos += CubeCoords.DirToOffset(direction);
            // TODO: probably something else too
        }

        public override string ToXML() => $"<push direction=\"{direction}\" />\n";
    }
}
