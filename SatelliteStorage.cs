using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using SatelliteStorage.DriveSystem;
using SatelliteStorage.UI;
using SatelliteStorage.Utils;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SatelliteStorage
{
	public class SatelliteStorage : Mod
	{
		public static SatelliteStorage Instance;
		private readonly Dictionary<int, BaseUiState> _uidict = new();

		public static bool TakeDriveChestItemSended;
		public static bool AddDriveChestItemSended;

		public const int GeneratorsInterval = 20000;

		internal enum MessageType : byte
		{
			RequestDriveChestItems,
			ResponseDriveChestItems,
			TakeDriveChestItem,
			AddDriveChestItem,
			SyncDriveChestItem,
			DepositDriveChestItem,
			TryCraftRecipe,
			SetSputnikState,
			ChangeGeneratorState,
			SyncGeneratorState,
			RequestStates
		}

		internal enum GeneratorTypes : byte
		{
			BaseGenerator,
			HellstoneGenerator,
			MeteoriteGenerator,
			ShroomiteGenerator,
			SpectreGenerator,
			LuminiteGenerator,
			ChlorophyteGenerator,
			HallowedGenerator,
			SoulGenerator,
			PowerGenerator
		}

		internal enum ChancesTypes
		{
			VeryHighChance,
			HighChance,
			AverageChance,
			LowChance,
			VeryLowChance
		}

		public Dictionary<int, Generator> Generators = new();

		public override void Load()
		{
			Instance = this;
			var driveChestSystem = new DriveChestSystem();

			Generators.Add((int) GeneratorTypes.BaseGenerator,
				new Generator(25)
					.AddDrop(ItemID.TinBar, 1, 100, (int) ChancesTypes.VeryHighChance)
					.AddDrop(ItemID.CopperBar, 1, 100, (int) ChancesTypes.VeryHighChance)
					.AddDrop(ItemID.IronBar, 1, 50, (int) ChancesTypes.HighChance)
					.AddDrop(ItemID.LeadBar, 1, 50, (int) ChancesTypes.HighChance)
					.AddDrop(ItemID.GoldBar, 1, 25, (int) ChancesTypes.AverageChance)
					.AddDrop(ItemID.PlatinumBar, 1, 25, (int) ChancesTypes.AverageChance)
					.AddDrop(ItemID.Diamond, 1, 5, (int) ChancesTypes.VeryLowChance)
					.AddDrop(ItemID.Amber, 1, 5, (int) ChancesTypes.VeryLowChance)
					.AddDrop(ItemID.Ruby, 1, 5, (int) ChancesTypes.VeryLowChance)
					.AddDrop(ItemID.Emerald, 1, 5, (int) ChancesTypes.VeryLowChance)
					.AddDrop(ItemID.Sapphire, 1, 5, (int) ChancesTypes.VeryLowChance)
					.AddDrop(ItemID.Topaz, 1, 5, (int) ChancesTypes.VeryLowChance)
					.AddDrop(ItemID.Amethyst, 1, 5, (int) ChancesTypes.VeryLowChance)
			);

			Generators.Add((int) GeneratorTypes.HellstoneGenerator,
				new Generator(25)
					.AddDrop(ItemID.Obsidian, 1, 100, (int) ChancesTypes.VeryHighChance)
					.AddDrop(ItemID.HellstoneBar, 1, 25, (int) ChancesTypes.AverageChance)
			);

			Generators.Add((int) GeneratorTypes.MeteoriteGenerator,
				new Generator(10)
					.AddDrop(ItemID.MeteoriteBar, 1, 100, (int) ChancesTypes.AverageChance)
			);

			Generators.Add((int) GeneratorTypes.ShroomiteGenerator,
				new Generator(25)
					.AddDrop(ItemID.GlowingMushroom, 1, 100, (int) ChancesTypes.VeryHighChance)
					.AddDrop(ItemID.ChlorophyteBar, 1, 15, (int) ChancesTypes.VeryLowChance)
			);

			Generators.Add((int) GeneratorTypes.SpectreGenerator,
				new Generator(25)
					.AddDrop(ItemID.Bone, 1, 100, (int) ChancesTypes.VeryHighChance)
					.AddDrop(ItemID.Ectoplasm, 1, 50, (int) ChancesTypes.HighChance)
					.AddDrop(ItemID.ChlorophyteBar, 1, 15, (int) ChancesTypes.VeryLowChance)
			);

			Generators.Add((int) GeneratorTypes.LuminiteGenerator,
				new Generator(15)
					.AddDrop(ItemID.LunarBar, 1, 50, (int) ChancesTypes.HighChance)
					.AddDrop(ItemID.FragmentSolar, 1, 50, (int) ChancesTypes.HighChance)
					.AddDrop(ItemID.FragmentNebula, 1, 50, (int) ChancesTypes.HighChance)
					.AddDrop(ItemID.FragmentVortex, 1, 50, (int) ChancesTypes.HighChance)
					.AddDrop(ItemID.FragmentStardust, 1, 50, (int) ChancesTypes.HighChance)
			);

			Generators.Add((int) GeneratorTypes.ChlorophyteGenerator,
				new Generator(25)
					.AddDrop(ItemID.JungleSpores, 1, 100, (int) ChancesTypes.VeryHighChance)
					.AddDrop(ItemID.Stinger, 1, 100, (int) ChancesTypes.VeryHighChance)
					.AddDrop(ItemID.Vine, 1, 100, (int) ChancesTypes.VeryHighChance)
					.AddDrop(ItemID.ChlorophyteBar, 1, 50, (int) ChancesTypes.HighChance)
			);

			Generators.Add((int) GeneratorTypes.HallowedGenerator,
				new Generator(25)
					.AddDrop(ItemID.SilverCoin, 1, 100, (int) ChancesTypes.VeryHighChance)
					.AddDrop(ItemID.HallowedBar, 1, 25, (int) ChancesTypes.LowChance)
					.AddDrop(ItemID.SuperHealingPotion, 1, 5, (int) ChancesTypes.VeryLowChance)
					.AddDrop(ItemID.SuperManaPotion, 1, 5, (int) ChancesTypes.VeryLowChance)
			);

			Generators.Add((int) GeneratorTypes.SoulGenerator,
				new Generator(25)
					.AddDrop(ItemID.SoulofFlight, 1, 100, (int) ChancesTypes.VeryHighChance)
					.AddDrop(ItemID.SoulofLight, 1, 100, (int) ChancesTypes.VeryHighChance)
					.AddDrop(ItemID.SoulofNight, 1, 100, (int) ChancesTypes.VeryHighChance)
			);

			Generators.Add((int) GeneratorTypes.PowerGenerator,
				new Generator(25)
					.AddDrop(ItemID.SoulofMight, 1, 100, (int) ChancesTypes.VeryHighChance)
					.AddDrop(ItemID.SoulofSight, 1, 100, (int) ChancesTypes.VeryHighChance)
					.AddDrop(ItemID.SoulofFright, 1, 100, (int) ChancesTypes.VeryHighChance)
			);

			base.Load();

			if (!Main.dedServ)
			{
				_uidict.Add((int) UiTypes.DriveChest, new DriveChestUi());

				foreach (var ui in _uidict.Values)
				{
					ui.Activate();
				}
			}
		}

		public static void SetUiState(int type, bool state)
		{
			if (Instance._uidict.ContainsKey(type))
				Instance._uidict[type].SetState(state);
		}

		public static bool GetUiState(int type)
		{
			if (!Instance._uidict.ContainsKey(type))
				return false;
			return Instance._uidict[type].GetState();
		}

		public void OnUpdateUI(GameTime gameTime)
		{
			foreach (var ui in _uidict.Values)
			{
				ui.OnUpdateUI(gameTime);
			}
		}

		public void OnModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			foreach (var ui in _uidict.Values)
			{
				ui.OnModifyInterfaceLayers(layers);
			}
		}

		public void SendSputnikState()
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				return;

			var packet = GetPacket();
			packet.Write((byte) MessageType.SetSputnikState);
			packet.Write((byte) (DriveChestSystem.IsSputnikPlaced ? 1 : 0));
			packet.Send();
			packet.Close();
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode == NetmodeID.Server)
			{
				var msgType = (MessageType) reader.ReadByte();
				byte playernumber;

				switch (msgType)
				{
					case MessageType.RequestDriveChestItems:

						playernumber = reader.ReadByte();

						var packet = GetPacket();
						packet.Write((byte) MessageType.ResponseDriveChestItems);
						packet.Write(playernumber);
						DriveItemsSerializer.WriteDriveItemsToPacket(DriveChestSystem.GetItems(), packet);
						packet.Send(playernumber);
						packet.Close();

						break;
					case MessageType.SetSputnikState:
						DriveChestSystem.IsSputnikPlaced = reader.ReadByte() == 1;
						SendSputnikState(); // sync to all clients
						break;
					case MessageType.RequestStates:
						playernumber = reader.ReadByte();
						DriveChestSystem.SyncAllGeneratorsTo(playernumber);
						SendSputnikState();
						break;
					case MessageType.TakeDriveChestItem:
						playernumber = reader.ReadByte();

						int clickType = reader.ReadByte();

						var tmouseItem = Main.player[playernumber].inventory[58];

						var itemTaked = false;

						var takeItemType = reader.Read7BitEncodedInt();
						var takeItemPrefix = reader.Read7BitEncodedInt();

						Item takeItem;

						var type = 0;
						var stack = 0;
						var prefix = 0;

						var isTMouseItemAir = tmouseItem.IsAir;
						var isTMouseItemSame = tmouseItem.type == takeItemType;
						if (!isTMouseItemAir && !isTMouseItemSame)
							return;

						if (clickType == 1)
						{
							if (isTMouseItemSame)
							{
								if (tmouseItem.stack + 1 > tmouseItem.maxStack)
									return;
							}
						}
						else
						{
							if (!isTMouseItemAir)
								return;
						}


						takeItem = DriveChestSystem.TakeItem(takeItemType, takeItemPrefix, clickType == 1 ? 1 : 0);
						if (takeItem != null)
						{
							if (clickType == 1)
							{
								if (isTMouseItemAir)
								{
									itemTaked = true;
									type = takeItem.type;
									stack = takeItem.stack;
									prefix = takeItem.prefix;
								}
								else
								{
									itemTaked = true;
									type = takeItem.type;
									stack = takeItem.stack = tmouseItem.stack + 1;
									prefix = takeItem.prefix;
								}
							}
							else
							{
								itemTaked = true;
								type = takeItem.type;
								stack = takeItem.stack;
								prefix = takeItem.prefix;
							}
						}


						var takeItemPacket = GetPacket();
						takeItemPacket.Write((byte) MessageType.TakeDriveChestItem);
						takeItemPacket.Write(playernumber);
						takeItemPacket.Write(itemTaked);
						takeItemPacket.Write7BitEncodedInt(type);
						takeItemPacket.Write7BitEncodedInt(stack);
						takeItemPacket.Write7BitEncodedInt(prefix);
						takeItemPacket.Write((byte) clickType);
						takeItemPacket.Send(playernumber);
						takeItemPacket.Close();

						break;
					case MessageType.AddDriveChestItem:
						playernumber = reader.ReadByte();

						var amouseItem = Main.player[playernumber].inventory[58];

						var added = false;

						var addItem = new DriveItem();
						addItem.Type = amouseItem.type;
						addItem.Stack = amouseItem.stack;
						addItem.Prefix = amouseItem.prefix;

						if (!amouseItem.IsAir && DriveChestSystem.AddItem(addItem))
						{
							amouseItem.TurnToAir();
							added = true;
						}

						var addItemPacket = GetPacket();
						addItemPacket.Write((byte) MessageType.AddDriveChestItem);
						addItemPacket.Write(playernumber);
						addItemPacket.Write(added);
						addItemPacket.Send(playernumber);
						addItemPacket.Close();
						break;
					case MessageType.TryCraftRecipe:
						playernumber = reader.ReadByte();
						var recipeId = reader.Read7BitEncodedInt();
						var recipe = Main.recipe[recipeId];
						var plInvItems = Main.player[playernumber].inventory;
						var plMouseItem = plInvItems[58];

						var isMouseItemAir = plMouseItem.IsAir;
						var isMouseItemSame = plMouseItem.type == recipe.createItem.type;
						if (!isMouseItemAir && !isMouseItemSame)
							return;

						if (isMouseItemSame)
						{
							if (plMouseItem.stack + recipe.createItem.stack > plMouseItem.maxStack)
								return;
						}

						var uses = DriveChestSystem.GetItemUsesForCraft(plInvItems, recipe);
						if (uses == null)
							return;

						var changedInvSlots = new Dictionary<int, int>();

						uses.ForEach(u =>
						{
							var item = new Item();
							item.type = u.Type;
							item.SetDefaults(item.type);
							item.stack = 1;

							if (u.From == 0)
							{
								plInvItems[u.Slot].stack -= u.Stack;
								if (plInvItems[u.Slot].stack <= 0)
									plInvItems[u.Slot].TurnToAir();

								changedInvSlots[u.Slot] = plInvItems[u.Slot].stack;
							}
							else
							{
								DriveChestSystem.SubItem(u.Type, u.Stack);
							}
						});

						if (isMouseItemAir)
						{
							plMouseItem = recipe.createItem.Clone();
						}
						else
						{
							plMouseItem.stack += recipe.createItem.stack;
						}

						var tryCraftItemPacket = GetPacket();
						tryCraftItemPacket.Write((byte) MessageType.TryCraftRecipe);
						tryCraftItemPacket.Write(playernumber);
						tryCraftItemPacket.Write7BitEncodedInt(plMouseItem.type);
						tryCraftItemPacket.Write7BitEncodedInt(plMouseItem.stack);
						tryCraftItemPacket.Write7BitEncodedInt(plMouseItem.prefix);
						tryCraftItemPacket.Write7BitEncodedInt(changedInvSlots.Keys.Count);
						foreach (var key in changedInvSlots.Keys)
						{
							tryCraftItemPacket.Write7BitEncodedInt(key);
							tryCraftItemPacket.Write7BitEncodedInt(changedInvSlots[key]);
						}

						tryCraftItemPacket.Send(playernumber);
						tryCraftItemPacket.Close();

						break;
					case MessageType.DepositDriveChestItem:
						playernumber = reader.ReadByte();

						int depositType = reader.ReadByte();
						var invSlot = reader.ReadByte();

						var invItem = Main.player[playernumber].inventory[invSlot];

						if (invItem.IsAir)
							return;

						var depositItem = new DriveItem();
						depositItem.Type = invItem.type;
						depositItem.Stack = invItem.stack;
						depositItem.Prefix = invItem.prefix;


						if ((depositType == 1 || DriveChestSystem.HasItem(depositItem)) && DriveChestSystem.AddItem(depositItem))
						{
							invItem.TurnToAir();

							var depositItemPacket = GetPacket();
							depositItemPacket.Write((byte) MessageType.DepositDriveChestItem);
							depositItemPacket.Write(playernumber);
							depositItemPacket.Write(invSlot);
							depositItemPacket.Send(playernumber);
							depositItemPacket.Close();
						}

						break;
				}
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				var player = Main.LocalPlayer;
				var msgType = (MessageType) reader.ReadByte();
				byte playernumber;

				switch (msgType)
				{
					case MessageType.ResponseDriveChestItems:
						playernumber = reader.ReadByte();

						var items = DriveItemsSerializer.ReadDriveItems(reader);
						DriveChestSystem.InitItems(items);
						SoundEngine.PlaySound(SoundID.MenuOpen);
						Main.playerInventory = true;
						SetUiState((int) UiTypes.DriveChest, true);

						break;
					case MessageType.TakeDriveChestItem:
						playernumber = reader.ReadByte();
						var itemTaked = reader.ReadBoolean();
						var takeItem = new Item();
						takeItem.type = reader.Read7BitEncodedInt();
						takeItem.SetDefaults(takeItem.type);
						takeItem.stack = reader.Read7BitEncodedInt();
						takeItem.prefix = reader.Read7BitEncodedInt();

						var clickType = reader.ReadByte();

						if (itemTaked)
						{
							Main.mouseItem = takeItem;
							if (clickType == 1)
							{
								SoundEngine.PlaySound(SoundID.MenuTick);
							}
							else
							{
								SoundEngine.PlaySound(SoundID.Grab);
							}
						}

						TakeDriveChestItemSended = false;

						break;
					case MessageType.AddDriveChestItem:
						playernumber = reader.ReadByte();
						var added = reader.ReadBoolean();
						var mouseItem = player.inventory[58];

						if (added)
						{
							mouseItem.TurnToAir();
							Main.mouseItem.TurnToAir();
							SoundEngine.PlaySound(SoundID.Grab);
						}

						AddDriveChestItemSended = false;

						break;
					case MessageType.DepositDriveChestItem:
						playernumber = reader.ReadByte();
						int invSlot = reader.ReadByte();

						var item = player.inventory[invSlot];
						item.TurnToAir();

						break;
					case MessageType.SetSputnikState:
						int state = reader.ReadByte();
						DriveChestSystem.IsSputnikPlaced = state == 1;
						break;
					case MessageType.SyncGeneratorState:
						var generatorType = reader.ReadByte();
						var generatorCount = reader.Read7BitEncodedInt();
						DriveChestSystem.Instance.Generators[generatorType] = generatorCount;

						break;
					case MessageType.TryCraftRecipe:
						playernumber = reader.ReadByte();

						var mItemType = reader.Read7BitEncodedInt();
						var mItemStack = reader.Read7BitEncodedInt();
						var mItemPrefix = reader.Read7BitEncodedInt();

						var mItem = player.inventory[58];

						mItem.type = mItemType;
						mItem.SetDefaults(mItem.type);
						mItem.stack = mItemStack;
						mItem.prefix = mItemPrefix;

						Main.mouseItem = mItem;

						var subInvItemsCount = reader.Read7BitEncodedInt();

						for (var i = 0; i < subInvItemsCount; i++)
						{
							var slot = reader.Read7BitEncodedInt();
							var count = reader.Read7BitEncodedInt();
							player.inventory[slot].stack = count;
							if (player.inventory[slot].stack <= 0)
								player.inventory[slot].TurnToAir();
						}

						SoundEngine.PlaySound(SoundID.Grab);
						DriveChestSystem.CheckRecipesRefresh = false;

						break;
					case MessageType.SyncDriveChestItem:

						var syncItem = new DriveItem();

						syncItem.Type = reader.Read7BitEncodedInt();
						syncItem.Stack = reader.Read7BitEncodedInt();
						syncItem.Prefix = reader.Read7BitEncodedInt();

						DriveChestSystem.SyncItem(syncItem);
						DriveChestUi.ReloadItems();

						break;
					default:
						Logger.Debug("ExampleMod: Unknown Message type: " + msgType);
						break;
				}
			}
		}
	}
}