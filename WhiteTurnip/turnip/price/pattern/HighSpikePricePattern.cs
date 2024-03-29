﻿using System;
using WhiteTurnip.common.utils;

namespace WhiteTurnip.turnip.price.pattern
{
    class HighSpikePricePattern : IPricePattern
    {
        private readonly Randoms randoms;
        private readonly int[] priceMaxDiffByWeekOfDay = { 0, -1, -1, -1, 10, 350, 1 };
        private readonly int[] priceMinDiffByWeekOfDay = { 0, -10, -10, -10, 1, 50, -10 };

        public HighSpikePricePattern(Randoms randoms)
        {
            this.randoms = randoms;
        }

        public int[,] Prices()
        {

            int[,] prices = new int[7, 2];
            prices[0, 0] = TurnipContext.TURNIP_BUY_PRICE;
            prices[0, 1] = TurnipContext.TURNIP_BUY_PRICE;

            int prevPrice = prices[0, 1];
            for (var d = 1; d < 7; d++)
            {
                for (var af = 0; af <= 1; af++)
                {
                    int diff = GetNextDiff(d);
                    prices[d, af] = Math.Max(0, prevPrice + diff);
                    prevPrice = prices[d, af];
                }
            }

            return prices;
        }

        private int GetNextDiff(int dayOfWeek)
        {
            return randoms.GetInt(priceMinDiffByWeekOfDay[dayOfWeek],
                priceMaxDiffByWeekOfDay[dayOfWeek]);
        }
    }
}
