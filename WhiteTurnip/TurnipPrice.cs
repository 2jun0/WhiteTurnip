using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace WhiteTurnip
{
    class TurnipPrice
    {
        public static int TURNIP_BUY_PRICE = 100;

        private Random random = null;
        private WeekPrice currentWeekPrice;
        private int currentWeek;

        public int GetTurnipPrice(TimeData timeData)
        {
            if (timeData.timeOfDay < 1200)
            {
                return currentWeekPrice.prices[2 * (timeData.daysPlayed%7)];
            }
            else
            {
                return currentWeekPrice.prices[2 * (timeData.daysPlayed%7) + 1];
            }
        }

        public void Update(int week)
        {
            currentWeek = week;
            currentWeekPrice = DecideWeekPrice();
        }

        private WeekPrice DecideWeekPrice()
        {
            random = new Random((int)Game1.uniqueIDForThisGame + this.currentWeek);
            var r = random.NextDouble();
            if (r < 0.1375) return GetDecreasingPrice();
            else if (r < 0.1375 + 0.35) return GetFluctuatingPrice();
            else if (r < 0.1375 + 0.35 + 0.1375) return GetHighSpikePrice();
            else return GetLowSpikePrice();
        }

        private WeekPrice GetDecreasingPrice()
        {
            WeekPrice wp = new WeekPrice(WeekPrice.PATTERN_DECREASING);

            for (var i = 2; i < 14; i++)
            {
                var diff = random.Next(-10, -1);
                wp.prices[i] = wp.prices[i - 1] + diff;
            }

            return wp;
        }

        private WeekPrice GetFluctuatingPrice()
        {
            WeekPrice wp = new WeekPrice(WeekPrice.PATTERN_FLUCTUATING);

            for (var i = 2; i < 14; i++)
            {
                var diff = random.Next(-10, 10);
                wp.prices[i] = wp.prices[i - 1] + diff;
            }

            return wp;
        }

        private WeekPrice GetHighSpikePrice()
        {
            WeekPrice wp = new WeekPrice(WeekPrice.PATTERN_HIGH_SPIKE);
            var diff = 0;
            for (var i = 2; i < 7; i++)
            {
                diff = random.Next(-10, -1);
                wp.prices[i] = wp.prices[i - 1] + diff;
            }

            for (var i = 7; i < 9; i++)
            {
                diff = random.Next(1, 10);
                wp.prices[i] = wp.prices[i - 1] + diff;
            }

            diff = random.Next(100, 700);
            wp.prices[9] = wp.prices[8] + diff;

            for (var i =10; i < 14; i++)
            {
                diff = random.Next(-10, 1);
                wp.prices[i] = wp.prices[i - 1] + diff;
            }

            return wp;
        }

        private WeekPrice GetLowSpikePrice()
        {
            WeekPrice wp = new WeekPrice(WeekPrice.PATTERN_LOW_SPIKE);
            var diff = 0;
            for (var i = 2; i < 7; i++)
            {
                diff = random.Next(-10, -1);
                wp.prices[i] = wp.prices[i - 1] + diff;
            }

            for (var i = 7; i < 11; i++)
            {
                diff = random.Next(-1, 10);
                wp.prices[i] = wp.prices[i - 1] + diff;
            }

            for (var i = 12; i < 14; i++)
            {
                diff = random.Next(-12, -1);
                wp.prices[i] = wp.prices[i - 1] + diff;
            }

            return wp;
        }
    }
}
