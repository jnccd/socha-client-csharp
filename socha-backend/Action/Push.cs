using SochaClient.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socha.Backend.Action
{
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

            return curShip.Pos == otherShip.Pos &&
                   s.Board.GetField(otherShip.Pos + CubeCoords.DirToOffset(direction))?.FType == FieldType.water;
        }

        public override void PerformOn(State s, Ship curShip = null, Ship otherShip = null, bool modifyState = false)
        {
            curShip ??= s.CurrentPlayer.Ship;
            otherShip ??= s.GetOtherPlayer(s.CurrentPlayer).Ship;

            otherShip.Pos += CubeCoords.DirToOffset(direction);
            curShip.MovementPoints -= 1;
        }

        public override string ToXML() => $"<push direction=\"{direction}\" />\n";
    }
}
