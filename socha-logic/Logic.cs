using System.Linq;
using SochaClient.Backend;

namespace SochaClient
{
    public class Logic : Backend.Logic
    {
        static void Main(string[] args) => Starter.Main(args, new Logic());

        public Logic()
        {
            // TODO: Add init logic
        }

        public override Move GetMove()
        {
            // TODO: Add your game logic
            
            return GameState.GetPossibleMoves().First();
        }
    }
}
