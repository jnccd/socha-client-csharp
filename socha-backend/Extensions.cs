using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SochaClient.Backend
{
    public static class Extensions
    {
        public static PlayerTeam OtherTeam(this PlayerTeam t) => t == PlayerTeam.ONE ? PlayerTeam.TWO : PlayerTeam.ONE;
        public static Direction Step(this Direction d, int stepWidth) => (Direction)(((int)d + stepWidth) % Enum.GetNames(typeof(Direction)).Length);
        public static int Difference(this Direction d, Direction e) => Math.Min(Math.Abs((int)d - 6) - (int)e,
                                                                       Math.Min(Math.Abs((int)d)     - (int)e,
                                                                                Math.Abs((int)d + 6) - (int)e));
        public static List<Action> Clone(this List<Action> actions)
        {
            var re = new List<Action>();
            foreach (var a in actions)
                re.Add((Action)a.Clone());
            return re;
        }

        public static string Combine(this IEnumerable<string> s, string combinator = "")
        {
            return s.Count() == 0 ? "" : s.Foldl("", (x, y) => x + combinator + y).Remove(0, combinator.Length);
        }
        public static b Foldl<a, b>(this IEnumerable<a> xs, b y, Func<b, a, b> f)
        {
            foreach (a x in xs)
                y = f(y, x);
            return y;
        }
        public static b Foldl<a, b>(this IEnumerable<a> xs, Func<b, a, b> f)
        {
            return xs.Foldl(default, f);
        }
    }
}
