using SatelliteStorage.DriveSystem;
using SatelliteStorage.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SatelliteStorage.Tiles
{
    class SputnikTile : ModTile
    {
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileOreFinderPriority[Type] = 500;
			TileID.Sets.DisableSmartCursor[Type] = true;

			DustType = DustID.Firefly;
			
			ContainerName.SetDefault(Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChest"));

			var name = CreateMapEntryName();
			name.SetDefault(Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChest"));
			AddMapEntry(new(108, 65, 138), name, MapName);
			
			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.newTile.Origin = new(2, 1);
			TileObjectData.newTile.CoordinateHeights = new int[3] { 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new(AnchorType.EmptyTile, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);
		}

        public override bool CanPlace(int i, int j)
        {
			if (!Main.LocalPlayer.ZoneNormalSpace) return false;
			if (DriveChestSystem.IsSputnikPlaced) return false;
            return base.CanPlace(i, j);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}

		public static string MapName(string name, int i, int j)
		{
			return Language.GetTextValue("Mods.SatelliteStorage.ItemName.SputnikItem");
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<SputnikItem>());

			DriveChestSystem.IsSputnikPlaced = false;
			if (Main.netMode == NetmodeID.Server)
				SatelliteStorage.Instance.SendSputnikState();
		}

		public override void MouseOver(int i, int j)
		{
			var player = Main.LocalPlayer;
			player.cursorItemIconText = Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChest");
			player.noThrow = 2;
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);
			var player = Main.LocalPlayer;
			if (player.cursorItemIconText == "")
			{
				player.cursorItemIconEnabled = false;
				player.cursorItemIconID = 0;
			}
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			var tile = Main.tile[i, j];
			if (tile.TileFrameX == 0)
			{
				r = 1f;
				g = 0.75f;
				b = 1f;
			}
		}

		public override void PlaceInWorld(int i, int j, Item item)
		{
			DriveChestSystem.IsSputnikPlaced = true;
			SatelliteStorage.Instance.SendSputnikState();
			base.PlaceInWorld(i, j, item);
		}
	}
}
