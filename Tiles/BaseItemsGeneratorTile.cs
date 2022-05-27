using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;

namespace SatelliteStorage.Tiles
{
    class BaseItemsGeneratorTile : ModTile
    {
		public byte generatorType;
		public int itemDrop;

		public override void SetStaticDefaults()
		{
			// Properties
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileOreFinderPriority[Type] = 500;
			TileID.Sets.DisableSmartCursor[Type] = true;

			// Names
			ContainerName.SetDefault(Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChest"));

			ModTranslation name = CreateMapEntryName();
			name.SetDefault(Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChest"));
			AddMapEntry(new Color(108, 65, 138), name, MapName);

			// Placement

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.CoordinateHeights = new int[3] { 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;

			AnimationFrameHeight = 54;

			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;

			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);

			TileObjectData.addTile(Type);

			SetGeneratorDefaults();
		}

		public virtual void SetGeneratorDefaults()
        {
			itemDrop = ModContent.ItemType<Items.DriveChestItem>();
			generatorType = (byte)SatelliteStorage.GeneratorTypes.BaseGenerator;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}

		public static string MapName(string name, int i, int j)
		{
			return Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChest");
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			if (Main.netMode == NetmodeID.SinglePlayer || Main.netMode == NetmodeID.Server)
			{
				DriveSystem.DriveChestSystem.SubGenerator(generatorType);
			}

			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, itemDrop);
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
			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				DriveSystem.DriveChestSystem.AddGenerator(generatorType);
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
            {
				Player player = Main.LocalPlayer;
				ModPacket packet = SatelliteStorage.instance.GetPacket();
				packet.Write((byte)SatelliteStorage.MessageType.AddDriveChestItem);
				packet.Write((byte)player.whoAmI);
				
				packet.Write((byte)generatorType);
				packet.Write((byte)1);

				packet.Send();
				packet.Close();
			}

			base.PlaceInWorld(i, j, item);
		}

		/*
		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		{
			// Tweak the frame drawn by x position so tiles next to each other are off-sync and look much more interesting
			int uniqueAnimationFrame = Main.tileFrame[Type] + i;
			
			if (i % 1 == 0)
				uniqueAnimationFrame += 1;
			if (i % 2 == 0)
				uniqueAnimationFrame += 1;
			if (i % 3 == 0)
				uniqueAnimationFrame += 1;
			
			uniqueAnimationFrame %= 3;

			frameYOffset = uniqueAnimationFrame * 54;
		}
		*/

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Texture2D texture = ModContent.Request<Texture2D>("SatelliteStorage/Tiles/" + Name).Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

			int height = tile.TileFrameY % AnimationFrameHeight == 54 ? 16 : 16;

			int frameYOffset = Main.tileFrame[Type] * AnimationFrameHeight;

			/*
			int uniqueAnimationFrame = Main.tileFrame[Type] + i;

			if (i % 2 == 0)
				uniqueAnimationFrame += 1;
			if (i % 3 == 0)
				uniqueAnimationFrame += 1;

			uniqueAnimationFrame %= 4;

			frameYOffset = uniqueAnimationFrame * AnimationFrameHeight;
			*/

			spriteBatch.Draw(
				texture,
				new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
				new Rectangle(tile.TileFrameX, tile.TileFrameY + frameYOffset, 16, height),
				Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);

			return false;
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			if (++frameCounter >= 10) {
				frameCounter = 0;
				frame = ++frame % 4;
			}
		}
	}
}
