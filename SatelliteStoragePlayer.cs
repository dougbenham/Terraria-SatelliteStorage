using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace SatelliteStorage
{
    class SatelliteStoragePlayer : ModPlayer
    {
        private static List<bool> oldAdjList;
        
        public static bool CheckAdjChanged()
        {
            var player = Main.LocalPlayer;
            var adjList = new List<bool>();

            adjList.Add(player.adjHoney);
            adjList.Add(player.adjLava);
            adjList.Add(player.adjWater);

            foreach (var b in player.adjTile)
            {
                adjList.Add(b);
            }

            if (oldAdjList == null || oldAdjList.Count != adjList.Count)
            {
                oldAdjList = adjList;
                return true;
            }

            for (var i = 0; i < adjList.Count; i++)
            {
                if (adjList[i] != oldAdjList[i])
                {
                    oldAdjList = adjList;
                    return true;
                }
            }

            return false;
        }
    }
}
