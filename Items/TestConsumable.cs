namespace SatelliteStorage.Items
{
	/*
    class TestConsumable : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Test Consumable");
			Tooltip.SetDefault("Test Consumable");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 20;
			Item.value = 100;
			Item.rare = ItemRarityID.Blue;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
		}

		public override bool CanUseItem(Player player)
		{
			// If you decide to use the below UseItem code, you have to include !NPC.AnyNPCs(id), as this is also the check the server does when receiving MessageID.SpawnBoss
			return true;
			//return Main.hardMode && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && !NPC.AnyNPCs(NPCID.Plantera);
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{

				for (int i = 0; i < 4500; i++)
                {
					DriveSystem.DriveItem item = new DriveSystem.DriveItem();
					item.type = i;
					item.stack = 9999;
					DriveSystem.DriveChestSystem.AddItem(item);
				}


				/*
				Item.NewItem(player.GetItemSource_OpenItem(ItemID.DrillContainmentUnit), player.getRect(), ItemID.DrillContainmentUnit, Main.rand.Next(1, 4));
				for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 6E-05); k++)
				{
					// The inside of this for loop corresponds to one single splotch of our Ore.
					// First, we randomly choose any coordinate in the world by choosing a random x and y value.
					int x = WorldGen.genRand.Next(0, Main.maxTilesX);

					// WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.
					int y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow, Main.maxTilesY);


					// Then, we call WorldGen.TileRunner with random "strength" and random "steps", as well as the Tile we wish to place.
					// Feel free to experiment with strength and step to see the shape they generate.
					WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 6), ModContent.TileType<Tiles.QuartzOre>());

					// Alternately, we could check the tile already present in the coordinate we are interested.
					// Wrapping WorldGen.TileRunner in the following condition would make the ore only generate in Snow.
					// Tile tile = Framing.GetTileSafely(x, y);
					// if (tile.active() && tile.type == TileID.SnowBlock)
					// {
					// 	WorldGen.TileRunner(.....);
					// }
				}
				/
			}

			return true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.DirtBlock, 1)
				.Register();
		}
	}
	*/
}
