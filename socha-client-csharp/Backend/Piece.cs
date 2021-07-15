using SochaClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SochaClient
{
    public class Piece
    {
        public PieceColor PColor { get; private set; }
        public PieceType Type { get; private set; }
        public int Height { get; internal set; }

        public Piece(PieceColor color, PieceType kind, int height)
        {
            PColor = color;
            Type = kind;
            Height = height;
        }

        public Color ToColor()
        {
            if (PColor == PieceColor.RED)
            {
                switch (Type)
                {
                    case PieceType.Herzmuschel:
                        return Color.DarkOrange;
                    case PieceType.Moewe:
                        return Color.Yellow;
                    case PieceType.Robbe:
                        return Color.Red;
                    case PieceType.Seestern:
                        return Color.Crimson;
                }
            }
            else
            {
                switch (Type)
                {
                    case PieceType.Herzmuschel:
                        return Color.DodgerBlue;
                    case PieceType.Moewe:
                        return Color.SkyBlue;
                    case PieceType.Robbe:
                        return Color.CornflowerBlue;
                    case PieceType.Seestern:
                        return Color.Turquoise;
                }
            }

            return Color.Black;
        }
    }
}
