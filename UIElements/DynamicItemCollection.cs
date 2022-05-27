using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SatelliteStorage.DriveSystem;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace SatelliteStorage.UIElements
{

	public class DynamicItemCollection : UIElement
	{
		private readonly List<int> _itemIdsAvailableToShow = new();

		private readonly List<int> _itemIdsToLoadTexturesFor = new();

		public Dictionary<int, DriveItem> _driveItems = new();
		
		private int _itemsPerLine;

		private const int sizePerEntryX = 44;

		private const int sizePerEntryY = 44;

		private readonly List<SnapPoint> _dummySnapPoints = new();

		public int hoverItemIndex = -1;

		public DynamicItemCollection()
		{
			Width = new(0f, 1f);
			HAlign = 0.5f;
			UpdateSize();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			hoverItemIndex = -1;
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			Main.inventoryScale = 0.846153855f;
			GetGridParameters(out var startX, out var startY, out var startItemIndex, out var endItemIndex);
			var num = _itemsPerLine;
			for (var i = startItemIndex; i < endItemIndex; i++)
			{
				var num2 = _itemIdsAvailableToShow[i];
				var driveItem = _driveItems.ContainsKey(i) ? _driveItems[i] : new();
				var itemSlotHitbox = GetItemSlotHitbox(startX, startY, startItemIndex, i);
				var inv = ContentSamples.ItemsByType[num2];
				inv.prefix = driveItem.prefix;
				var context = driveItem.context > 0 ? driveItem.context : 29;
				if ((int)TextureAssets.Item[num2].State == 0)
				{
					num--;
				}
				
				var cREATIVE_ItemSlotShouldHighlightAsSelected = false;
				if (IsMouseHovering && itemSlotHitbox.Contains(Main.MouseScreen.ToPoint()) && !PlayerInput.IgnoreMouseInterface)
				{
					Main.LocalPlayer.mouseInterface = true;
					hoverItemIndex = i;
					ItemSlot.OverrideHover(ref inv, context);
					//ItemSlot.LeftClick(ref inv, context);
					//ItemSlot.RightClick(ref inv, context);
					ItemSlot.MouseHover(ref inv, context);
					
					cREATIVE_ItemSlotShouldHighlightAsSelected = true;
				}
				UILinkPointNavigator.Shortcuts.CREATIVE_ItemSlotShouldHighlightAsSelected = cREATIVE_ItemSlotShouldHighlightAsSelected;
				ItemSlot.Draw(spriteBatch, ref inv, context, itemSlotHitbox.TopLeft());
				if (driveItem.stack > 1) Terraria.Utils.DrawBorderString(spriteBatch, driveItem.stackText, itemSlotHitbox.BottomLeft() + new Vector2(9, -20), Color.White, 0.7f);
				if (num <= 0)
				{
					break;
				}
			}
			while (_itemIdsToLoadTexturesFor.Count > 0 && num > 0)
			{
				var num3 = _itemIdsToLoadTexturesFor[0];
				_itemIdsToLoadTexturesFor.RemoveAt(0);
				if ((int)TextureAssets.Item[num3].State == 0)
				{
					Main.instance.LoadItem(num3);
					num -= 4;
				}
			}
		}

		private Rectangle GetItemSlotHitbox(int startX, int startY, int startItemIndex, int i)
		{
			var num = i - startItemIndex;
			var num2 = num % _itemsPerLine;
			var num3 = num / _itemsPerLine;
			return new(startX + num2 * 44, startY + num3 * 44, 44, 44);
		}

		private void GetGridParameters(out int startX, out int startY, out int startItemIndex, out int endItemIndex)
		{
			var rectangle = GetDimensions().ToRectangle();
			var viewCullingArea = Parent.GetViewCullingArea();
			var x = rectangle.Center.X;
			startX = x - (int)(44 * _itemsPerLine * 0.5f);
			startY = rectangle.Top;
			startItemIndex = 0;
			endItemIndex = _itemIdsAvailableToShow.Count;
			var num = (Math.Min(viewCullingArea.Top, rectangle.Top) - viewCullingArea.Top) / 44;
			startY += -num * 44;
			startItemIndex += -num * _itemsPerLine;
			var num2 = (int)Math.Ceiling(viewCullingArea.Height / 44f) * _itemsPerLine;
			if (endItemIndex > num2 + startItemIndex + _itemsPerLine)
			{
				endItemIndex = num2 + startItemIndex + _itemsPerLine;
			}
		}

		public override void Recalculate()
		{
			base.Recalculate();
			UpdateSize();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			if (IsMouseHovering)
			{
				Main.LocalPlayer.mouseInterface = true;
			}
		}

		private class DriveItemData
		{
			public readonly List<DriveItem> items = new();
		}

		public void SetContentsToShow(List<int> itemIdsToShow, List<DriveItem> driveItems)
		{
			_itemIdsAvailableToShow.Clear();
			_itemIdsToLoadTexturesFor.Clear();
			_itemIdsAvailableToShow.AddRange(itemIdsToShow);
			_itemIdsToLoadTexturesFor.AddRange(itemIdsToShow);
			_driveItems.Clear();

			var dictOfDriveItems = new Dictionary<int, DriveItemData>();

			for (var i = 0; i < driveItems.Count; i++)
            {
				var item = driveItems[i];
				if (!dictOfDriveItems.ContainsKey(item.type)) dictOfDriveItems.Add(item.type, new());
				dictOfDriveItems[item.type].items.Add(item);
			}

			for(var i = 0; i < _itemIdsAvailableToShow.Count; i++)
            {
				var type = _itemIdsAvailableToShow[i];
				var data = dictOfDriveItems.ContainsKey(type) ? dictOfDriveItems[type] : null;
				
				if (data != null)
                {
					_driveItems.Add(i, data.items[0]);
					data.items.RemoveAt(0);
                }
			}

			UpdateSize();
			
		}

		public int GetItemsPerLine()
		{
			return _itemsPerLine;
		}

		public override List<SnapPoint> GetSnapPoints()
		{
			var list = new List<SnapPoint>();
			GetGridParameters(out var startX, out var startY, out var startItemIndex, out var endItemIndex);
			_ = _itemsPerLine;
			var viewCullingArea = Parent.GetViewCullingArea();
			var num = endItemIndex - startItemIndex;
			while (_dummySnapPoints.Count < num)
			{
				_dummySnapPoints.Add(new("CreativeInfinitesSlot", 0, Vector2.Zero, Vector2.Zero));
			}
			var num2 = 0;
			var vector = GetDimensions().Position();
			for (var i = startItemIndex; i < endItemIndex; i++)
			{
				var center = GetItemSlotHitbox(startX, startY, startItemIndex, i).Center;
				if (viewCullingArea.Contains(center))
				{
					var snapPoint = _dummySnapPoints[num2];
					snapPoint.ThisIsAHackThatChangesTheSnapPointsInfo(Vector2.Zero, center.ToVector2() - vector, num2);
					snapPoint.Calculate(this);
					num2++;
					list.Add(snapPoint);
				}
			}
			foreach (var element in Elements)
			{
				list.AddRange(element.GetSnapPoints());
			}
			return list;
		}

		public void UpdateSize()
		{
			var num = (_itemsPerLine = GetDimensions().ToRectangle().Width / 44);
			var num2 = (int)Math.Ceiling(_itemIdsAvailableToShow.Count / (float)num);
			MinHeight.Set(44 * num2, 0f);
		}
	}
}