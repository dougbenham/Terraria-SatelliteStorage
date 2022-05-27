using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    class BaseItemsGeneratorTile : ModTile
    {
		public byte GeneratorType;

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

			var name = CreateMapEntryName();
			name.SetDefault(Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChest"));
			AddMapEntry(new(108, 65, 138), name, MapName);

			// Placement

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new(1, 1);
			TileObjectData.newTile.CoordinateHeights = new int[3] { 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;

			AnimationFrameHeight = 54;

			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;

			TileObjectData.newTile.AnchorBottom = new(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);

			TileObjectData.addTile(Type);

			SetGeneratorDefaults();
		}

		public virtual void SetGeneratorDefaults()
        {
			ItemDrop = ModContent.ItemType<DriveChestItem>();
			GeneratorType = (byte)SatelliteStorage.GeneratorTypes.BaseGenerator;
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
				DriveChestSystem.SubGenerator(GeneratorType);
			}

			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ((ModBlockType) this).ItemDrop);
		}

		public override void MouseOver(int i, int j)
		{

			var player = Main.LocalPlayer;


			player.cursorItemIconText = Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChest");

			player.noThrow = 2;
			
			//player.cursorItemIconEnabled = true;
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
			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				DriveChestSystem.AddGenerator(GeneratorType);
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
            {
				var player = Main.LocalPlayer;
				var packet = SatelliteStorage.Instance.GetPacket();
				packet.Write((byte)SatelliteStorage.MessageType.AddDriveChestItem);
				packet.Write((byte)player.whoAmI);
				
				packet.Write(GeneratorType);
				packet.Write((byte)1);

				packet.Send();
				packet.Close();
			}

			base.PlaceInWorld(i, j, item);
		}
		
		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			var tile = Main.tile[i, j];
			var texture = ModContent.Request<Texture2D>("SatelliteStorage/Tiles/" + Name).Value;
			var zero = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);

			var height = tile.TileFrameY % AnimationFrameHeight == 54 ? 16 : 16;

			var frameYOffset = Main.tileFrame[Type] * AnimationFrameHeight;
			
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
