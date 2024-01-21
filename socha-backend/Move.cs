using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace SochaClient.Backend
{
    /// <summary>
    /// This is a generic move class
    /// </summary>
    public class Move : ICloneable
    {
        readonly List<Action> actions;

        public Move(List<Action> actions)
        {
            this.actions = actions;
        }

        /// <summary>
        /// Checks if this move can be performed on game State S
        /// </summary>
        /// <param name="S"> The game State this move should be performed on </param> 
        public bool IsLegalOn(State S) // https://youtu.be/nz20lu2AM2k?t=8
        {
            // TODO: Check if amount of advances correct, check for coal, check 
            
            return true;
        }

        /// <summary>
        /// Converts this Move to XML
        /// <para>This is used to pack the Move into a format that can be send to the Server</para> 
        /// <para>You usually wont need this Method if you are programming your Client Logic</para> 
        /// </summary>
        public string ToXML() =>    $"<room roomId=\"{Starter.RoomID}\">\n" +
                                        $"<move>\n" +
                                            $"<actions>\n" +
                                                actions.Select(action => action.ToXML()).Combine() +
                                            $"</actions>\n" +
                                        $"</move>\n" +
                                    $"</room>";

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public object Clone() => MemberwiseClone();
    }

    // -- Action class structure

    public abstract class Action
    {
        abstract public bool IsLegalOn(State s);

        abstract public void PerformOn(State s);

        abstract public string ToXML();
    }

    public class Advance : Action
    {
        readonly int distance;

        public Advance(int distance)
        {
            this.distance = distance;
        }

        public override bool IsLegalOn(State s)
        {
            var pos = (CubeCoords)s.CurrentPlayer.Ship.Pos.Clone();
            for (int i = 0; i < distance; i++)
            {
                pos += CubeCoords.DirToOffset(s.CurrentPlayer.Ship.Dir);

                if (s.Board.GetField(pos) == null || s.Board.GetField(pos).FType != FieldType.water)
                    return false;
            }

            return true;
        }

        public override void PerformOn(State s)
        {
            s.CurrentPlayer.Ship.Pos += CubeCoords.DirToOffset(s.CurrentPlayer.Ship.Dir) * distance;
            s.CurrentPlayer.Ship.MovementPoints -= distance;
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

        public override bool IsLegalOn(State s)
        {
            var coalCost = Math.Max(0, Math.Abs(acc) - 1);
            if (coalCost > s.CurrentPlayer.Ship.Coal)
                return false;

            return true;
        }

        public override void PerformOn(State s)
        {
            var coalCost = Math.Max(0, Math.Abs(acc) - 1);
            s.CurrentPlayer.Ship.Coal -= coalCost;
            s.CurrentPlayer.Ship.Speed += acc;
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

        public override bool IsLegalOn(State s)
        {
            var dirDiff = s.CurrentPlayer.Ship.Dir.Difference(direction);
            if (dirDiff > s.CurrentPlayer.Ship.Coal + s.CurrentPlayer.Ship.FreeTurns) 
                return false;
            return true;
        }

        public override void PerformOn(State s)
        {
            s.CurrentPlayer.Ship.Dir = direction;

            var cost = s.CurrentPlayer.Ship.Dir.Difference(direction);
            if (s.CurrentPlayer.Ship.FreeTurns > 0)
            {
                var freeTurnsUsed = Math.Min(cost, s.CurrentPlayer.Ship.FreeTurns.Value);
                cost -= freeTurnsUsed;
                s.CurrentPlayer.Ship.FreeTurns -= freeTurnsUsed;
            }
            if (cost > 0)
                s.CurrentPlayer.Ship.Coal -= cost;
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

        public override bool IsLegalOn(State s)
        {
            // TODO: idk
            return true;
        }

        public override void PerformOn(State s)
        {
            s.GetOtherPlayer(s.CurrentPlayer).Ship.Pos += CubeCoords.DirToOffset(direction);
            // TODO: probably something else too
        }

        public override string ToXML() => $"<push direction=\"{direction}\" />\n";
    }
}
