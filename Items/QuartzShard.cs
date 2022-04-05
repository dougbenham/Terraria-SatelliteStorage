using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Terraria.Localization;

namespace SatelliteStorage.Items
{
    class QuartzShard : ModItem
    {
		public override void SetStaticDefaults()
		{
			
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
		}

		public override void SetDefaults()
		{
			Item.width = 20; // The item texture's width
			Item.height = 20; // The item texture's height
			Item.maxStack = 999; // The item's max stack value
			Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.


			/*
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;

			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;

			Item.createTile = ModContent.TileType<Tiles.QuartzOre>();
			*/
		}

		// Researching the Example item will give you immediate access to the torch, block, wall and workbench!
		public override void OnResearched(bool fullyResearched)
		{
			if (fullyResearched)
			{
				//CreativeUI.ResearchItem(ModContent.ItemType<ExampleTorch>());
				//CreativeUI.ResearchItem(ModContent.ItemType<ExampleBlock>());
				//CreativeUI.ResearchItem(ModContent.ItemType<ExampleWall>());
				//CreativeUI.ResearchItem(ModContent.ItemType<ExampleWorkbench>());
			}
		}
	}
}
