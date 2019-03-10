using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareChallengeClient
{
    /// <summary>
    /// Contains all the information of the current GameState
    /// </summary>
    public class State
    {
        public PlayerColor StartPlayerColor;
        public PlayerColor CurrentPlayerColor;
        public int Turn;
        public string RedDisplayName;
        public string BlueDisplayName;

        public Board BoardState;

        public State()
        {
            BoardState = new Board();
        }
    }

    public enum PlayerColor { RED, BLUE }
}
