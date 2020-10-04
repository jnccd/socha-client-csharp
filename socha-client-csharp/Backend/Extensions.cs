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
    }
}
