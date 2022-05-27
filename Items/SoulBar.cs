using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace SatelliteStorage.Items
{
	class SoulBar : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
		}

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 24;
			Item.rare = ItemRarityID.Orange;
			Item.maxStack = 999;
			Item.value = 500;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ItemID.SoulofFlight)
			.AddIngredient(ItemID.SoulofLight)
			.AddIngredient(ItemID.SoulofNight)
			.AddTile(TileID.Anvils)
			.Register();
		}
	}
}
