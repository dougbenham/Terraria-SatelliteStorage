using Terraria.ModLoader;
using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using Terraria.ID;
using SatelliteStorage.DriveSystem;
using System;

namespace SatelliteStorage
{
    class SatelliteStorageSystem : ModSystem
    {
        private double lastGeneratorsTickTime;
        private long lastGeneratorsServerTimestamp;
        private bool requestStates;

        public override void UpdateUI(GameTime gameTime)
        {
            SatelliteStorage.instance.OnUpdateUI(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            SatelliteStorage.instance.OnModifyInterfaceLayers(layers);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            IList<DriveItem> t_items = DriveChestSystem.GetItems();

            IList<TagCompound> itemsCompound = new List<TagCompound>();

            for (var i = 0; i < t_items.Count; i++)
            {
                var item = t_items[i];
                itemsCompound.Add(Utils.DriveItemsSerializer.SaveDriveItem(item));
            }

            IList<TagCompound> generatorsCompound = new List<TagCompound>();

            var generators = DriveChestSystem.GetGenerators();
            foreach (var key in generators.Keys)
            {
                var generatorCompound = new TagCompound();
                generatorCompound.Add("type", key);
                generatorCompound.Add("count", generators[key]);
                generatorsCompound.Add(generatorCompound);
            }

            tag.Set("SatelliteStorage_DriveChestItems", itemsCompound);
            tag.Set("SatelliteStorage_IsSputnikPlaced", DriveChestSystem.IsSputnikPlaced);
            tag.Set("SatelliteStorage_Generators", generatorsCompound);
            
            base.SaveWorldData(tag);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            base.LoadWorldData(tag);
            
            var items = tag.GetList<TagCompound>("SatelliteStorage_DriveChestItems");
            var generatorsCompound = tag.GetList<TagCompound>("SatelliteStorage_Generators");
            DriveChestSystem.IsSputnikPlaced = tag.GetBool("SatelliteStorage_IsSputnikPlaced");
            var loadedItems = new List<DriveItem>();

            for(var i = 0; i < items.Count; i++)
            {
                var itemCompound = items[i];
                var item = Utils.DriveItemsSerializer.LoadDriveItem(itemCompound);
                loadedItems.Add(item);
            }

            var generators = DriveChestSystem.GetGenerators();
            foreach (var generatorCompound in generatorsCompound)
            {
                generators[generatorCompound.GetInt("type")] = generatorCompound.GetInt("count");
            }

            DriveChestSystem.InitItems(loadedItems);
            DriveChestSystem.InitGenerators(generators);
        }

        public override void OnWorldUnload()
        {
            base.OnWorldUnload();
            DriveChestSystem.ClearItems();
        }

        public override void AddRecipes()
        {
            Mod.CreateRecipe(ItemID.MagicMirror, 1)
            .AddIngredient(ItemID.Glass, 250)
            .AddIngredient(ItemID.FallenStar, 50)
            .AddIngredient(ItemID.ReflectiveDye, 1)
            .Register();

            Mod.CreateRecipe(ItemID.IceMirror, 1)
            .AddIngredient(ItemID.MagicMirror, 1)
            .AddIngredient(ItemID.IceBlock, 50)
            .Register();
        }

        public override void PostUpdateWorld()
        {
            if (Main.netMode == NetmodeID.SinglePlayer && Main.gameTimeCache.TotalGameTime.TotalMilliseconds > lastGeneratorsTickTime + SatelliteStorage.GeneratorsInterval)
            {
                lastGeneratorsTickTime = Main.gameTimeCache.TotalGameTime.TotalMilliseconds;

                DriveChestSystem.OnGeneratorsTick();
            }
           

            base.PostUpdateWorld();
        }

        public override void PostUpdateEverything()
        {
            if (Main.netMode == NetmodeID.Server) {
                var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                if (timestamp > lastGeneratorsServerTimestamp + SatelliteStorage.GeneratorsInterval)
                {
                    lastGeneratorsServerTimestamp = timestamp;
                    DriveChestSystem.OnGeneratorsTick();
                }
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (!requestStates)
                {
                    requestStates = true;

                    var player = Main.LocalPlayer;

                    var packet = SatelliteStorage.instance.GetPacket();
                    packet.Write((byte)SatelliteStorage.MessageType.RequestStates);
                    packet.Write((byte)player.whoAmI);
                    packet.Send();
                    packet.Close();
                }
            }

            base.PostUpdateEverything();
        }
    }
}
