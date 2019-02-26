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
        string Strategy { get { return Program.Strategy; } }

        public Logic()
        {
            Program.LogNetwork = true;
        }

        public Move GetMove()
        {
            return GameState.BoardState.GetAllPossibleMoves(MyColor).First();
        }
    }
}
