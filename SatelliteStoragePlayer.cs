using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace SatelliteStorage
{
    class SatelliteStoragePlayer : ModPlayer
    {
        private static List<bool> _oldAdjList;
        
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

            if (_oldAdjList == null || _oldAdjList.Count != adjList.Count)
            {
                _oldAdjList = adjList;
                return true;
            }

            for (var i = 0; i < adjList.Count; i++)
            {
                if (adjList[i] != _oldAdjList[i])
                {
                    _oldAdjList = adjList;
                    return true;
                }
            }

            return false;
        }
    }
}
