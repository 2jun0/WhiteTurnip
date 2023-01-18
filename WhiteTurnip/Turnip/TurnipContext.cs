using System;
using WhiteTurnip.utils;
using WhiteTurnip.weekPrice.pattern;
using WhiteTurnip.weekPrice;

namespace WhiteTurnip.Turnip
{
    class TurnipContext
    {
        public const int TURNIP_BUY_PRICE = 100;

        public WeekPriceFactory WeekPriceFactory()
        {
            return new WeekPriceFactory(randoms(),
                PricePatternProbabilities(),
                PricePatternProviders());
        }

        private int[] PricePatternProbabilities()
        {
            return new int[] { 1, 2, 2, 1 };
        }

        private Func<PricePattern>[] PricePatternProviders()
        {
            return new Func<PricePattern>[]{
                () => new DecreasingPricePattern(randoms()),
                () => new FluctuatingPricePattern(randoms()),
                () => new HighSpikePricePattern(randoms()),
                () => new LowSpikePricePattern(randoms()),
            };
        }

        private Randoms randoms()
        {
            return Randoms.WeekRandoms();
        }
    }
}
