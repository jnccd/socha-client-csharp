using SochaClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace SochaClient
{
    public class Piece
    {
        public PieceColor Color { get; private set; }
        public PieceKind Kind { get; private set; }
        public int Height { get; internal set; }

        public Piece(PieceColor color, PieceKind kind, int height)
        {
            Color = color;
            Kind = kind;
            Height = height;
        }
    }
}
