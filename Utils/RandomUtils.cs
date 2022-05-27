using System;
using System.Collections.Generic;

namespace SatelliteStorage.Utils
{
    class RandomUtils
    {
        private static readonly Random _random = new();

        public static int Roulette(List<int> chances)
        {
            var sumOfPercents = 0;
            foreach(var itemPercent in chances)
            {
                sumOfPercents += itemPercent;
            }

            var multiplier = 10;

            sumOfPercents *= multiplier;
            var rand = _random.Next(1, sumOfPercents);

            var rangeStart = 1;

            for(var i = 0; i < chances.Count; i++)
            {
                var itemPercent = chances[i];
                var rangeFinish = rangeStart + (itemPercent * multiplier);

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
