﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SatelliteStorage.Tiles
{
    class SputnikTile : ModTile
    {
		public override void SetStaticDefaults()
		{
			// Properties
			//Main.tileSpelunker[Type] = true;
			//Main.tileContainer[Type] = true;
			Main.tileLighted[Type] = true;
			//Main.tileShine2[Type] = true;
			//Main.tileShine[Type] = 1200;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileOreFinderPriority[Type] = 500;
			//TileID.Sets.HasOutlines[Type] = true;
			//TileID.Sets.BasicChest[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			DustType = DustID.Firefly;

			// Names
			ContainerName.SetDefault(Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChest"));

			ModTranslation name = CreateMapEntryName();
			name.SetDefault(Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChest"));
			AddMapEntry(new Color(108, 65, 138), name, MapName);
			
			//name = CreateMapEntryName(Name + "_Locked"); // With multiple map entries, you need unique translation keys.
			//name.SetDefault("Locked Example Chest");
			//AddMapEntry(new Color(0, 141, 63), name, MapName);

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.newTile.Origin = new Point16(2, 1);
			TileObjectData.newTile.CoordinateHeights = new int[3] { 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;

			//TileObjectData.newTile.CoordinatePadding = 2;
			//TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;

			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.EmptyTile, TileObjectData.newTile.Width, 0);

			TileObjectData.addTile(Type);
		}

        public override bool CanPlace(int i, int j)
        {
			if (!Main.LocalPlayer.ZoneNormalSpace) return false;
			if (DriveSystem.DriveChestSystem.isSputnikPlaced) return false;
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
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.SputnikItem>());

			DriveSystem.DriveChestSystem.isSputnikPlaced = false;
			SatelliteStorage.SyncIsSputnikPlacedToClients();
		}

		private void SendSyncSputnikState()
        {
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				Player player = Main.LocalPlayer;
				ModPacket packet = SatelliteStorage.instance.GetPacket();
				packet.Write((byte)SatelliteStorage.MessageType.SetSputnikState);
				packet.Write((byte)player.whoAmI);
				packet.Write((byte)(DriveSystem.DriveChestSystem.isSputnikPlaced ? 1 : 0));
				packet.Send();
				packet.Close();
			}
		}

		public override bool RightClick(int i, int j)
		{

			return true;
		}

		public override void MouseOver(int i, int j)
		{

			Player player = Main.LocalPlayer;


			player.cursorItemIconText = Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChest");

			player.noThrow = 2;
			//player.cursorItemIconEnabled = true;
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			if (player.cursorItemIconText == "")
			{
				player.cursorItemIconEnabled = false;
				player.cursorItemIconID = 0;
			}
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX == 0)
			{
				r = 1f;
				g = 0.75f;
				b = 1f;
			}
		}

		public override void PlaceInWorld(int i, int j, Item item)
		{
			DriveSystem.DriveChestSystem.isSputnikPlaced = true;
			SendSyncSputnikState();
			base.PlaceInWorld(i, j, item);
		}
	}
}
