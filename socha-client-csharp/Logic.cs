using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socha_client_csharp
{
    public class Logic
    {
        public PlayerTeam MyTeam;
        public State GameState;

        public Logic()
        {
            // TODO: Add init logic
        }

        public SetMove GetMove()
        {
            // TODO: Add your game logic

            return GameState.CurrentBoard.GetAllPossibleMoves(MyTeam).First();
        }
    }
}
