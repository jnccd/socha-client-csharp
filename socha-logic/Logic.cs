using System;
using System.Linq;
using System.Runtime.InteropServices;
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

            var possibleMoves = GameState.GetPossibleMoves();

            var bestScore = -9999; Move bestMove = null;
            foreach (var possibleMove in possibleMoves.Take(200))
            {
                Console.WriteLine($"Handling move {possibleMove}");
                State futureState = null;
                try
                {
                    futureState = possibleMove.PerformOn(GameState);
                }
                catch
                {
                    continue;
                }

                var curScore = futureState.MyselfPlayer.Ship.Pos.q;
                if (curScore > bestScore)
                {
                    bestScore = curScore;
                    bestMove = possibleMove;
                }
            }

            if (bestMove == null)
                GetHashCode();

            return bestMove ?? possibleMoves.First();
        }
    }
}
