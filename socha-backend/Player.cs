using System;
using System.Collections.Generic;
using System.Text;

namespace SochaClient.Backend
{
    public class Player : ICloneable
    {
        public PlayerTeam Team { get; private set; }
        public Ship Ship;

        public Player(PlayerTeam team, Ship ship)
        {
            Team = team;
            Ship = ship;
        }

        public object Clone()
        {
            Player re = (Player)MemberwiseClone();
            re.Ship = (Ship)Ship.Clone();
            return re;
        }
    }

    public class Ship : ICloneable
    {
        public int Coal;
        public Direction Dir;
        public int Passengers;
        public int Points;
        public int Speed;
        public CubeCoords Pos;

        public int? MovementPoints = null;
        public int? FreeTurns = null;

        public Ship(int coal = 6, Direction dir = Direction.RIGHT, int passengers = 0, int points = 0, int speed = 1, CubeCoords pos = null)
        {
            Coal = coal;
            Dir = dir;
            Passengers = passengers;
            Points = points;
            Speed = speed;
            Pos = pos;
        }

        public object Clone()
        {
            Ship re = (Ship)MemberwiseClone();
            re.Pos = (CubeCoords)Pos.Clone();
            return re;
        }
    }
}
