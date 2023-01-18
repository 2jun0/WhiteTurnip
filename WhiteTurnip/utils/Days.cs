using StardewValley;

namespace WhiteTurnip.utils
{
    class Days
    {
        public static int DayOfWeek()
        {
            return (int)Game1.stats.DaysPlayed % 7;
        }

        public static int Week()
        {
            return (int)Game1.stats.DaysPlayed / 7;
        }

        public static bool IsAfternoon()
        {
            return Game1.timeOfDay >= 1200;
        }
    }
}
