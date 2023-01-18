using System;
using WhiteTurnip.common.utils;

namespace WhiteTurnip.turnip.price.pattern
{
    class FluctuatingPricePattern : IPricePattern
    {
        private readonly Randoms randoms;
        private readonly int priceMaxDiff = 10;
        private readonly int priceMinDiff = -10;

        public FluctuatingPricePattern(Randoms randoms)
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
                    int diff = GetNextDiff();
                    prices[d, af] = Math.Max(0, prevPrice + diff);
                    prevPrice = prices[d, af];
                }
            }

            return prices;
        }

        private int GetNextDiff()
        {
            return randoms.GetInt(priceMinDiff, priceMaxDiff);
        }
    }
}
