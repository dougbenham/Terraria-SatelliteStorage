using Terraria.ModLoader;
using Terraria;
using System.Collections.Generic;

namespace SatelliteStorage
{
    class SatelliteStoragePlayer : ModPlayer
    {

        private static List<bool> oldAdjList;

        public override bool CanUseItem(Item item)
        {
            if (UI.DriveChestUI.mouseOver) return false;
            return base.CanUseItem(item);
        }

        public static bool CheckAdjChanged()
        {
            Player player = Main.LocalPlayer;
            List<bool> adjList = new List<bool>();

            adjList.Add(player.adjHoney);
            adjList.Add(player.adjLava);
            adjList.Add(player.adjWater);

            foreach (bool b in player.adjTile)
            {
                adjList.Add(b);
            }

            if (oldAdjList == null || oldAdjList.Count != adjList.Count)
            {
                oldAdjList = adjList;
                return true;
            }

            for (int i = 0; i < adjList.Count; i++)
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
