using System.Linq;
using SochaClient.Backend;

namespace SochaClient
{
    public class Program
    {
        static void Main(string[] args) => Starter.Main(args, new Logic());
    }

    public class Logic : Backend.Logic
    {
        public Logic()
        {
            // TODO: Add init logic
        }

        public override Move GetMove()
        {
            // TODO: Add your game logic
            
            return GameState.GetAllPossibleMoves().First();
        }
    }
}
