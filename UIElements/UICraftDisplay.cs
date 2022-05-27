using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.UI;
using SatelliteStorage.DriveSystem;
using Terraria.GameContent.UI.Elements;
using Terraria;

namespace SatelliteStorage.UIElements
{
    class UICraftDisplay : UIElement
    {
		private int _lastCheckedVersionForEdits = -1;
		private UIElement _containerInfinites;
		private UIElement _containerSacrifice;
		private bool _hovered;
		private UISearchBar _searchBar;
		private List<int> _itemIdsAvailableTotal;
		private List<int> _itemIdsAvailableToShow;

		private DynamicItemCollection _itemGrid;
		private CreativeUnlocksTracker _lastTrackerCheckedForEdits;
		private bool _showSacrificesInsteadOfInfinites;
		private EntrySorter<int, ICreativeItemSortStep> _sorter;
		private bool _didClickSomething;
		private bool _didClickSearchBar;

		public Action<int> onRecipeChoosen;

		public int selectedRecipe = -1;

		public static bool hidden = true;

		public UICraftDisplay(UIState uiStateThatHoldsThis)
		{
			_itemIdsAvailableTotal = new List<int>();
			_itemIdsAvailableToShow = new List<int>();

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
			UIElement uIElement = new UIElement
			{
				Width = StyleDimension.Fill,
				Height = StyleDimension.Fill
			};
			uIElement.SetPadding(0f);
			_containerInfinites = uIElement;
			UIElement uIElement2 = new UIElement
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
			UIPanel uIPanel = new UIPanel();
			SetBasicSizesForCreativeSacrificeOrInfinitesPanel(uIPanel);
			uIPanel.BackgroundColor *= 0.8f;
			uIPanel.BorderColor *= 0.8f;
			return uIPanel;
		}

        public override void Draw(SpriteBatch spriteBatch)
        {
			if (hidden) return;
            base.Draw(spriteBatch);
        }

        private static void SetBasicSizesForCreativeSacrificeOrInfinitesPanel(UIElement element)
		{
			element.Width = new StyleDimension(0f, 1f);
			element.Height = new StyleDimension(-38f, 1f);
			element.Top = new StyleDimension(38f, 0f);
		}

		private void SelectItem(UIMouseEvent evt, UIElement listeningElement)
		{
			if (_itemGrid.hoverItemIndex <= -1) return;
			DriveItem driveItem = _itemGrid._driveItems[_itemGrid.hoverItemIndex];
			if (driveItem == null) return;
			//SatelliteStorage.Debug("driveItem: " + driveItem.type + ", stack: " + driveItem.stack);
			Vector2 mousePos = evt.MousePosition;


			Player player = Main.LocalPlayer;
			Item mouseItem = player.inventory[58];

			if (!mouseItem.IsAir || !Main.mouseItem.IsAir) return;
			if (selectedRecipe == driveItem.recipe) return;
			selectedRecipe = driveItem.recipe;
			UpdateContents();
			onRecipeChoosen(driveItem.recipe);
			return;
		}

		private void BuildInfinitesMenuContents(UIElement totalContainer)
		{
			UIPanel uIPanel = CreateBasicPanel();
			totalContainer.Append(uIPanel);
			uIPanel.OnUpdate += Hover_OnUpdate;
			uIPanel.OnMouseOver += Hover_OnMouseOver;
			uIPanel.OnMouseOut += Hover_OnMouseOut;
			DynamicItemCollection item = (_itemGrid = new DynamicItemCollection());

			item.OnMouseDown += (UIMouseEvent evt, UIElement listeningElement) =>
			{
				SelectItem(evt, listeningElement);
				return;
			};


			UIElement uIElement = new UIElement
			{
				Height = new StyleDimension(24f, 0f),
				Width = new StyleDimension(0f, 1f)
			};
			uIElement.SetPadding(0f);
			uIPanel.Append(uIElement);

			UIList uIList = new UIList
			{
				Width = new StyleDimension(-25f, 1f),
				Height = new StyleDimension(-28f, 1f),
				VAlign = 1f,
				HAlign = 0f
			};
			uIPanel.Append(uIList);
			float num = 4f;
			UIScrollbar uIScrollbar = new UIScrollbar
			{
				Height = new StyleDimension(-28f - num * 2f, 1f),
				Top = new StyleDimension(0f - num, 0f),
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
			UpdateItemsTypes();
			_itemIdsAvailableToShow.Clear();

			_itemIdsAvailableToShow.AddRange(_itemIdsAvailableTotal);
			_itemIdsAvailableToShow.Sort(_sorter);

			List<DriveItem> recipeItems = new List<DriveItem>();
			bool hasRecipe = false;
			foreach (int key in DriveChestSystem.availableRecipes.Keys)
			{
				Item item = DriveChestSystem.availableRecipes[key].createItem;
				DriveItem driveItem = new DriveItem();
				driveItem.type = item.type;
				driveItem.stack = item.stack;
				driveItem.prefix = item.prefix;
				driveItem.recipe = key;
				driveItem.context = 26;
				if (selectedRecipe == key)
				{
					hasRecipe = true;
					driveItem.context = 34;
				}
				recipeItems.Add(driveItem);
			}

			if (!hasRecipe) UICraftRecipe.hidden = true;
			else UICraftRecipe.hidden = false;

			_itemGrid.SetContentsToShow(_itemIdsAvailableToShow, recipeItems);
			hidden = false;
			if (DriveChestSystem.availableRecipes.Keys.Count <= 0)
			{
				hidden = true;
				UICraftRecipe.hidden = true;
			}
		}


		public void UpdateItemsTypes()
		{
			List<int> types = new List<int>();
			foreach (int key in DriveChestSystem.availableRecipes.Keys)
            {
				Item item = DriveChestSystem.availableRecipes[key].createItem;
				types.Add(item.type);
			}

			_itemIdsAvailableTotal.Clear();
			_itemIdsAvailableTotal.AddRange(types);
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
			CreativeUnlocksTracker localPlayerCreativeTracker = Main.LocalPlayerCreativeTracker;
			if (_lastTrackerCheckedForEdits != localPlayerCreativeTracker)
			{
				_lastTrackerCheckedForEdits = localPlayerCreativeTracker;
				_lastCheckedVersionForEdits = -1;
			}
			int lastEditId = localPlayerCreativeTracker.ItemSacrifices.LastEditId;
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
