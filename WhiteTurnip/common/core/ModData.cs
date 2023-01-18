using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteTurnip.common.core
{
    class ModData
    {
        private static string LASTEST_TIME_DATA_KEY = "ejun0.WhiteTurnip.LastestTimeData";

        public TimeData TimeData()
        {
            if (HasModData(LASTEST_TIME_DATA_KEY))
            {
                string[] parts = GetModData(LASTEST_TIME_DATA_KEY).Split(',');
                return new TimeData(int.Parse(parts[0]), int.Parse(parts[1]));
            } 
            else
            {
                return new TimeData();
            }
        }

        public void SetTimeData(TimeData timeData)
        {
            string value = string.Format("{0},{1}", timeData.DaysPlayed, timeData.TimeOfDay);
            SetModData(LASTEST_TIME_DATA_KEY, value);
        }

        private bool HasModData(string key)
        {
            return Game1.player.modData.ContainsKey(key);
        }

        private string GetModData(string key)
        {
            return Game1.player.modData[key];
        }

        private void SetModData(string key, string value)
        {
            Game1.player.modData[key] = value;
        }
    }
}
