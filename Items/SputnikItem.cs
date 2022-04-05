using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Terraria.Localization;

namespace SatelliteStorage.Items
{
    class SputnikItem : ModItem
    {
		public override void SetStaticDefaults()
		{

			//Tooltip.SetDefault("Quartz Shard"); // The (English) text shown below your item's name
			DisplayName.SetDefault("Sputnik");


			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
		}

		public override void SetDefaults()
		{
			Item.width = 96; // The item texture's width
			Item.height = 48; // The item texture's height

			Item.maxStack = 1; // The item's max stack value

			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.Blue;
			Item.consumable = true;
			Item.value = 500;
			Item.createTile = ModContent.TileType<Tiles.SputnikTile>();
		}

		public override void AddRecipes()
		{
			CreateRecipe(1)
			.AddIngredient(ModContent.ItemType<Items.QuartzModule>(), 2)
			.AddIngredient(ItemID.IronBar, 35)
			.AddIngredient(ItemID.FallenStar, 50)
			.AddTile(TileID.Anvils)
			.Register();

			CreateRecipe(1)
			.AddIngredient(ModContent.ItemType<Items.QuartzModule>(), 2)
			.AddIngredient(ItemID.LeadBar, 35)
			.AddIngredient(ItemID.FallenStar, 50)
			.AddTile(TileID.Anvils)
			.Register();

		}
	}
}
