using System;
using WhiteTurnip.utils;
using WhiteTurnip.turnip.price.pattern;
using WhiteTurnip.turnip.price;

namespace WhiteTurnip.turnip
{
    class TurnipContext
    {
        public const int TURNIP_BUY_PRICE = 100;

        public TurnipWeekPriceFactory WeekPriceFactory()
        {
            return new TurnipWeekPriceFactory(randoms(),
                PricePatternProbabilities(),
                PricePatternProviders());
        }

        private int[] PricePatternProbabilities()
        {
            return new int[] { 1, 2, 2, 1 };
        }

        private Func<IPricePattern>[] PricePatternProviders()
        {
            return new Func<IPricePattern>[]{
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
