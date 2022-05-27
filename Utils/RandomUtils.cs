using System;
using System.Collections.Generic;

namespace SatelliteStorage.Utils
{
    class RandomUtils
    {
        private static Random random = new Random();

        public static int Roulette(List<int> chances)
        {
            int sumOfPercents = 0;
            foreach(int itemPercent in chances)
            {
                sumOfPercents += itemPercent;
            }

            int multiplier = 10;

            sumOfPercents *= multiplier;
            int rand = random.Next(1, sumOfPercents);

            int rangeStart = 1;

            for(int i = 0; i < chances.Count; i++)
            {
                int itemPercent = chances[i];
                int rangeFinish = rangeStart + (itemPercent * multiplier);

                if (rand >= rangeStart && rand <= rangeFinish)
                {
                    return +i;
                }

                rangeStart = rangeFinish + 1;
            }

            return 0;
        }
    }
}
