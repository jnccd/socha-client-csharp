using SochaClient.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socha.Backend.Action
{
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
                    s.Board.GetField(pos).FType != FieldType.water)
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
}
