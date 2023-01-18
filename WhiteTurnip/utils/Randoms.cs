using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteTurnip.utils
{
    class Randoms
    {
        private Random random;

        public Randoms(int seed)
        {
            random = new Random(seed);
        }
            
        public static Randoms WeekRandoms()
        {
            return new Randoms(GetWeekRandomsSeed());
        }

        private static int GetWeekRandomsSeed()
        {
            return (int)Game1.uniqueIDForThisGame + Days.Week();
        }

        public T GetEntityRange<T>(int[]  probabilities, T[] entities)
        {
            int sumOfProbabilities = probabilities.Sum();

            int r = random.Next(sumOfProbabilities);
            int acc = 0;
            for (int i = 0; i < probabilities.Length; i++)
            {
                acc += probabilities[i];

                if (r < acc)
                {
                    return entities[i];
                }
            }

            return entities[0];
        }

        public int GetInt(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}
