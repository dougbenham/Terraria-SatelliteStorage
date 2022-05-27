using System;
using SatelliteStorage.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SatelliteStorage.Global
{
    class SatelliteStorageGlobalTile : GlobalTile
    {
        public override bool Drop(int i, int j, int type)
        {
            if (Main.netMode is NetmodeID.Server or NetmodeID.SinglePlayer &&
                type is TileID.Platinum or TileID.Gold &&
                new Random().Next(0, 100) > 70)
            {
	            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<QuartzShard>());
	            return false;
            }

            return base.Drop(i, j, type);
        }
    }
}
