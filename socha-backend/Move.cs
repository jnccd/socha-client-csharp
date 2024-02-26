using Socha.Backend.Action;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Action = Socha.Backend.Action.Action;

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
        public bool IsLegalOn(State s, Ship curShip = null, Ship otherShip = null) // https://youtu.be/nz20lu2AM2k?t=8
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));

            curShip ??= (Ship)s.CurrentPlayer.Ship.Clone();
            otherShip ??= (Ship)s.GetOtherPlayer(s.CurrentPlayer).Ship.Clone();

            // Perform and check actions
            foreach (var action in actions)
            {
                if (!action.IsLegalOn(s, curShip, otherShip))
                    return false;
                action.PerformOn(s, curShip, otherShip);
            }

            // Check final state
            if (curShip.Coal < 0 || curShip.MovementPoints != 0)
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

        public State PerformOn(State s, bool cloneState = true) => s.Perform(this, cloneState);

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
}
