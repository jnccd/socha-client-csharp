using SochaClient;
using System.Linq;

namespace SochaClientLogic
{
    public class Logic
    {
        public PlayerTeam MyTeam;
        public State GameState;

        public Logic()
        {
            // TODO: Add init logic
        }

        public Move GetMove()
        {
            // TODO: Add your game logic
            
            return GameState.GetAllPossibleMoves().First();
        }
    }
}
