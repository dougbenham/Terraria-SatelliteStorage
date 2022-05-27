using System;
using System.Collections.Generic;
using SatelliteStorage.UI;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace SatelliteStorage.DriveSystem
{
	public class DriveChestSystem
    {
        public static DriveChestSystem Instance;
        public static readonly Dictionary<int, Recipe> AvailableRecipes = new();
        private List<DriveItem> items = new();
		public Dictionary<int, int> generators = new();
		private readonly Dictionary<int, int> generatedItemsQueue = new();
		public static bool IsSputnikPlaced = false;
		public static bool CheckRecipesRefresh;

        public DriveChestSystem()
        {
            Instance = this;
		}

        public static void InitItems(List<DriveItem> items)
        {
            CheckRecipesRefresh = false;
            Instance.items = items;
        }

        public static void ClearItems()
        {
            Instance.items.Clear();
        }

        public static List<DriveItem> GetItems()
        {
            return Instance.items;
        }

		public static void InitGenerators(Dictionary<int, int> generators)
		{
			Instance.generators = generators;
		}

		public static Dictionary<int, int> GetGenerators()
        {
			return Instance.generators;
        }

		public static void AddGenerator(byte type)
        {
			if (Instance.generators.ContainsKey(type)) Instance.generators[type]++;
			else Instance.generators[type] = 1;
			SyncGenerator(type);
		}

		public static void SubGenerator(byte type)
        {
			if (!Instance.generators.ContainsKey(type)) return;
			Instance.generators[type]--;
			if (Instance.generators[type] < 0) Instance.generators[type] = 0;
			SyncGenerator(type);
		}

		public static void SyncGenerator(byte type, int to = -1)
        {
			if (Main.netMode != NetmodeID.Server) return;

			var packet = SatelliteStorage.Instance.GetPacket();
			packet.Write((byte)SatelliteStorage.MessageType.SyncGeneratorState);
			packet.Write(type);
			packet.Write7BitEncodedInt(Instance.generators[type]);
			packet.Send(to);
			packet.Close();
		}

		public static void SyncAllGeneratorsTo(int to)
        {
			foreach (byte key in Instance.generators.Keys)
            {
				SyncGenerator(key, to);
            }
        }

		public static bool AddItem(DriveItem item, bool needSync = true)
        {
            var searchItem = Instance.items.Find(v => v.type == item.type && v.prefix == item.prefix);
            if (searchItem != null)
            {
                searchItem.Stack += item.Stack;
				if (needSync) Instance.SendItemSync(searchItem);
                return true;
            }

            Instance.items.Add(item);

            if (needSync) Instance.SendItemSync(item);

            return true;
        }

		public static void SyncItemByType(int type)
        {
			var searchItem = Instance.items.Find(v => v.type == type);
			if (searchItem == null) return;
			Instance.SendItemSync(searchItem);
		}

        public static bool HasItem(DriveItem item)
        {
            var searchItem = Instance.items.Find(v => v.type == item.type && v.prefix == item.prefix);
            if (searchItem != null) return true;
            return false;
        }

        public static void SyncItem(DriveItem item)
        {
            var searchItem = Instance.items.Find(v => v.type == item.type && v.prefix == item.prefix);
            if (searchItem != null)
            {
                searchItem.Stack = item.Stack;
                if (searchItem.Stack <= 0) Instance.items.Remove(searchItem);
            }
            else
            {
                Instance.items.Add(item);
            }

            CheckRecipesRefresh = false;
        }
       
        private void SendItemSync(DriveItem item)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                CheckRecipesRefresh = false;
                return;
            }

            var packet = SatelliteStorage.Instance.GetPacket();
            packet.Write((byte)SatelliteStorage.MessageType.SyncDriveChestItem);
            packet.Write7BitEncodedInt(item.type);
            packet.Write7BitEncodedInt(item.Stack);
            packet.Write7BitEncodedInt(item.prefix);
            packet.Send();
            packet.Close();
        }

        public static Item TakeItem(int type, int prefix, int count = 0)
        {
            var searchItem = Instance.items.Find(v => v.type == type && v.prefix == prefix);
            if (searchItem == null) return null;
            var item = searchItem.ToItem();
			if (count > 0) item.stack = count;
			if (item.stack > item.maxStack) item.stack = item.maxStack;
			
			searchItem.Stack -= item.stack;
            if (searchItem.Stack <= 0) Instance.items.Remove(searchItem);

            Instance.SendItemSync(searchItem);

            return item;
        }

		public static void SubItem(int type, int count)
        {
			var searchItem = Instance.items.Find(v => v.type == type);
			if (searchItem == null) return;

			searchItem.Stack -= count;
			if (searchItem.Stack <= 0) Instance.items.Remove(searchItem);

			Instance.SendItemSync(searchItem);
		}


		public static List<RecipeItemUse> GetItemUsesForCraft(Item[] playerInv, Recipe recipe)
		{
			var uses = new List<RecipeItemUse>();
			var recipeItems = new Dictionary<int, int>();
			var hasItems = new Dictionary<int, int>();

			recipe.requiredItem.ForEach(r =>
			{
				recipeItems[r.type] = r.stack;
				hasItems[r.type] = 0;
			});

			
			for (var l = 0; l < 58; l++)
			{
				var invItem = playerInv[l];
				if (recipeItems.ContainsKey(invItem.type))
                {
					if (hasItems[invItem.type] < recipeItems[invItem.type])
					{
						hasItems[invItem.type] += invItem.stack;
						var usesItem = new RecipeItemUse();
						usesItem.type = invItem.type;
						usesItem.stack = invItem.stack;
						//int difference = 0;
						//difference = invItem.stack - playerInv[l].stack;
						//SatelliteStorage.Debug("DIFF: " + difference);
						//if (difference > 0) usesItem.stack -= difference;

						if (hasItems[invItem.type] > recipeItems[invItem.type]) hasItems[invItem.type] = recipeItems[invItem.type];
						if (usesItem.stack > recipeItems[usesItem.type]) usesItem.stack = recipeItems[usesItem.type];

						usesItem.slot = l;
						usesItem.from = 0;

						if (usesItem.stack > 0) uses.Add(usesItem);
					}
				}
			}
			

			for (var i = 0; i < Instance.items.Count; i++)
			{
				var driveItem = Instance.items[i];
				if (recipeItems.ContainsKey(driveItem.type))
                {
					if (hasItems[driveItem.type] < recipeItems[driveItem.type])
                    {
						var needItems = recipeItems[driveItem.type] - hasItems[driveItem.type];
						var usesItem = new RecipeItemUse();
						usesItem.type = driveItem.type;
						usesItem.stack = driveItem.Stack;
						usesItem.from = 1;
						hasItems[driveItem.type] += driveItem.Stack;
						if (hasItems[driveItem.type] > recipeItems[driveItem.type]) hasItems[driveItem.type] = recipeItems[driveItem.type];
						if (usesItem.stack > needItems) usesItem.stack = needItems;
						uses.Add(usesItem);
					}
				}
			}

			foreach(var key in recipeItems.Keys)
            {
				if (hasItems[key] < recipeItems[key]) return null;
            }

			return uses;
		}

		public static bool RequestOpenDriveChest()
        {
			var player = Main.LocalPlayer;
			Main.mouseRightRelease = false;

			if (SatelliteStorage.GetUIState((int)UITypes.DriveChest))
			{
				SatelliteStorage.SetUIState((int)UITypes.DriveChest, false);
				SoundEngine.PlaySound(SoundID.MenuClose);
				return true;
			}

			//NetMessage.SendData((int)SatelliteStorage.MessageType.RequestDriveChestItems);

			if (player.sign >= 0)
			{
				SoundEngine.PlaySound(SoundID.MenuClose);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = "";
			}

			if (Main.editChest)
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = "";
			}

			if (player.editedChestName)
			{
				player.editedChestName = false;
			}

			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				SoundEngine.PlaySound(SoundID.MenuOpen);
				Main.playerInventory = true;
				SatelliteStorage.SetUIState((int)UITypes.DriveChest, true);
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				var packet = SatelliteStorage.Instance.GetPacket();
				packet.Write((byte)SatelliteStorage.MessageType.RequestDriveChestItems);
				packet.Write((byte)player.whoAmI);
				packet.Send();
				packet.Close();
			}

			return true;
		}

		public static void OnGeneratorsTick()
        {
			if (Main.netMode != NetmodeID.SinglePlayer && Main.netMode != NetmodeID.Server) return;
			
			var random = new Random();

			foreach (var key in Instance.generators.Keys)
            {
				for (var i = 0; i < Instance.generators[key]; i++)
				{
					var generator = SatelliteStorage.Instance.generators[key];

					if (random.Next(0, 100) <= generator.chance)
					{
						var index = generator.GetRandomDropIndex();
						var data = generator.GetDropData(index);


						if (Instance.generatedItemsQueue.ContainsKey(data[0])) Instance.generatedItemsQueue[data[0]] += data[1];
						else Instance.generatedItemsQueue.Add(data[0], data[1]);
					}
				}
            }

			foreach (var key in Instance.generatedItemsQueue.Keys)
			{
				var addItem = new DriveItem();
				addItem.type = key;
				addItem.Stack = Instance.generatedItemsQueue[key];
				AddItem(addItem);
			}

			Instance.generatedItemsQueue.Clear();
		}

        public static void ResearchRecipes()
        {
			var player = Main.LocalPlayer;

			var maxRecipes = Recipe.maxRecipes;
			
			AvailableRecipes.Clear();
			
			var dictionary = new Dictionary<int, int>();
			var array = Main.player[Main.myPlayer].inventory;
			
			for (var l = 0; l < 58; l++)
			{
				var item = array[l];
				if (item.stack > 0)
				{
					if (dictionary.ContainsKey(item.type))
					{
						dictionary[item.type] += item.stack;
					}
					else
					{
						dictionary[item.type] = item.stack;
					}
				}
			}

            var driveItems = GetItems();

			for (var l = 0; l < driveItems.Count; l++)
			{
				var driveItem = driveItems[l];

				if (dictionary.ContainsKey(driveItem.type))
				{
					dictionary[driveItem.type] += driveItem.Stack;
				}
				else
				{
					dictionary[driveItem.type] = driveItem.Stack;
				}
			}

			/*
			if (Main.player[Main.myPlayer].chest != -1)
			{
				if (Main.player[Main.myPlayer].chest > -1)
				{
					array = Main.chest[Main.player[Main.myPlayer].chest].item;
				}
				else if (Main.player[Main.myPlayer].chest == -2)
				{
					array = Main.player[Main.myPlayer].bank.item;
				}
				else if (Main.player[Main.myPlayer].chest == -3)
				{
					array = Main.player[Main.myPlayer].bank2.item;
				}
				else if (Main.player[Main.myPlayer].chest == -4)
				{
					array = Main.player[Main.myPlayer].bank3.item;
				}
				else if (Main.player[Main.myPlayer].chest == -5)
				{
					array = Main.player[Main.myPlayer].bank4.item;
				}
				for (int m = 0; m < 40; m++)
				{
					item = array[m];
					if (item != null && item.stack > 0)
					{
						if (dictionary.ContainsKey(item.netID))
						{
							dictionary[item.netID] += item.stack;
						}
						else
						{
							dictionary[item.netID] = item.stack;
						}
					}
				}
			}
			*/

			for (var n = 0; n < maxRecipes && Main.recipe[n].createItem.type != ItemID.None; n++)
			{
				var flag = true;
				if (flag)
				{
					for (var num3 = 0; num3 < Main.recipe[n].requiredTile.Count && Main.recipe[n].requiredTile[num3] != -1; num3++)
					{
						if (!Main.player[Main.myPlayer].adjTile[Main.recipe[n].requiredTile[num3]])
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					for (var num4 = 0; num4 < Main.recipe[n].requiredItem.Count; num4++)
					{
						var item = Main.recipe[n].requiredItem[num4];
						if (item.type == ItemID.None)
						{
							break;
						}
						var num5 = item.stack;
						var flag2 = false;
						/*
						foreach (int key in dictionary.Keys)
						{
							if (Main.recipe[n].useWood(key, item.type) || Main.recipe[n].useSand(key, item.type) || Main.recipe[n].useIronBar(key, item.type) || Main.recipe[n].useFragment(key, item.type) || Main.recipe[n].AcceptedByItemGroups(key, item.type) || Main.recipe[n].usePressurePlate(key, item.type))
							{
								num5 -= dictionary[key];
								flag2 = true;
							}
						}
						*/
						if (!flag2 && dictionary.ContainsKey(item.type))
						{
							num5 -= dictionary[item.type];
						}
						if (num5 > 0)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					var num6 = !Main.recipe[n].HasCondition(Recipe.Condition.NearWater) || Main.player[Main.myPlayer].adjWater || Main.player[Main.myPlayer].adjTile[172];
					var flag3 = !Main.recipe[n].HasCondition(Recipe.Condition.NearHoney) || Main.recipe[n].HasCondition(Recipe.Condition.NearHoney) == Main.player[Main.myPlayer].adjHoney;
					var flag4 = !Main.recipe[n].HasCondition(Recipe.Condition.NearLava) || Main.recipe[n].HasCondition(Recipe.Condition.NearLava) == Main.player[Main.myPlayer].adjLava;
					var flag5 = !Main.recipe[n].HasCondition(Recipe.Condition.InSnow) || Main.player[Main.myPlayer].ZoneSnow;
					var flag6 = !Main.recipe[n].HasCondition(Recipe.Condition.InGraveyardBiome) || Main.player[Main.myPlayer].ZoneGraveyard;
					if (!(num6 && flag3 && flag4 && flag5 && flag6))
					{
						flag = false;
					}
				}
				if (flag)
				{
					AvailableRecipes[n] = Main.recipe[n];
				}
			}
		}
    }
}
