using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cother
{
    public static class R
    {
        private static Random rgen = new Random();
        public static int Next(int max)
        {
            return rgen.Next(max);
        }
        public static int Next(int min, int max)
        {
            return rgen.Next(min, max);
        }

        public static float NextFloat()
        {
            return (float)rgen.NextDouble();
        }

        public static bool Coin()
        {
            return rgen.Next(0, 2) == 0;
        }
    }
}
