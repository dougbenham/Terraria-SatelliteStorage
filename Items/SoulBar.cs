using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
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
			Item.rare = 3;
			Item.maxStack = 999;
			Item.value = 500;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ItemID.SoulofFlight, 1)
			.AddIngredient(ItemID.SoulofLight, 1)
			.AddIngredient(ItemID.SoulofNight, 1)
			.AddTile(TileID.Anvils)
			.Register();
		}
	}
}
