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
        private List<DriveItem> _items = new();
		public Dictionary<int, int> Generators = new();
		private readonly Dictionary<int, int> _generatedItemsQueue = new();
		public static bool IsSputnikPlaced = false;
		public static bool CheckRecipesRefresh;

        public DriveChestSystem()
        {
            Instance = this;
		}

        public static void InitItems(List<DriveItem> items)
        {
            CheckRecipesRefresh = false;
            Instance._items = items;
        }

        public static void ClearItems()
        {
            Instance._items.Clear();
        }

        public static List<DriveItem> GetItems()
        {
            return Instance._items;
        }

		public static void InitGenerators(Dictionary<int, int> generators)
		{
			Instance.Generators = generators;
		}

		public static Dictionary<int, int> GetGenerators()
        {
			return Instance.Generators;
        }

		public static void AddGenerator(byte type)
        {
			if (Instance.Generators.ContainsKey(type)) Instance.Generators[type]++;
			else Instance.Generators[type] = 1;
			SyncGenerator(type);
		}

		public static void SubGenerator(byte type)
        {
			if (!Instance.Generators.ContainsKey(type)) return;
			Instance.Generators[type]--;
			if (Instance.Generators[type] < 0) Instance.Generators[type] = 0;
			SyncGenerator(type);
		}

		public static void SyncGenerator(byte type, int to = -1)
        {
			if (Main.netMode != NetmodeID.Server) return;

			var packet = SatelliteStorage.Instance.GetPacket();
			packet.Write((byte)SatelliteStorage.MessageType.SyncGeneratorState);
			packet.Write(type);
			packet.Write7BitEncodedInt(Instance.Generators[type]);
			packet.Send(to);
			packet.Close();
		}

		public static void SyncAllGeneratorsTo(int to)
        {
			foreach (byte key in Instance.Generators.Keys)
            {
				SyncGenerator(key, to);
            }
        }

		public static bool AddItem(DriveItem item, bool needSync = true)
        {
            var searchItem = Instance._items.Find(v => v.Type == item.Type && v.Prefix == item.Prefix);
            if (searchItem != null)
            {
                searchItem.Stack += item.Stack;
				if (needSync) Instance.SendItemSync(searchItem);
                return true;
            }

            Instance._items.Add(item);

            if (needSync) Instance.SendItemSync(item);

            return true;
        }

		public static void SyncItemByType(int type)
        {
			var searchItem = Instance._items.Find(v => v.Type == type);
			if (searchItem == null) return;
			Instance.SendItemSync(searchItem);
		}

        public static bool HasItem(DriveItem item)
        {
            var searchItem = Instance._items.Find(v => v.Type == item.Type && v.Prefix == item.Prefix);
            if (searchItem != null) return true;
            return false;
        }

        public static void SyncItem(DriveItem item)
        {
            var searchItem = Instance._items.Find(v => v.Type == item.Type && v.Prefix == item.Prefix);
            if (searchItem != null)
            {
                searchItem.Stack = item.Stack;
                if (searchItem.Stack <= 0) Instance._items.Remove(searchItem);
            }
            else
            {
                Instance._items.Add(item);
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
            packet.Write7BitEncodedInt(item.Type);
            packet.Write7BitEncodedInt(item.Stack);
            packet.Write7BitEncodedInt(item.Prefix);
            packet.Send();
            packet.Close();
        }

        public static Item TakeItem(int type, int prefix, int count = 0)
        {
            var searchItem = Instance._items.Find(v => v.Type == type && v.Prefix == prefix);
            if (searchItem == null) return null;
            var item = searchItem.ToItem();
			if (count > 0) item.stack = count;
			if (item.stack > item.maxStack) item.stack = item.maxStack;
			
			searchItem.Stack -= item.stack;
            if (searchItem.Stack <= 0) Instance._items.Remove(searchItem);

            Instance.SendItemSync(searchItem);

            return item;
        }

		public static void SubItem(int type, int count)
        {
			var searchItem = Instance._items.Find(v => v.Type == type);
			if (searchItem == null) return;

			searchItem.Stack -= count;
			if (searchItem.Stack <= 0) Instance._items.Remove(searchItem);

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
						usesItem.Type = invItem.type;
						usesItem.Stack = invItem.stack;
						//int difference = 0;
						//difference = invItem.stack - playerInv[l].stack;
						//SatelliteStorage.Debug("DIFF: " + difference);
						//if (difference > 0) usesItem.stack -= difference;

						if (hasItems[invItem.type] > recipeItems[invItem.type]) hasItems[invItem.type] = recipeItems[invItem.type];
						if (usesItem.Stack > recipeItems[usesItem.Type]) usesItem.Stack = recipeItems[usesItem.Type];

						usesItem.Slot = l;
						usesItem.From = 0;

						if (usesItem.Stack > 0) uses.Add(usesItem);
					}
				}
			}
			

			for (var i = 0; i < Instance._items.Count; i++)
			{
				var driveItem = Instance._items[i];
				if (recipeItems.ContainsKey(driveItem.Type))
                {
					if (hasItems[driveItem.Type] < recipeItems[driveItem.Type])
                    {
						var needItems = recipeItems[driveItem.Type] - hasItems[driveItem.Type];
						var usesItem = new RecipeItemUse();
						usesItem.Type = driveItem.Type;
						usesItem.Stack = driveItem.Stack;
						usesItem.From = 1;
						hasItems[driveItem.Type] += driveItem.Stack;
						if (hasItems[driveItem.Type] > recipeItems[driveItem.Type]) hasItems[driveItem.Type] = recipeItems[driveItem.Type];
						if (usesItem.Stack > needItems) usesItem.Stack = needItems;
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

			if (SatelliteStorage.GetUiState((int)UiTypes.DriveChest))
			{
				SatelliteStorage.SetUiState((int)UiTypes.DriveChest, false);
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
				SatelliteStorage.SetUiState((int)UiTypes.DriveChest, true);
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

			foreach (var key in Instance.Generators.Keys)
            {
				for (var i = 0; i < Instance.Generators[key]; i++)
				{
					var generator = SatelliteStorage.Instance.Generators[key];

					if (random.Next(0, 100) <= generator.Chance)
					{
						var index = generator.GetRandomDropIndex();
						var data = generator.GetDropData(index);


						if (Instance._generatedItemsQueue.ContainsKey(data[0])) Instance._generatedItemsQueue[data[0]] += data[1];
						else Instance._generatedItemsQueue.Add(data[0], data[1]);
					}
				}
            }

			foreach (var key in Instance._generatedItemsQueue.Keys)
			{
				var addItem = new DriveItem();
				addItem.Type = key;
				addItem.Stack = Instance._generatedItemsQueue[key];
				AddItem(addItem);
			}

			Instance._generatedItemsQueue.Clear();
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

				if (dictionary.ContainsKey(driveItem.Type))
				{
					dictionary[driveItem.Type] += driveItem.Stack;
				}
				else
				{
					dictionary[driveItem.Type] = driveItem.Stack;
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
