using SochaClient.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socha.Backend.Action
{
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
}
