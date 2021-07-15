using System;
using System.Collections.Generic;
using System.Text;

namespace SochaClient
{
    public class Player : ICloneable
    {
        public string Name { get; private set; }
        public PlayerTeam Team { get; private set; }
        public int Amber;
        public PieceColor Color { get => Team.ToColor(); }

        public Player(string name, PlayerTeam team, int amber)
        {
            Name = name;
            Team = team;
            Amber = amber;
        }

        public object Clone() => MemberwiseClone();
    }
}
