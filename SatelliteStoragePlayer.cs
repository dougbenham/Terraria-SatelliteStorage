using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

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

        public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
        {
	        if (context != ItemSlot.Context.InventoryItem && context != ItemSlot.Context.InventoryCoin && context != ItemSlot.Context.InventoryAmmo)
		        return false;
	        if (storageAccess.X < 0 || storageAccess.Y < 0)
		        return false;
	        Item item = inventory[slot];
	        if (item.favorited || item.IsAir)
		        return false;
	        int oldType = item.type;
	        int oldStack = item.stack;
	        GetStorageHeart().TryDeposit(item);

	        if (item.type != oldType || item.stack != oldStack)
	        {
		        SoundEngine.PlaySound(SoundID.Grab);
		        StorageGUI.RefreshItems();
	        }

	        return true;
        }
    }
}
