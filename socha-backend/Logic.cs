using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SochaClient.Backend
{
    public abstract class Logic
    {
        public PlayerTeam MyTeam;
        public State GameState;

        public abstract Move GetMove();
    }
}
