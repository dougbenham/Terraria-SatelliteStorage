using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.Localization;
using System;

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
				.AddIngredient(ModContent.ItemType<Items.DriveChestItem>(), 1)
				.AddIngredient(ItemID.FallenStar, 10)
				.AddIngredient(ItemID.MagicMirror, 1)
				.AddTile(TileID.Anvils)
				.Register();

			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Items.DriveChestItem>(), 1)
				.AddIngredient(ItemID.FallenStar, 10)
				.AddIngredient(ItemID.IceMirror, 1)
				.AddTile(TileID.Anvils)
				.Register();
		}

        public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				if (cooldownTime + 500 > new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()) return true;
				if (!DriveSystem.DriveChestSystem.isSputnikPlaced)
				{
					Main.NewText(Language.GetTextValue("Mods.SatelliteStorage.Common.CantUseWithoutSputnik"), new Color(173, 57, 71));
					cooldownTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
					return true;
				}
				if (!SatelliteStorage.GetUIState((int)UI.UITypes.DriveChest)) return DriveSystem.DriveChestSystem.RequestOpenDriveChest();
			}
			cooldownTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
			return true;
		}

    }
}
