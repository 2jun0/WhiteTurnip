using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteTurnip.utils;
using WhiteTurnip.weekPrice.pattern;

namespace WhiteTurnip.weekPrice
{
    class WeekPriceFactory
    {
        private readonly Randoms randoms;
        private readonly int[] pricePatternProbabilities;
        private readonly Func<PricePattern>[] pricePatternProviders;

        public WeekPriceFactory(Randoms randoms, int[] pricePatternProbabilities, Func<PricePattern>[] pricePatternProviders)
        {
            this.randoms = randoms;
            this.pricePatternProbabilities = pricePatternProbabilities;
            this.pricePatternProviders = pricePatternProviders;
        }

        public WeekPrice Create()
        {
            PricePattern pricePattern = GetRandomPricePattern();
            int[,] prices = pricePattern.Prices();

            return new WeekPrice(prices);
        }

        private PricePattern GetRandomPricePattern()
        {
            return randoms.GetEntityRange(pricePatternProbabilities, pricePatternProviders)();
        }
    }
}
