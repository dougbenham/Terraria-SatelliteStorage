using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SatelliteStorage.DriveSystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SatelliteStorage.UIElements
{
    class UICraftRecipe : UIElement
    {
		private int _lastCheckedVersionForEdits = -1;
		private UIElement _containerInfinites;
		private UIElement _containerSacrifice;
		private bool _hovered;
		private UISearchBar _searchBar;
		private readonly List<int> _itemIdsAvailableTotal;
		private readonly List<int> _itemIdsAvailableToShow;

		private DynamicItemCollection _itemGrid;
		private CreativeUnlocksTracker _lastTrackerCheckedForEdits;
		private bool _showSacrificesInsteadOfInfinites;
		private EntrySorter<int, ICreativeItemSortStep> _sorter;
		private bool _didClickSomething;
		private bool _didClickSearchBar;

		private int currentRecipe = -1;

		public static bool hidden = true;

		public UICraftRecipe(UIState uiStateThatHoldsThis)
		{
			_itemIdsAvailableTotal = new();
			_itemIdsAvailableToShow = new();

			BuildPage();
		}


		public void RebuildPage()
		{
			UpdateContents();
			//BuildPage();
		}


		private void BuildPage()
		{
			_lastCheckedVersionForEdits = -1;
			RemoveAllChildren();
			SetPadding(0f);
			var uIElement = new UIElement
			{
				Width = StyleDimension.Fill,
				Height = StyleDimension.Fill
			};
			uIElement.SetPadding(0f);
			_containerInfinites = uIElement;
			var uIElement2 = new UIElement
			{
				Width = StyleDimension.Fill,
				Height = StyleDimension.Fill
			};
			uIElement2.SetPadding(0f);
			_containerSacrifice = uIElement2;

			BuildInfinitesMenuContents(uIElement);
			//BuildSacrificeMenuContents(uIElement2);

			UpdateContents();
			base.OnUpdate += UICreativeInfiniteItemsDisplay_OnUpdate;
		}

		private static UIPanel CreateBasicPanel()
		{
			var uIPanel = new UIPanel();
			SetBasicSizesForCreativeSacrificeOrInfinitesPanel(uIPanel);
			uIPanel.BackgroundColor *= 0.8f;
			uIPanel.BorderColor *= 0.8f;
			return uIPanel;
		}

		private static void SetBasicSizesForCreativeSacrificeOrInfinitesPanel(UIElement element)
		{
			element.Width = new(0f, 1f);
			element.Height = new(-38f, 1f);
			element.Top = new(38f, 0f);
		}

		private void BuildInfinitesMenuContents(UIElement totalContainer)
		{
			var uIPanel = CreateBasicPanel();
			totalContainer.Append(uIPanel);
			uIPanel.OnUpdate += Hover_OnUpdate;
			uIPanel.OnMouseOver += Hover_OnMouseOver;
			uIPanel.OnMouseOut += Hover_OnMouseOut;
			var item = (_itemGrid = new());

			item.OnMouseDown += (UIMouseEvent evt, UIElement listeningElement) =>
			{
				//TakeItem(evt, listeningElement, 0);
				return;
			};

			item.OnRightMouseDown += (UIMouseEvent evt, UIElement listeningElement) =>
			{
				//TakeItem(evt, listeningElement, 1);
				return;
			};

			var uIElement = new UIElement
			{
				Height = new(24f, 0f),
				Width = new(0f, 1f)
			};
			uIElement.SetPadding(0f);
			uIPanel.Append(uIElement);

			var uIList = new UIList
			{
				Width = new(-25f, 1f),
				Height = new(-28f, 1f),
				VAlign = 1f,
				HAlign = 0f
			};
			uIPanel.Append(uIList);
			var num = 4f;
			var uIScrollbar = new UIScrollbar
			{
				Height = new(-28f - num * 2f, 1f),
				Top = new(0f - num, 0f),
				VAlign = 1f,
				HAlign = 1f
			};
			uIPanel.Append(uIScrollbar);
			uIList.SetScrollbar(uIScrollbar);
			uIList.Add(item);
		}

		private void Hover_OnUpdate(UIElement affectedElement)
		{
			if (_hovered)
			{
				Main.LocalPlayer.mouseInterface = true;
			}
		}

		private void Hover_OnMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			_hovered = false;
		}

		private void Hover_OnMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			_hovered = true;
		}

		private void UpdateContents()
		{
			if (currentRecipe <= -1) return;
			var recipe = Main.recipe[currentRecipe];
			var types = new List<int>();

			recipe.requiredItem.ForEach(item =>
			{
				types.Add(item.type);
			});

			_itemIdsAvailableTotal.Clear();
			_itemIdsAvailableTotal.AddRange(types);

			
			_itemIdsAvailableToShow.Clear();

			_itemIdsAvailableToShow.AddRange(_itemIdsAvailableTotal);
			_itemIdsAvailableToShow.Sort(_sorter);

			var driveItems = new List<DriveItem>();
			recipe.requiredItem.ForEach(item =>
			{
				var driveItem = DriveItem.FromItem(item);
				driveItem.context = 26;
				driveItems.Add(driveItem);
			});

			_itemGrid.SetContentsToShow(_itemIdsAvailableToShow, driveItems);
		}

		public void SetRecipe(int recipe)
        {
			currentRecipe = recipe;
			UpdateContents();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (hidden) return;
			base.Draw(spriteBatch);
		}

		public override void Click(UIMouseEvent evt)
		{
			base.Click(evt);
			AttemptStoppingUsingSearchbar(evt);
		}

		private void AttemptStoppingUsingSearchbar(UIMouseEvent evt)
		{
			_didClickSomething = true;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			if (_didClickSomething && !_didClickSearchBar && _searchBar != null && _searchBar.IsWritingText)
			{
				_searchBar.ToggleTakingText();
			}
			_didClickSomething = false;
			_didClickSearchBar = false;
		}

		public override void Recalculate()
		{
			base.Recalculate();
			if (_itemGrid != null) _itemGrid.Recalculate();
		}

		private void UICreativeInfiniteItemsDisplay_OnUpdate(UIElement affectedElement)
		{
			RemoveAllChildren();
			var localPlayerCreativeTracker = Main.LocalPlayerCreativeTracker;
			if (_lastTrackerCheckedForEdits != localPlayerCreativeTracker)
			{
				_lastTrackerCheckedForEdits = localPlayerCreativeTracker;
				_lastCheckedVersionForEdits = -1;
			}
			var lastEditId = localPlayerCreativeTracker.ItemSacrifices.LastEditId;
			if (_lastCheckedVersionForEdits != lastEditId)
			{
				_lastCheckedVersionForEdits = lastEditId;
				UpdateContents();
			}
			if (_showSacrificesInsteadOfInfinites)
			{
				Append(_containerSacrifice);
			}
			else
			{
				Append(_containerInfinites);
			}
		}
	}
}
