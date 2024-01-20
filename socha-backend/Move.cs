using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace SochaClient.Backend
{
    /// <summary>
    /// This is a generic move class
    /// </summary>
    public class Move : ICloneable
    {
        public abstract class Action
        {
            abstract public string ToXML();
        }

        public class Acceleration : Action
        {
            int acc;

            public Acceleration(int acc)
            {
                this.acc = acc;
            }

            public override string ToXML() => $"<acceleration acc=\"{acc}\" />";
        }

        public class Advance : Action
        {
            int distance;

            public Advance(int distance)
            {
                this.distance = distance;
            }

            public override string ToXML() => $"<advance distance=\"{distance}\" />";
        }

        public class Push : Action
        {
            Direction direction;

            public Push(Direction direction)
            {
                this.direction = direction;
            }

            public override string ToXML() => $"<push direction=\"{direction}\" />";
        }

        public class Turn : Action
        {
            Direction direction;

            public Turn(Direction direction)
            {
                this.direction = direction;
            }

            public override string ToXML() => $"<turn direction=\"{direction}\" />";
        }

        List<Action> actions;

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
            
            
            return true;
        }

        /// <summary>
        /// Converts this Move to XML
        /// <para>This is used to pack the Move into a format that can be send to the Server</para> 
        /// <para>You usually wont need this Method if you are programming your Client Logic</para> 
        /// </summary>
        public string ToXML()
        {
            if (From != null)
                return  $"<room roomId=\"{Starter.RoomID}\">\n" +
                            $"<data class=\"move\">\n" +
                                $"<from x=\"{HexFrom.X}\" y=\"{HexFrom.Y}\"/>" +
                                $"<to x = \"{HexTo.X}\" y=\"{HexTo.Y}\"/>" +
                            $"</data>\n" +
                        $"</room>";
            else
                return $"<room roomId=\"{Starter.RoomID}\">\n" +
                            $"<data class=\"move\">\n" +
                                $"<to x = \"{HexTo.X}\" y=\"{HexTo.Y}\"/>" +
                            $"</data>\n" +
                        $"</room>";
        }

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public object Clone() => MemberwiseClone();
    }
}
