using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareChallengeClient
{
    public class Logic
    {
        public PlayerColor MyColor;
        public State GameState;

        public Logic()
        {

        }

        public Move GetMove()
        {
            return GameState.BoardState.GetAllPossibleMoves(MyColor).First();
        }
    }
}
