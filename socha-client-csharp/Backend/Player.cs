using System;
using System.Collections.Generic;
using System.Text;

namespace SochaClient
{
    public class Player : ICloneable
    {
        public string Name { get; private set; }
        public PlayerTeam Team { get; private set; }
        public int Amber { get; private set; }

        public Player(string name, PlayerTeam team, int amber)
        {
            Name = name;
            Team = team;
            Amber = amber;
        }

        public object Clone() => MemberwiseClone();
    }
}
