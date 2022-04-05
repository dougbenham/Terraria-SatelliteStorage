using Terraria.ModLoader;
using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader.IO;
using Terraria.ID;
using log4net;
using SatelliteStorage.DriveSystem;
using Terraria.GameContent.Achievements;
using static IL.Terraria.Recipe;
using System;

namespace SatelliteStorage
{
    class SatelliteStorageSystem : ModSystem
    {
        private double lastGeneratorsTickTime = 0;
        private long lastGeneratorsServerTimestamp = 0;
        private bool requestStates = false;

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

            for (int i = 0; i < t_items.Count; i++)
            {
                DriveItem item = t_items[i];
                itemsCompound.Add(Utils.DriveItemsSerializer.SaveDriveItem(item));
                
                //LogManager.GetLogger("SatelliteStorage").Debug(ItemIO.Save(item));
                //SatelliteStorage.Debug("SAVE Item: " + item.type + ", stack: " + item.stack);
            }

            IList<TagCompound> generatorsCompound = new List<TagCompound>();

            Dictionary<int, int> generators = DriveChestSystem.GetGenerators();
            foreach (int key in generators.Keys)
            {
                TagCompound generatorCompound = new TagCompound();
                generatorCompound.Add("type", key);
                generatorCompound.Add("count", generators[key]);
                generatorsCompound.Add(generatorCompound);
            }

            tag.Set("SatelliteStorage_DriveChestItems", itemsCompound);
            tag.Set("SatelliteStorage_IsSputnikPlaced", DriveChestSystem.isSputnikPlaced);
            tag.Set("SatelliteStorage_Generators", generatorsCompound);
            
            base.SaveWorldData(tag);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            base.LoadWorldData(tag);
            
            IList<TagCompound> items = tag.GetList<TagCompound>("SatelliteStorage_DriveChestItems");
            IList<TagCompound> generatorsCompound = tag.GetList<TagCompound>("SatelliteStorage_Generators");
            DriveChestSystem.isSputnikPlaced = tag.GetBool("SatelliteStorage_IsSputnikPlaced");
            List<DriveItem> loadedItems = new List<DriveItem>();

            for(int i = 0; i < items.Count; i++)
            {
                TagCompound itemCompound = items[i];
                //LogManager.GetLogger("SatelliteStorage").Debug(itemCompound);
                DriveItem item = Utils.DriveItemsSerializer.LoadDriveItem(itemCompound);
                loadedItems.Add(item);
                //SatelliteStorage.Debug("LOAD Item: "+item.type + ", stack: " + item.stack);
            }

            Dictionary<int, int> generators = DriveChestSystem.GetGenerators();
            foreach (TagCompound generatorCompound in generatorsCompound)
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

        public override void PreUpdateItems()
        {
            base.PreUpdateItems();
            
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
                long timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
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

                    Player player = Main.LocalPlayer;

                    ModPacket packet = SatelliteStorage.instance.GetPacket();
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
