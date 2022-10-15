using System.Collections.Generic;
using System.Drawing;

namespace SochaClient.Backend
{
    public static class Extensions
    {
        public static PlayerTeam OtherTeam(this PlayerTeam t) => t == PlayerTeam.ONE ? PlayerTeam.TWO : PlayerTeam.ONE;
    }
}
