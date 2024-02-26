using SochaClient.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socha.Backend.Action
{
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
}
