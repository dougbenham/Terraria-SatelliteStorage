using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SatelliteStorage.Global
{
    class SatelliteStorageGlobalTile : GlobalTile
    {
        public override bool Drop(int i, int j, int type)
        {
            if (Main.netMode == NetmodeID.Server || Main.netMode == NetmodeID.SinglePlayer)
            {
                if (type == TileID.Platinum || type == TileID.Gold)
                {
                    if (new Random().Next(0, 100) > 70)
                    {
                        Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.QuartzShard>());
                        return false;
                    }
                }
            }

            return base.Drop(i, j, type);
        }
    }
}
