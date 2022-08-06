using System;
using System.Collections.Generic;
using System.Text;

namespace SochaClient
{
    public class Player : ICloneable
    {
        public PlayerTeam Team { get; private set; }
        public int Fishes;

        public Player(PlayerTeam team, int fishes)
        {
            Team = team;
            Fishes = fishes;
        }

        public object Clone() => MemberwiseClone();
    }
}
