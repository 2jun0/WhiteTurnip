namespace WhiteTurnip.weekPrice
{
    class WeekPrice
    {
        private int[,] prices; // size: (7, 2)

        public WeekPrice(int[,] prices)
        {
            this.prices = prices;
        }

        public int GetPrice(int dayOfWeek, bool isAfternoon)
        {
            if (isAfternoon)
            {
                return prices[dayOfWeek, 1];
            }
            else
            {
                return prices[dayOfWeek, 0];
            }
        }
    }
}
