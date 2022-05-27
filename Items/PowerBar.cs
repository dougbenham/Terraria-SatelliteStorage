using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace SatelliteStorage.Items
{
	class PowerBar : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
		}

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 24;
			Item.rare = 5;
			Item.maxStack = 999;
			Item.value = 500;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ItemID.SoulofMight, 1)
			.AddIngredient(ItemID.SoulofSight, 1)
			.AddIngredient(ItemID.SoulofFright, 1)
			.AddTile(TileID.Anvils)
			.Register();
		}
	}
}
