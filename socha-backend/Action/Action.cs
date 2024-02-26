using SochaClient.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socha.Backend.Action
{
    public abstract class Action : ICloneable
    {
        abstract public bool IsLegalOn(State s, Ship curShip = null, Ship otherShip = null);

        abstract public void PerformOn(State s, Ship curShip = null, Ship otherShip = null, bool modifyState = false);

        abstract public string ToXML();

        public object Clone() => MemberwiseClone();
    }
}
