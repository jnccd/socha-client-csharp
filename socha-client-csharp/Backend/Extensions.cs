using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socha_client_csharp
{
    public static class Extensions
    {
        public static PlayerTeam OtherTeam(this PlayerTeam t) => t == PlayerTeam.ONE ? PlayerTeam.TWO : PlayerTeam.ONE;

        public static PieceColor Next(this PieceColor c, List<PieceColor> OrderedColors) => OrderedColors[(OrderedColors.IndexOf(c) + 1) % OrderedColors.Count];

        public static Color ToColor(this PieceColor? c) 
        {
            if (c.HasValue)
                if (c.Value == PieceColor.BLUE)
                    return Color.Blue;
                else if (c.Value == PieceColor.GREEN)
                    return Color.Green;
                else if (c.Value == PieceColor.RED)
                    return Color.Red;
                else
                    return Color.Yellow;
            else
                return Color.Black;
        }
    }
}
