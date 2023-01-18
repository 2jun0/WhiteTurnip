using StardewValley;

namespace WhiteTurnip.common.utils
{
    class Days
    {
        public static int DayOfWeek()
        {
            return Day() % 7;
        }

        public static int Week()
        {
            return Day() / 7;
        }

        public static int Day()
        {
            return (int)Game1.stats.DaysPlayed;
        }

        public static int Time()
        {
            return (int)Game1.timeOfDay;
        }

        public static bool IsAfternoon()
        {
            return Game1.timeOfDay >= 1200;
        }
    }
}
