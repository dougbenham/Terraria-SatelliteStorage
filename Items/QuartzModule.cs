using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace SatelliteStorage.Items
{
    class QuartzModule : ModItem
    {
		public override void SetStaticDefaults()
		{

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
		}

		public override void SetDefaults()
		{
			Item.width = 26; // The item texture's width
			Item.height = 26; // The item texture's height

			Item.maxStack = 99; // The item's max stack value
		}

		public override void AddRecipes()
		{
			CreateRecipe(1)
			.AddIngredient(ItemID.CrimtaneBar, 5)
			.AddIngredient(ModContent.ItemType<Items.QuartzShard>(), 10)
			.AddTile(TileID.Anvils)
			.Register();

			CreateRecipe(1)
			.AddIngredient(ItemID.DemoniteBar, 5)
			.AddIngredient(ModContent.ItemType<Items.QuartzShard>(), 10)
			.AddTile(TileID.Anvils)
			.Register();
		}
	}
}
