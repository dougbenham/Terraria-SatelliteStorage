using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SatelliteStorage.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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
					OverrideColor = ItemRarity.GetColor(itm.rare)
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
			Item.createTile = ModContent.TileType<BaseItemsGeneratorTile>();
			generatorType = (byte)SatelliteStorage.GeneratorTypes.BaseGenerator;
		}

		public virtual void AddGeneratorRecipes()
        {
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<QuartzModule>())
			.AddIngredient(ModContent.ItemType<QuartzShard>(), 25)
			.AddTile(TileID.Anvils)
			.Register();
		}
	}
}
