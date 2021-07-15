using SochaClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace SochaClient
{
    public class Piece
    {
        public PieceColor Color { get; private set; }
        public PieceType Kind { get; private set; }
        public int Height { get; internal set; }

        public Piece(PieceColor color, PieceType kind, int height)
        {
            Color = color;
            Kind = kind;
            Height = height;
        }
    }
}
