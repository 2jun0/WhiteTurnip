using System;
using WhiteTurnip.turnip.price.pattern;
using WhiteTurnip.utils;

namespace WhiteTurnip.turnip.price
{
    class TurnipWeekPriceFactory
    {
        private readonly Randoms randoms;
        private readonly int[] pricePatternProbabilities;
        private readonly Func<IPricePattern>[] pricePatternProviders;

        public TurnipWeekPriceFactory(Randoms randoms, int[] pricePatternProbabilities, Func<IPricePattern>[] pricePatternProviders)
        {
            this.randoms = randoms;
            this.pricePatternProbabilities = pricePatternProbabilities;
            this.pricePatternProviders = pricePatternProviders;
        }

        public TurnipWeekPrice Create()
        {
            IPricePattern pricePattern = GetRandomPricePattern();
            int[,] prices = pricePattern.Prices();

            return new TurnipWeekPrice(prices);
        }

        private IPricePattern GetRandomPricePattern()
        {
            return randoms.GetEntityRange(pricePatternProbabilities, pricePatternProviders)();
        }
    }
}
