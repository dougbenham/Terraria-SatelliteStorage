using System;
using Microsoft.Xna.Framework;
using SatelliteStorage.DriveSystem;
using SatelliteStorage.UI;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SatelliteStorage.Items
{
    class DriveChestRemoteItem : ModItem
	{
		private long cooldownTime;
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 40;
			Item.scale = 0.8f;
			Item.maxStack = 1;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.rare = ItemRarityID.Blue;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = false;
			Item.value = 500;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<DriveChestItem>())
				.AddIngredient(ItemID.FallenStar, 10)
				.AddIngredient(ItemID.MagicMirror)
				.AddTile(TileID.Anvils)
				.Register();

			CreateRecipe()
				.AddIngredient(ModContent.ItemType<DriveChestItem>())
				.AddIngredient(ItemID.FallenStar, 10)
				.AddIngredient(ItemID.IceMirror)
				.AddTile(TileID.Anvils)
				.Register();
		}

        public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				if (cooldownTime + 500 > new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()) return true;
				if (!DriveChestSystem.IsSputnikPlaced)
				{
					Main.NewText(Language.GetTextValue("Mods.SatelliteStorage.Common.CantUseWithoutSputnik"), new Color(173, 57, 71));
					cooldownTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
					return true;
				}
				if (!SatelliteStorage.GetUIState((int)UITypes.DriveChest)) return DriveChestSystem.RequestOpenDriveChest();
			}
			cooldownTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
			return true;
		}

    }
}
