using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Localization;

namespace SatelliteStorage.Items
{
    class BaseItemsGeneratorItem : ModItem
    {
		public byte generatorType;

		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 48;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;

			SetGeneratorDefaults();
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{

			var line = new TooltipLine(Mod, "Drops", Language.GetTextValue("Mods.SatelliteStorage.Common.GeneratorDropsTitle"))
			{
				
				OverrideColor = new Color(215, 176, 235)
			};
			tooltips.Add(line);

			var gen = SatelliteStorage.instance.generators[generatorType];

			foreach (var data in gen.drops)
			{
				var itm = new Item();
				itm.SetDefaults(data[0]);

				line = new(Mod, "dropText_" + Item.Name + "_" + itm.Name, "● " + itm.Name + " (" + Language.GetTextValue("Mods.SatelliteStorage.ChanceNames._" + data[3]) + ")")
				{
					OverrideColor = Terraria.GameContent.UI.ItemRarity.GetColor(itm.rare)
				};
				tooltips.Add(line);
			}
			/*
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.mod == "Terraria" && line2.Name == "ItemName")
				{
					line2.overrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
				}
			}*/
		}

		public override void AddRecipes()
		{
			AddGeneratorRecipes();
		}

		public virtual void SetGeneratorDefaults()
		{
			Item.maxStack = 1;
			Item.value = 500;
			Item.createTile = ModContent.TileType<Tiles.BaseItemsGeneratorTile>();
			generatorType = (byte)SatelliteStorage.GeneratorTypes.BaseGenerator;
		}

		public virtual void AddGeneratorRecipes()
        {
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<Items.QuartzModule>(), 1)
			.AddIngredient(ModContent.ItemType<Items.QuartzShard>(), 25)
			.AddTile(TileID.Anvils)
			.Register();
		}
	}
}
