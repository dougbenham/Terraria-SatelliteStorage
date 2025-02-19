using Terraria.ModLoader;
using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria.ID;
using Terraria.ModLoader.IO;
using SatelliteStorage.DriveSystem;
using System;
using Terraria.Audio;

namespace SatelliteStorage
{
    public class Generator {

        public int chance = 0;

        private List<int> dropsChances = new List<int>();
        public List<int[]> drops = new List<int[]>();

        public Generator(int chance)
        {
            this.chance = chance;
        }

        public Generator AddDrop(int type, int count, int chance, int chanceType)
        {
            drops.Add(new int[4] { type, count, chance, chanceType });
            dropsChances.Add(chance);
            return this;
        }

        public int GetRandomDropIndex()
        {
            int index = Utils.RandomUtils.Roulette(dropsChances);
            return index;
        }

        public int[] GetDropData(int index) // 0 - type, 1 - count, 2 - chance
        {
            return drops[index];
        }
    }

	public class SatelliteStorage : Mod
	{
        public static SatelliteStorage instance;
        public DriveChestSystem driveChestSystem;
        private Dictionary<int, UI.BaseUIState> uidict = new Dictionary<int, UI.BaseUIState>();

        public static bool TakeDriveChestItemSended = false;
        public static bool AddDriveChestItemSended = false;

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
            RequestSputnikState,
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

        internal enum ChancesTypes : int
        {
            VeryHighChance,
            HighChance,
            AverageChance,
            LowChance,
            VeryLowChance
        }

        public Dictionary<int, Generator> generators = new Dictionary<int, Generator>();

        public override void Load()
        {
            instance = this;
            driveChestSystem = new DriveChestSystem();

            generators.Add((int)GeneratorTypes.BaseGenerator, 
                new Generator(25)
                .AddDrop(ItemID.TinBar, 1, 100, (int)ChancesTypes.VeryHighChance)
                .AddDrop(ItemID.CopperBar, 1, 100, (int)ChancesTypes.VeryHighChance)
                .AddDrop(ItemID.IronBar, 1, 50, (int)ChancesTypes.HighChance)
                .AddDrop(ItemID.LeadBar, 1, 50, (int)ChancesTypes.HighChance)
                .AddDrop(ItemID.GoldBar, 1, 25, (int)ChancesTypes.AverageChance)
                .AddDrop(ItemID.PlatinumBar, 1, 25, (int)ChancesTypes.AverageChance)
                .AddDrop(ItemID.Diamond, 1, 5, (int)ChancesTypes.VeryLowChance)
                .AddDrop(ItemID.Amber, 1, 5, (int)ChancesTypes.VeryLowChance)
                .AddDrop(ItemID.Ruby, 1, 5, (int)ChancesTypes.VeryLowChance)
                .AddDrop(ItemID.Emerald, 1, 5, (int)ChancesTypes.VeryLowChance)
                .AddDrop(ItemID.Sapphire, 1, 5, (int)ChancesTypes.VeryLowChance)
                .AddDrop(ItemID.Topaz, 1, 5, (int)ChancesTypes.VeryLowChance)
                .AddDrop(ItemID.Amethyst, 1, 5, (int)ChancesTypes.VeryLowChance)
            );

            generators.Add((int)GeneratorTypes.HellstoneGenerator,
                new Generator(25)
                .AddDrop(ItemID.Obsidian, 1, 100, (int)ChancesTypes.VeryHighChance)
                .AddDrop(ItemID.HellstoneBar, 1, 25, (int)ChancesTypes.AverageChance)
            );

            generators.Add((int)GeneratorTypes.MeteoriteGenerator,
                new Generator(10)
                .AddDrop(ItemID.MeteoriteBar, 1, 100, (int)ChancesTypes.AverageChance)
            );

            generators.Add((int)GeneratorTypes.ShroomiteGenerator,
                new Generator(25)
                .AddDrop(ItemID.GlowingMushroom, 1, 100, (int)ChancesTypes.VeryHighChance)
                .AddDrop(ItemID.ChlorophyteBar, 1, 15, (int)ChancesTypes.VeryLowChance)
            );

            generators.Add((int)GeneratorTypes.SpectreGenerator,
                new Generator(25)
                .AddDrop(ItemID.Bone, 1, 100, (int)ChancesTypes.VeryHighChance)
                .AddDrop(ItemID.Ectoplasm, 1, 50, (int)ChancesTypes.HighChance)
                .AddDrop(ItemID.ChlorophyteBar, 1, 15, (int)ChancesTypes.VeryLowChance)
            );

            generators.Add((int)GeneratorTypes.LuminiteGenerator,
                new Generator(15)
                .AddDrop(ItemID.LunarBar, 1, 50, (int)ChancesTypes.HighChance)
                .AddDrop(ItemID.FragmentSolar, 1, 50, (int)ChancesTypes.HighChance)
                .AddDrop(ItemID.FragmentNebula, 1, 50, (int)ChancesTypes.HighChance)
                .AddDrop(ItemID.FragmentVortex, 1, 50, (int)ChancesTypes.HighChance)
                .AddDrop(ItemID.FragmentStardust, 1, 50, (int)ChancesTypes.HighChance)
            );

            generators.Add((int)GeneratorTypes.ChlorophyteGenerator,
                new Generator(25)
                .AddDrop(ItemID.JungleSpores, 1, 100, (int)ChancesTypes.VeryHighChance)
                .AddDrop(ItemID.Stinger, 1, 100, (int)ChancesTypes.VeryHighChance)
                .AddDrop(ItemID.Vine, 1, 100, (int)ChancesTypes.VeryHighChance)
                .AddDrop(ItemID.ChlorophyteBar, 1, 50, (int)ChancesTypes.HighChance)
            );

            generators.Add((int)GeneratorTypes.HallowedGenerator,
                new Generator(25)
                .AddDrop(ItemID.SilverCoin, 1, 100, (int)ChancesTypes.VeryHighChance)
                .AddDrop(ItemID.HallowedBar, 1, 25, (int)ChancesTypes.LowChance)
                .AddDrop(ItemID.SuperHealingPotion, 1, 5, (int)ChancesTypes.VeryLowChance)
                .AddDrop(ItemID.SuperManaPotion, 1, 5, (int)ChancesTypes.VeryLowChance)
            );

            generators.Add((int)GeneratorTypes.SoulGenerator,
                new Generator(25)
                .AddDrop(ItemID.SoulofFlight, 1, 100, (int)ChancesTypes.VeryHighChance)
                .AddDrop(ItemID.SoulofLight, 1, 100, (int)ChancesTypes.VeryHighChance)
                .AddDrop(ItemID.SoulofNight, 1, 100, (int)ChancesTypes.VeryHighChance)
            );

            generators.Add((int)GeneratorTypes.PowerGenerator,
                new Generator(25)
                .AddDrop(ItemID.SoulofMight, 1, 100, (int)ChancesTypes.VeryHighChance)
                .AddDrop(ItemID.SoulofSight, 1, 100, (int)ChancesTypes.VeryHighChance)
                .AddDrop(ItemID.SoulofFright, 1, 100, (int)ChancesTypes.VeryHighChance)
            );

            
            /*
            void logRare(string str, int type)
            {
                Item itm = new Item();
                itm.SetDefaults(type);
                Debug(str + ": " + itm.rare);
            }

            
            logRare("hellstone", ItemID.HellstoneBar);
            logRare("meteorite", ItemID.MeteoriteBar);
            logRare("shroomite", ItemID.ShroomiteBar);
            logRare("spectre", ItemID.SpectreBar);
            logRare("hallowed", ItemID.HallowedBar);
            logRare("chlorophyte", ItemID.ChlorophyteBar);
            logRare("luminite", ItemID.LunarBar);
            logRare("soul", ItemID.SoulofFlight);
            logRare("power", ItemID.SoulofMight);
            */

            base.Load();

            if (!Main.dedServ)
            {
                uidict.Add((int)UI.UITypes.DriveChest, new UI.DriveChestUI());


                foreach(UI.BaseUIState ui in uidict.Values)
                {
                    ui.Activate();
                }
            }
        }

        public override void Unload()
        {
            base.Unload();
        }

        public static void SetUIState(int type, bool state)
        {
            if (instance.uidict.ContainsKey(type)) instance.uidict[type].SetState(state);
        }

        public static bool GetUIState(int type)
        {
            if (!instance.uidict.ContainsKey(type)) return false;
            return instance.uidict[type].GetState();
        }

        public static void Debug(string msg)
        {
            instance.Logger.Debug(msg);
        }

        public void OnUpdateUI(GameTime gameTime)
        {
            foreach (UI.BaseUIState ui in uidict.Values)
            {
                ui.OnUpdateUI(gameTime);
            }
        }

        public void OnModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            foreach (UI.BaseUIState ui in uidict.Values)
            {
                ui.OnModifyInterfaceLayers(layers);
            }
        }
    
        public static void SyncIsSputnikPlacedToClients()
        {
            if (Main.netMode != NetmodeID.Server) return;
            var spPacket = instance.GetPacket();
            spPacket.Write((byte)MessageType.SetSputnikState);
            spPacket.Write((byte)(DriveChestSystem.isSputnikPlaced ? 1 : 0));
            spPacket.Send(-1);
            spPacket.Close();
        }

        private void SendSputnikState(int playernumber)
        {
            var rsPacket = GetPacket();
            rsPacket.Write((byte)MessageType.SetSputnikState);
            rsPacket.Write((byte)(DriveChestSystem.isSputnikPlaced ? 1 : 0));
            rsPacket.Send(playernumber);
            rsPacket.Close();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                MessageType msgType = (MessageType)reader.ReadByte();
                byte playernumber;

                switch (msgType)
                {
                    case MessageType.RequestDriveChestItems:

                        playernumber = reader.ReadByte();

                        var packet = GetPacket();
                        packet.Write((byte)MessageType.ResponseDriveChestItems);
                        packet.Write((byte)playernumber);
                        Utils.DriveItemsSerializer.WriteDriveItemsToPacket(DriveChestSystem.GetItems(), packet);
                        packet.Send(playernumber);
                        packet.Close();
                        
                        break;
                    case MessageType.SetSputnikState:
                        playernumber = reader.ReadByte();
                        DriveChestSystem.isSputnikPlaced = reader.ReadByte() == 1 ? true : false;
                        SyncIsSputnikPlacedToClients();
                        break;
                    case MessageType.ChangeGeneratorState:
                        playernumber = reader.ReadByte();
                        byte generatorType = reader.ReadByte();
                        byte changeTo = reader.ReadByte();

                        if (changeTo == 1)
                        {
                            DriveChestSystem.AddGenerator(generatorType);
                        } else
                        {
                            DriveChestSystem.SubGenerator(generatorType);
                        }

                        break;
                    case MessageType.RequestSputnikState:
                        playernumber = reader.ReadByte();
                        SendSputnikState(playernumber);
                        break;                    
                    case MessageType.RequestStates:
                        playernumber = reader.ReadByte();
                        DriveChestSystem.SyncAllGeneratorsTo(playernumber);
                        SendSputnikState(playernumber);
                        break;
                    case MessageType.TakeDriveChestItem:
                        playernumber = reader.ReadByte();

                        int clickType = reader.ReadByte();

                        Item tmouseItem = Main.player[playernumber].inventory[58];

                        bool itemTaked = false;

                        int takeItemType = reader.Read7BitEncodedInt();
                        int takeItemPrefix = reader.Read7BitEncodedInt();

                        Item takeItem;

                        int type = 0;
                        int stack = 0;
                        int prefix = 0;

                        bool isTMouseItemAir = tmouseItem.IsAir;
                        bool isTMouseItemSame = tmouseItem.type == takeItemType;
                        if (!isTMouseItemAir && !isTMouseItemSame) return;

                        if (clickType == 1)
                        {
                            if (!isTMouseItemAir && !isTMouseItemSame) return;

                            if (isTMouseItemSame)
                            {
                                if (tmouseItem.stack + 1 > tmouseItem.maxStack) return;
                            }
                        }
                        else
                        {
                            if (!isTMouseItemAir) return;
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
                                } else
                                {
                                    itemTaked = true;
                                    type = takeItem.type;
                                    stack = takeItem.stack = tmouseItem.stack + 1;
                                    prefix = takeItem.prefix;
                                }
                            } else
                            {
                                itemTaked = true;
                                type = takeItem.type;
                                stack = takeItem.stack;
                                prefix = takeItem.prefix;
                            }
                        }
                        

                        var takeItemPacket = GetPacket(); 
                        takeItemPacket.Write((byte)MessageType.TakeDriveChestItem);
                        takeItemPacket.Write((byte)playernumber);
                        takeItemPacket.Write(itemTaked);
                        takeItemPacket.Write7BitEncodedInt(type);
                        takeItemPacket.Write7BitEncodedInt(stack);
                        takeItemPacket.Write7BitEncodedInt(prefix);
                        takeItemPacket.Write((byte)clickType);
                        takeItemPacket.Send(playernumber);
                        takeItemPacket.Close();

                        break;
                    case MessageType.AddDriveChestItem:
                        playernumber = reader.ReadByte();

                        Item amouseItem = Main.player[playernumber].inventory[58];

                        bool added = false;

                        DriveItem addItem = new DriveItem();
                        addItem.type = amouseItem.type;
                        addItem.stack = amouseItem.stack;
                        addItem.prefix = amouseItem.prefix;

                        if (!amouseItem.IsAir && DriveChestSystem.AddItem(addItem))
                        {
                            amouseItem.TurnToAir();
                            added = true;
                        }

                        var addItemPacket = GetPacket();
                        addItemPacket.Write((byte)MessageType.AddDriveChestItem);
                        addItemPacket.Write((byte)playernumber);
                        addItemPacket.Write(added);
                        addItemPacket.Send(playernumber);
                        addItemPacket.Close();
                        break;
                    case MessageType.TryCraftRecipe:
                        playernumber = reader.ReadByte();
                        int recipeID = reader.Read7BitEncodedInt();
                        Recipe recipe = Main.recipe[recipeID];
                        Item[] plInvItems = Main.player[playernumber].inventory;
                        Item plMouseItem = plInvItems[58];

                        bool isMouseItemAir = plMouseItem.IsAir;
                        bool isMouseItemSame = plMouseItem.type == recipe.createItem.type;
                        if (!isMouseItemAir && !isMouseItemSame) return;

                        if (isMouseItemSame)
                        {
                            if (plMouseItem.stack + recipe.createItem.stack > plMouseItem.maxStack) return;
                        }

                        List<RecipeItemsUses> uses = DriveChestSystem.GetItemsUsesForCraft(plInvItems, recipe);
                        if (uses == null) return;

                        Dictionary<int, int> changedInvSlots = new Dictionary<int, int>();

                        uses.ForEach(u =>
                        {
                            Item item = new Item();
                            item.type = u.type;
                            item.SetDefaults(item.type);
                            item.stack = 1;

                            if (u.from == 0)
                            {
                                plInvItems[u.slot].stack -= u.stack;
                                if (plInvItems[u.slot].stack <= 0) plInvItems[u.slot].TurnToAir();

                                changedInvSlots[u.slot] = plInvItems[u.slot].stack;
                            }
                            else
                            {
                                DriveChestSystem.SubItem(u.type, u.stack);
                            }

                            //SatelliteStorage.Debug(item.Name + "(" + u.stack + ") from " + u.from + (u.from == 0 ? (" at slot " + u.slot) : ""));
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
                        tryCraftItemPacket.Write((byte)MessageType.TryCraftRecipe);
                        tryCraftItemPacket.Write((byte)playernumber);
                        tryCraftItemPacket.Write7BitEncodedInt(plMouseItem.type);
                        tryCraftItemPacket.Write7BitEncodedInt(plMouseItem.stack);
                        tryCraftItemPacket.Write7BitEncodedInt(plMouseItem.prefix);
                        tryCraftItemPacket.Write7BitEncodedInt(changedInvSlots.Keys.Count);
                        foreach (int key in changedInvSlots.Keys)
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
                        byte invSlot = reader.ReadByte();

                        Item invItem = Main.player[playernumber].inventory[invSlot];

                        if (invItem.IsAir) return;

                        DriveItem depositItem = new DriveItem();
                        depositItem.type = invItem.type;
                        depositItem.stack = invItem.stack;
                        depositItem.prefix = invItem.prefix;
                       

                        if ((depositType == 1 ? true : DriveChestSystem.HasItem(depositItem)) && DriveChestSystem.AddItem(depositItem))
                        {
                            invItem.TurnToAir();
                            
                            var depositItemPacket = GetPacket();
                            depositItemPacket.Write((byte)MessageType.DepositDriveChestItem);
                            depositItemPacket.Write((byte)playernumber);
                            depositItemPacket.Write((byte)invSlot);
                            depositItemPacket.Send(playernumber);
                            depositItemPacket.Close();
                        }

                        break;
                    default:
                        //Logger.WarnFormat("ExampleMod: Unknown Message type: {0}", msgType);
                        break;
                }
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Player player = Main.LocalPlayer;
                MessageType msgType = (MessageType)reader.ReadByte();
                byte playernumber;

                switch (msgType)
                {
                    case MessageType.ResponseDriveChestItems:
                        playernumber = reader.ReadByte();

                        List<DriveItem> items = Utils.DriveItemsSerializer.ReadDriveItems(reader);
                        DriveChestSystem.InitItems(items);
                        SoundEngine.PlaySound(SoundID.MenuOpen);
                        Main.playerInventory = true;
                        SetUIState((int)UI.UITypes.DriveChest, true);

                        break;
                    case MessageType.TakeDriveChestItem:
                        playernumber = reader.ReadByte();
                        bool itemTaked = reader.ReadBoolean();
                        Item takeItem = new Item();
                        takeItem.type = reader.Read7BitEncodedInt();
                        takeItem.SetDefaults(takeItem.type);
                        takeItem.stack = reader.Read7BitEncodedInt();
                        takeItem.prefix = reader.Read7BitEncodedInt();

                        byte clickType = reader.ReadByte();

                        if (itemTaked)
                        {
                            Main.mouseItem = takeItem;
                            if (clickType == 1)
                            {
                                SoundEngine.PlaySound(12);
                            }
                            else
                            {
                                SoundEngine.PlaySound(7);
                            }
                        }

                        TakeDriveChestItemSended = false;

                        break;
                    case MessageType.AddDriveChestItem:
                        playernumber = reader.ReadByte();
                        bool added = reader.ReadBoolean();
                        Item mouseItem = player.inventory[58];

                        if (added)
                        {
                            mouseItem.TurnToAir();
                            Main.mouseItem.TurnToAir();
                            SoundEngine.PlaySound(7);
                        }

                        AddDriveChestItemSended = false;

                        break;
                    case MessageType.DepositDriveChestItem:
                        playernumber = reader.ReadByte();
                        int invSlot = reader.ReadByte();

                        Item item = player.inventory[invSlot];
                        item.TurnToAir();

                        break;
                    case MessageType.SetSputnikState:
                        int state = reader.ReadByte();

                        DriveChestSystem.isSputnikPlaced = state == 1 ? true : false;

                        break;                    
                    case MessageType.SyncGeneratorState:
                        byte generatorType = reader.ReadByte();
                        int generatorCount = reader.Read7BitEncodedInt();
                        DriveChestSystem.instance.generators[generatorType] = generatorCount;

                        break;
                    case MessageType.TryCraftRecipe:
                        playernumber = reader.ReadByte();
                        
                        int mItemType = reader.Read7BitEncodedInt();
                        int mItemStack = reader.Read7BitEncodedInt();
                        int mItemPrefix = reader.Read7BitEncodedInt();

                        Item mItem = player.inventory[58];

                        mItem.type = mItemType;
                        mItem.SetDefaults(mItem.type);
                        mItem.stack = mItemStack;
                        mItem.prefix = mItemPrefix;

                        Main.mouseItem = mItem;

                        int subInvItemsCount = reader.Read7BitEncodedInt();

                        for (int i = 0; i < subInvItemsCount; i++)
                        {
                            int slot = reader.Read7BitEncodedInt();
                            int count = reader.Read7BitEncodedInt();
                            player.inventory[slot].stack = count;
                            if (player.inventory[slot].stack <= 0) player.inventory[slot].TurnToAir();
                        }

                        SoundEngine.PlaySound(7);
                        DriveChestSystem.checkRecipesRefresh = false;

                        break;
                    case MessageType.SyncDriveChestItem:

                        DriveItem syncItem = new DriveItem();

                        syncItem.type = reader.Read7BitEncodedInt();
                        syncItem.stack = reader.Read7BitEncodedInt();
                        syncItem.prefix = reader.Read7BitEncodedInt();

                        DriveChestSystem.SyncItem(syncItem);
                        UI.DriveChestUI.ReloadItems();

                        break;
                    default:
                        Debug("ExampleMod: Unknown Message type: " + msgType);
                        break;
                }
            }
        }
    }
}