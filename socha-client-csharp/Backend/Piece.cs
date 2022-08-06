using SochaClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SochaClient
{
    public class Piece : ICloneable
    {
        public PlayerTeam Team { get; private set; }

        public Piece(PlayerTeam team)
        {
            Team = team;
        }

        public Color ToColor()
        {
            if (Team == PlayerTeam.ONE)
                return Color.Crimson;
            else
                return Color.SkyBlue;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
