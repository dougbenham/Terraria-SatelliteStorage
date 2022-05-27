using SatelliteStorage.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace SatelliteStorage.Items
{
	class HellstoneGeneratorItem : BaseItemsGeneratorItem
	{
		public override void SetGeneratorDefaults()
		{
			base.SetGeneratorDefaults();
			Item.value = 500;
			Item.createTile = ModContent.TileType<HellstoneGeneratorTile>();
			Item.rare = ItemRarityID.Green;
			generatorType = (byte) SatelliteStorage.GeneratorTypes.HellstoneGenerator;
		}

		public override void AddGeneratorRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<QuartzModule>())
				.AddIngredient(ItemID.HellstoneBar, 25)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}

	class MeteoriteGeneratorItem : BaseItemsGeneratorItem
	{
		public override void SetGeneratorDefaults()
		{
			base.SetGeneratorDefaults();
			Item.value = 500;
			Item.createTile = ModContent.TileType<MeteoriteGeneratorTile>();
			Item.rare = ItemRarityID.Blue;
			generatorType = (byte) SatelliteStorage.GeneratorTypes.MeteoriteGenerator;
		}

		public override void AddGeneratorRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<QuartzModule>())
				.AddIngredient(ItemID.MeteoriteBar, 25)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}

	class ShroomiteGeneratorItem : BaseItemsGeneratorItem
	{
		public override void SetGeneratorDefaults()
		{
			base.SetGeneratorDefaults();
			Item.value = 500;
			Item.createTile = ModContent.TileType<ShroomiteGeneratorTile>();
			Item.rare = ItemRarityID.Lime;
			generatorType = (byte) SatelliteStorage.GeneratorTypes.ShroomiteGenerator;
		}

		public override void AddGeneratorRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<QuartzModule>())
				.AddIngredient(ItemID.ShroomiteBar, 25)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}

	class SpectreGeneratorItem : BaseItemsGeneratorItem
	{
		public override void SetGeneratorDefaults()
		{
			base.SetGeneratorDefaults();
			Item.value = 500;
			Item.createTile = ModContent.TileType<SpectreGeneratorTile>();
			Item.rare = ItemRarityID.Lime;
			generatorType = (byte) SatelliteStorage.GeneratorTypes.SpectreGenerator;
		}

		public override void AddGeneratorRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<QuartzModule>())
				.AddIngredient(ItemID.SpectreBar, 25)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}

	class LuminiteGeneratorItem : BaseItemsGeneratorItem
	{
		public override void SetGeneratorDefaults()
		{
			base.SetGeneratorDefaults();
			Item.value = 500;
			Item.createTile = ModContent.TileType<LuminiteGeneratorTile>();
			Item.rare = ItemRarityID.Red;
			generatorType = (byte) SatelliteStorage.GeneratorTypes.LuminiteGenerator;
		}

		public override void AddGeneratorRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<QuartzModule>())
				.AddIngredient(ItemID.LunarBar, 25)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}

	class ChlorophyteGeneratorItem : BaseItemsGeneratorItem
	{
		public override void SetGeneratorDefaults()
		{
			base.SetGeneratorDefaults();
			Item.value = 500;
			Item.createTile = ModContent.TileType<ChlorophyteGeneratorTile>();
			Item.rare = ItemRarityID.Lime;
			generatorType = (byte) SatelliteStorage.GeneratorTypes.ChlorophyteGenerator;
		}

		public override void AddGeneratorRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<QuartzModule>())
				.AddIngredient(ItemID.ChlorophyteBar, 25)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}

	class HallowedGeneratorItem : BaseItemsGeneratorItem
	{
		public override void SetGeneratorDefaults()
		{
			base.SetGeneratorDefaults();
			Item.value = 500;
			Item.createTile = ModContent.TileType<HallowedGeneratorTile>();
			Item.rare = ItemRarityID.LightRed;
			generatorType = (byte) SatelliteStorage.GeneratorTypes.HallowedGenerator;
		}

		public override void AddGeneratorRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<QuartzModule>())
				.AddIngredient(ItemID.HallowedBar, 25)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}

	class SoulGeneratorItem : BaseItemsGeneratorItem
	{
		public override void SetGeneratorDefaults()
		{
			base.SetGeneratorDefaults();
			Item.value = 500;
			Item.createTile = ModContent.TileType<SoulGeneratorTile>();
			Item.rare = ItemRarityID.Orange;
			generatorType = (byte) SatelliteStorage.GeneratorTypes.SoulGenerator;
		}

		public override void AddGeneratorRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<QuartzModule>())
				.AddIngredient(ModContent.ItemType<SoulBar>(), 25)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}

	class PowerGeneratorItem : BaseItemsGeneratorItem
	{
		public override void SetGeneratorDefaults()
		{
			base.SetGeneratorDefaults();
			Item.value = 500;
			Item.createTile = ModContent.TileType<PowerGeneratorTile>();
			Item.rare = ItemRarityID.Pink;
			generatorType = (byte) SatelliteStorage.GeneratorTypes.PowerGenerator;
		}

		public override void AddGeneratorRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<QuartzModule>())
				.AddIngredient(ModContent.ItemType<PowerBar>(), 25)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}