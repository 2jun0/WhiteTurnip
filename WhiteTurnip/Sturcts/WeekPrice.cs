using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteTurnip
{
    class WeekPrice
    {
        public static int PATTERN_FLUCTUATING = 1;
        public static int PATTERN_HIGH_SPIKE = 2;
        public static int PATTERN_DECREASING = 3;
        public static int PATTERN_LOW_SPIKE = 4;

        public int pattern;

        public int[] prices; // size: 14

        public WeekPrice(int pattern)
        {
            this.pattern = pattern;
            this.prices = new int[14];
            this.prices[0] = 500;
            this.prices[1] = 500;
        }

    }
}
