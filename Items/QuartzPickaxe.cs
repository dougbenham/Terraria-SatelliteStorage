using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace SatelliteStorage.Items
{
    class QuartzPickaxe : ModItem
    {
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 9;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 15;
			Item.useAnimation = 19;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
			Item.crit = 0;
			Item.value = 500; // Buy this item for one gold - change gold to any coin and change the value to any number <= 100
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.useTurn = true;
			Item.pick = 65; // How strong the pickaxe is, see https://terraria.gamepedia.com/Pickaxe_power for a list of common values
			Item.maxStack = 1;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(10))
			{
				Dust.NewDust(new(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.IceTorch);
			}
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<QuartzShard>(), 10)
				.AddIngredient(ItemID.Wood, 3)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
