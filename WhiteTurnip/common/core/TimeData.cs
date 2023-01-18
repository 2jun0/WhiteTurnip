namespace WhiteTurnip.common.core
{
    class TimeData
    {
        private readonly int daysPlayed;
        private readonly int timeOfDay;

        public int DaysPlayed => daysPlayed;
        public int TimeOfDay => timeOfDay;

        public TimeData(int daysPlayed, int timeOfDay)
        {
            this.daysPlayed = daysPlayed;
            this.timeOfDay = timeOfDay;
        }

        public TimeData()
        {
            this.daysPlayed = -1;
            this.timeOfDay = -1;
        }

        public int Week()
        {
            return daysPlayed / 7;
        }

        public int DayOfWeek()
        {
            return daysPlayed % 7;
        }

        public bool isAfternoon()
        {
            return timeOfDay >= 1200;
        }
    }
}
