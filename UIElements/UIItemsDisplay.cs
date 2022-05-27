using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SatelliteStorage.DriveSystem;
using SatelliteStorage.UI;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace SatelliteStorage.UIElements
{
	public class UiItemsDisplay : UIElement
	{
		private readonly List<int> _itemIdsAvailableTotal;

		private readonly List<int> _itemIdsAvailableToShow;

		private CreativeUnlocksTracker _lastTrackerCheckedForEdits;

		private int _lastCheckedVersionForEdits = -1;

		private UISearchBar _searchBar;

		private UIPanel _searchBoxPanel;

		private UIState _parentUiState;

		private string _searchString;

		private DynamicItemCollection _itemGrid;

		private readonly EntryFilterer<Item, IItemEntryFilter> _filterer;

		private readonly EntrySorter<int, ICreativeItemSortStep> _sorter;

		private UIElement _containerInfinites;

		private UIElement _containerSacrifice;

		private bool _showSacrificesInsteadOfInfinites;

		private int _sacrificeAnimationTimeLeft;
		
		private bool _hovered;

		private bool _didClickSomething;

		private bool _didClickSearchBar;
		private UICreativeItemsInfiniteFilteringOptions _uICreativeItemsInfiniteFilteringOptions;

		public UiItemsDisplay(UIState uiStateThatHoldsThis)
		{
			_parentUiState = uiStateThatHoldsThis;
			_itemIdsAvailableTotal = new();
			_itemIdsAvailableToShow = new();
			_filterer = new();
			var list = new List<IItemEntryFilter>
			{
				new ItemFilters.Weapon(),
				new ItemFilters.Armor(),
				new ItemFilters.Vanity(),
				new ItemFilters.BuildingBlock(),
				new ItemFilters.Furniture(),
				new ItemFilters.Accessories(),
				new ItemFilters.MiscAccessories(),
				new ItemFilters.Consumables(),
				new ItemFilters.Tools(),
				new ItemFilters.Materials()
			};
			var list2 = new List<IItemEntryFilter>();
			list2.AddRange(list);
			list2.Add(new ItemFilters.MiscFallback(list));
			_filterer.AddFilters(list2);
			_filterer.SetSearchFilterObject(new ItemFilters.BySearch());
			_sorter = new();
			_sorter.AddSortSteps(new()
			{
				new SortingSteps.ByCreativeSortingId(),
				new SortingSteps.Alphabetical()
			});


			OnMouseDown += (_, _) => { DriveChestUi.DepositCursorItem(); };

			BuildPage();
		}

		public void RebuildPage()
        {
			UpdateContents();
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
			
			UpdateContents();
			base.OnUpdate += UICreativeInfiniteItemsDisplay_OnUpdate;
		}

		public void UpdateItemsTypes()
		{
			var types = new List<int>();
			DriveChestSystem.GetItems().ForEach(v => types.Add(v.Type));
			_itemIdsAvailableTotal.Clear();
			_itemIdsAvailableTotal.AddRange(types);
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

		private void TakeItem(UIMouseEvent evt, UIElement listeningElement, int clickType)
		{
			if (_itemGrid.HoverItemIndex <= -1) return;
			var driveItem = _itemGrid.DriveItems[_itemGrid.HoverItemIndex];
			if (driveItem == null) return;
			//SatelliteStorage.Debug("driveItem: " + driveItem.type + ", stack: " + driveItem.stack);
			var mousePos = evt.MousePosition;


			var player = Main.LocalPlayer;
			var mouseItem = player.inventory[58];

			var isMouseItemAir = mouseItem.IsAir && Main.mouseItem.IsAir;
			var isMouseItemSame = mouseItem.type == driveItem.Type;
			if (!isMouseItemAir && !isMouseItemSame) return;

			if (clickType == 1)
			{
				if (!isMouseItemAir && !isMouseItemSame) return;

				if (isMouseItemSame)
				{
					if (mouseItem.stack + 1 > mouseItem.maxStack) return;
				}
			} else
            {
				if (!isMouseItemAir) return;
			}

			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				var takeItem = DriveChestSystem.TakeItem(driveItem.Type, driveItem.Prefix, clickType == 1 ? 1 : 0);
				if (takeItem == null) return;

				if (clickType == 1)
				{
					if (isMouseItemAir)
					{
						Main.mouseItem = takeItem.Clone();
					}
					else
					{
						Main.mouseItem.stack += 1;
					}
				} else
                {
					Main.mouseItem = takeItem;
				}

				DriveChestUi.ReloadItems();

				if (clickType == 1)
				{
					SoundEngine.PlaySound(SoundID.MenuTick);
				}
				else
				{
					SoundEngine.PlaySound(SoundID.Grab);
				}
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				if (SatelliteStorage.TakeDriveChestItemSended) return;
				SatelliteStorage.TakeDriveChestItemSended = true;

				var packet = SatelliteStorage.Instance.GetPacket();
				packet.Write((byte)SatelliteStorage.MessageType.TakeDriveChestItem);
				packet.Write((byte)player.whoAmI);
				packet.Write((byte)clickType);
				packet.Write7BitEncodedInt(driveItem.Type);
				packet.Write7BitEncodedInt(driveItem.Prefix);
				packet.Send();
				packet.Close();
			}
		}

		private void BuildInfinitesMenuContents(UIElement totalContainer)
		{
			var uIPanel = CreateBasicPanel();
			totalContainer.Append(uIPanel);
			uIPanel.OnUpdate += Hover_OnUpdate;
			uIPanel.OnMouseOver += Hover_OnMouseOver;
			uIPanel.OnMouseOut += Hover_OnMouseOut;
			var item = (_itemGrid = new());

			item.OnMouseDown += (evt, listeningElement) =>
			{
				TakeItem(evt, listeningElement, 0);
				return;
			};

			item.OnRightMouseDown += (evt, listeningElement) =>
			{
				TakeItem(evt, listeningElement, 1);
				return;
			};

			var uIElement = new UIElement
			{
				Height = new(24f, 0f),
				Width = new(0f, 1f)
			};
			uIElement.SetPadding(0f);
			uIPanel.Append(uIElement);
			AddSearchBar(uIElement);
			_searchBar.SetContents(null, forced: true);
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
			_uICreativeItemsInfiniteFilteringOptions = new(_filterer, "CreativeInfinitesFilter");
			_uICreativeItemsInfiniteFilteringOptions.OnClickingOption += filtersHelper_OnClickingOption;
			_uICreativeItemsInfiniteFilteringOptions.Left = new(20f, 0f);
			totalContainer.Append(_uICreativeItemsInfiniteFilteringOptions);
			_uICreativeItemsInfiniteFilteringOptions.OnUpdate += Hover_OnUpdate;
			_uICreativeItemsInfiniteFilteringOptions.OnMouseOver += Hover_OnMouseOver;
			_uICreativeItemsInfiniteFilteringOptions.OnMouseOut += Hover_OnMouseOut;
		}

		private void UpdateSacrificeAnimation()
		{
			if (_sacrificeAnimationTimeLeft > 0)
			{
				_sacrificeAnimationTimeLeft--;
			}
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
			UpdateSacrificeAnimation();
		}

		private void filtersHelper_OnClickingOption()
		{
			UpdateContents();
		}

		private void UpdateContents()
		{
			UpdateItemsTypes();
			_itemIdsAvailableToShow.Clear();

			_itemIdsAvailableToShow.AddRange(_itemIdsAvailableTotal.Where(x =>
			{
				if (!ContentSamples.ItemsByType.ContainsKey(x)) return false;
				return _filterer.FitsFilter(ContentSamples.ItemsByType[x]);
			}));

			_itemIdsAvailableToShow.Sort(_sorter);
			_itemGrid.SetContentsToShow(_itemIdsAvailableToShow, DriveChestSystem.GetItems());
		}

		private void AddSearchBar(UIElement searchArea)
		{
			var uIImageButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Search", (AssetRequestMode)1))
			{
				VAlign = 0.5f,
				HAlign = 0f
			};
			uIImageButton.OnClick += Click_SearchArea;
			uIImageButton.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Search_Border", (AssetRequestMode)1));
			uIImageButton.SetVisibility(1f, 1f);
			uIImageButton.SetSnapPoint("CreativeInfinitesSearch", 0);
			searchArea.Append(uIImageButton);
			var uIPanel = (_searchBoxPanel = new()
			{
				Width = new(0f - uIImageButton.Width.Pixels - 3f, 1f),
				Height = new(0f, 1f),
				VAlign = 0.5f,
				HAlign = 1f
			});
			uIPanel.BackgroundColor = new(35, 40, 83);
			uIPanel.BorderColor = new(35, 40, 83);
			uIPanel.SetPadding(0f);
			searchArea.Append(uIPanel);
			var uISearchBar = (_searchBar = new(Language.GetText("UI.PlayerNameSlot"), 0.8f)
			{
				Width = new(0f, 1f),
				Height = new(0f, 1f),
				HAlign = 0f,
				VAlign = 0.5f,
				Left = new(0f, 0f),
				IgnoresMouseInteraction = true
			});
			uIPanel.OnClick += Click_SearchArea;
			uIPanel.OnRightClick += RightClick_SearchArea;
			uISearchBar.OnContentsChanged += OnSearchContentsChanged;
			uIPanel.Append(uISearchBar);
			uISearchBar.OnStartTakingInput += OnStartTakingInput;
			uISearchBar.OnEndTakingInput += OnEndTakingInput;
			uISearchBar.OnNeedingVirtualKeyboard += OpenVirtualKeyboardWhenNeeded;
			uISearchBar.OnCancledTakingInput += OnCanceledInput;
			var uIImageButton2 = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/SearchCancel", (AssetRequestMode)1))
			{
				HAlign = 1f,
				VAlign = 0.5f,
				Left = new(-2f, 0f)
			};
			uIImageButton2.OnMouseOver += searchCancelButton_OnMouseOver;
			uIImageButton2.OnClick += searchCancelButton_OnClick;
			uIPanel.Append(uIImageButton2);
		}

		private void searchCancelButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (_searchBar.HasContents)
			{
				_searchBar.SetContents(null, forced: true);
				SoundEngine.PlaySound(SoundID.MenuClose);
			}
			else
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
			}
		}

		private void searchCancelButton_OnMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
		}

		private void OnCanceledInput()
		{
			Main.LocalPlayer.ToggleInv();
		}

		private void Click_SearchArea(UIMouseEvent evt, UIElement listeningElement)
		{
			if (evt.Target.Parent != _searchBoxPanel)
			{
				_searchBar.ToggleTakingText();
				_didClickSearchBar = true;
			}
		}

		private void RightClick_SearchArea(UIMouseEvent evt, UIElement listeningelement)
		{
			_searchBar.SetContents(null, forced: true);
			Click_SearchArea(evt, listeningelement);
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
			if (_didClickSomething && !_didClickSearchBar && _searchBar.IsWritingText)
			{
				_searchBar.ToggleTakingText();
			}
			_didClickSomething = false;
			_didClickSearchBar = false;
		}

		private void OnSearchContentsChanged(string contents)
		{
			_searchString = contents;
			_filterer.SetSearchFilter(contents);
			UpdateContents();
		}

		private void OnStartTakingInput()
		{
			_searchBoxPanel.BorderColor = Main.OurFavoriteColor;
		}

		private void OnEndTakingInput()
		{
			_searchBoxPanel.BorderColor = new(35, 40, 83);
		}

		private void OpenVirtualKeyboardWhenNeeded()
		{
			var maxInputLength = 40;
			var uIVirtualKeyboard = new UIVirtualKeyboard(Language.GetText("UI.PlayerNameSlot").Value, _searchString, OnFinishedSettingName, GoBackHere, 3, allowEmpty: true);
			uIVirtualKeyboard.SetMaxInputLength(maxInputLength);
			IngameFancyUI.OpenUIState(uIVirtualKeyboard);
		}
		
		private void OnFinishedSettingName(string name)
		{
			var contents = name.Trim();
			_searchBar.SetContents(contents);
			GoBackHere();
		}

		private void GoBackHere()
		{
			IngameFancyUI.Close();
			_searchBar.ToggleTakingText();
			Main.CreativeMenu.GamepadMoveToSearchButtonHack = true;
		}
		
        public override void Recalculate()
        {
            base.Recalculate();
			if (_itemGrid != null) _itemGrid.Recalculate();
        }
	}
}