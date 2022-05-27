using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using SatelliteStorage.DriveSystem;
using Terraria.GameContent.UI.Elements;
using Terraria;
using Terraria.ModLoader;

namespace SatelliteStorage.UIElements
{
	public class UIItemsDisplay : UIElement
	{
		public enum InfiniteItemsDisplayPage
		{
			InfiniteItemsPickup,
			InfiniteItemsResearch
		}

		private List<int> _itemIdsAvailableTotal;

		private List<int> _itemIdsAvailableToShow;

		private CreativeUnlocksTracker _lastTrackerCheckedForEdits;

		private int _lastCheckedVersionForEdits = -1;

		private UISearchBar _searchBar;

		private UIPanel _searchBoxPanel;

		private UIState _parentUIState;

		private string _searchString;

		private DynamicItemCollection _itemGrid;

		private EntryFilterer<Item, IItemEntryFilter> _filterer;

		private EntrySorter<int, ICreativeItemSortStep> _sorter;

		private UIElement _containerInfinites;

		private UIElement _containerSacrifice;

		private bool _showSacrificesInsteadOfInfinites;

		public const string SnapPointName_SacrificeSlot = "CreativeSacrificeSlot";

		public const string SnapPointName_SacrificeConfirmButton = "CreativeSacrificeConfirm";

		public const string SnapPointName_InfinitesFilter = "CreativeInfinitesFilter";

		public const string SnapPointName_InfinitesSearch = "CreativeInfinitesSearch";

		public const string SnapPointName_InfinitesItemSlot = "CreativeInfinitesSlot";

		private List<UIImage> _sacrificeCogsSmall = new List<UIImage>();

		private List<UIImage> _sacrificeCogsMedium = new List<UIImage>();

		private List<UIImage> _sacrificeCogsBig = new List<UIImage>();

		private UIImageFramed _sacrificePistons;

		private UIParticleLayer _pistonParticleSystem;

		private Asset<Texture2D> _pistonParticleAsset;

		private int _sacrificeAnimationTimeLeft;

		private bool _researchComplete;

		private bool _hovered;

		private int _lastItemIdSacrificed;

		private int _lastItemAmountWeHad;

		private int _lastItemAmountWeNeededTotal;

		private bool _didClickSomething;

		private bool _didClickSearchBar;
		private UICreativeItemsInfiniteFilteringOptions uICreativeItemsInfiniteFilteringOptions;

		public UIItemsDisplay(UIState uiStateThatHoldsThis)
		{
			_parentUIState = uiStateThatHoldsThis;
			_itemIdsAvailableTotal = new List<int>();
			_itemIdsAvailableToShow = new List<int>();
			_filterer = new EntryFilterer<Item, IItemEntryFilter>();
			List<IItemEntryFilter> list = new List<IItemEntryFilter>
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
			List<IItemEntryFilter> list2 = new List<IItemEntryFilter>();
			list2.AddRange(list);
			list2.Add(new ItemFilters.MiscFallback(list));
			_filterer.AddFilters(list2);
			_filterer.SetSearchFilterObject(new ItemFilters.BySearch());
			_sorter = new EntrySorter<int, ICreativeItemSortStep>();
			_sorter.AddSortSteps(new List<ICreativeItemSortStep>
			{
				new SortingSteps.ByCreativeSortingId(),
				new SortingSteps.Alphabetical()
			});


			OnMouseDown += (UIMouseEvent evt, UIElement listeningElement) =>
			{
				Player player = Main.LocalPlayer;
				Item mouseItem = player.inventory[58];

				if (mouseItem.IsAir || Main.mouseItem.IsAir) return;

				if (Main.netMode == NetmodeID.SinglePlayer)
				{
					if (!DriveChestSystem.AddItem(DriveItem.FromItem(Main.mouseItem))) return;
					UI.DriveChestUI.ReloadItems();

					mouseItem.TurnToAir();
					Main.mouseItem.TurnToAir();
					SoundEngine.PlaySound(7);
				}

				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					if (SatelliteStorage.AddDriveChestItemSended) return;
					SatelliteStorage.AddDriveChestItemSended = true;
					ModPacket packet = SatelliteStorage.instance.GetPacket();
					packet.Write((byte)SatelliteStorage.MessageType.AddDriveChestItem);
					packet.Write((byte)player.whoAmI);
					/*
					packet.Write7BitEncodedInt(Main.mouseItem.type);
					packet.Write7BitEncodedInt(Main.mouseItem.stack);
					packet.Write7BitEncodedInt(Main.mouseItem.prefix);
					*/
					packet.Send();
					packet.Close();
				}
			};

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

		public void UpdateItemsTypes()
		{
			List<int> types = new List<int>();
			DriveChestSystem.GetItems().ForEach(v => types.Add(v.type));
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
			UIPanel uIPanel = new UIPanel();
			SetBasicSizesForCreativeSacrificeOrInfinitesPanel(uIPanel);
			uIPanel.BackgroundColor *= 0.8f;
			uIPanel.BorderColor *= 0.8f;
			return uIPanel;
		}

		private static void SetBasicSizesForCreativeSacrificeOrInfinitesPanel(UIElement element)
		{
			element.Width = new StyleDimension(0f, 1f);
			element.Height = new StyleDimension(-38f, 1f);
			element.Top = new StyleDimension(38f, 0f);
		}

		private void TakeItem(UIMouseEvent evt, UIElement listeningElement, int clickType)
		{
			if (_itemGrid.hoverItemIndex <= -1) return;
			DriveItem driveItem = _itemGrid._driveItems[_itemGrid.hoverItemIndex];
			if (driveItem == null) return;
			//SatelliteStorage.Debug("driveItem: " + driveItem.type + ", stack: " + driveItem.stack);
			Vector2 mousePos = evt.MousePosition;


			Player player = Main.LocalPlayer;
			Item mouseItem = player.inventory[58];

			bool isMouseItemAir = mouseItem.IsAir && Main.mouseItem.IsAir;
			bool isMouseItemSame = mouseItem.type == driveItem.type;
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
				Item takeItem = DriveChestSystem.TakeItem(driveItem.type, driveItem.prefix, clickType == 1 ? 1 : 0);
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

				UI.DriveChestUI.ReloadItems();

				if (clickType == 1)
				{
					SoundEngine.PlaySound(12);
				}
				else
				{
					SoundEngine.PlaySound(7);
				}
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				if (SatelliteStorage.TakeDriveChestItemSended) return;
				SatelliteStorage.TakeDriveChestItemSended = true;

				ModPacket packet = SatelliteStorage.instance.GetPacket();
				packet.Write((byte)SatelliteStorage.MessageType.TakeDriveChestItem);
				packet.Write((byte)player.whoAmI);
				packet.Write((byte)clickType);
				packet.Write7BitEncodedInt(driveItem.type);
				packet.Write7BitEncodedInt(driveItem.prefix);
				packet.Send();
				packet.Close();
			}

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
				TakeItem(evt, listeningElement, 0);
				return;
			};

			item.OnRightMouseDown += (UIMouseEvent evt, UIElement listeningElement) =>
			{
				TakeItem(evt, listeningElement, 1);
				return;
			};

			UIElement uIElement = new UIElement
			{
				Height = new StyleDimension(24f, 0f),
				Width = new StyleDimension(0f, 1f)
			};
			uIElement.SetPadding(0f);
			uIPanel.Append(uIElement);
			AddSearchBar(uIElement);
			_searchBar.SetContents(null, forced: true);
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
			uICreativeItemsInfiniteFilteringOptions = new UICreativeItemsInfiniteFilteringOptions(_filterer, "CreativeInfinitesFilter");
			uICreativeItemsInfiniteFilteringOptions.OnClickingOption += filtersHelper_OnClickingOption;
			uICreativeItemsInfiniteFilteringOptions.Left = new StyleDimension(20f, 0f);
			totalContainer.Append(uICreativeItemsInfiniteFilteringOptions);
			uICreativeItemsInfiniteFilteringOptions.OnUpdate += Hover_OnUpdate;
			uICreativeItemsInfiniteFilteringOptions.OnMouseOver += Hover_OnMouseOver;
			uICreativeItemsInfiniteFilteringOptions.OnMouseOut += Hover_OnMouseOut;
		}

		private void UpdateSacrificeAnimation()
		{
			if (_sacrificeAnimationTimeLeft > 0)
			{
				_sacrificeAnimationTimeLeft--;
			}
		}

		public void SetPageTypeToShow(InfiniteItemsDisplayPage page)
		{
			_showSacrificesInsteadOfInfinites = page == InfiniteItemsDisplayPage.InfiniteItemsResearch;
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

			_itemIdsAvailableToShow.AddRange(_itemIdsAvailableTotal.Where((int x) =>
			{
				if (!ContentSamples.ItemsByType.ContainsKey(x)) return false;
				return _filterer.FitsFilter(ContentSamples.ItemsByType[x]);
			}));

			_itemIdsAvailableToShow.Sort(_sorter);
			_itemGrid.SetContentsToShow(_itemIdsAvailableToShow, DriveChestSystem.GetItems());
		}

		private void AddSearchBar(UIElement searchArea)
		{
			UIImageButton uIImageButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Search", (AssetRequestMode)1))
			{
				VAlign = 0.5f,
				HAlign = 0f
			};
			uIImageButton.OnClick += Click_SearchArea;
			uIImageButton.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Search_Border", (AssetRequestMode)1));
			uIImageButton.SetVisibility(1f, 1f);
			uIImageButton.SetSnapPoint("CreativeInfinitesSearch", 0);
			searchArea.Append(uIImageButton);
			UIPanel uIPanel = (_searchBoxPanel = new UIPanel
			{
				Width = new StyleDimension(0f - uIImageButton.Width.Pixels - 3f, 1f),
				Height = new StyleDimension(0f, 1f),
				VAlign = 0.5f,
				HAlign = 1f
			});
			uIPanel.BackgroundColor = new Color(35, 40, 83);
			uIPanel.BorderColor = new Color(35, 40, 83);
			uIPanel.SetPadding(0f);
			searchArea.Append(uIPanel);
			UISearchBar uISearchBar = (_searchBar = new UISearchBar(Language.GetText("UI.PlayerNameSlot"), 0.8f)
			{
				Width = new StyleDimension(0f, 1f),
				Height = new StyleDimension(0f, 1f),
				HAlign = 0f,
				VAlign = 0.5f,
				Left = new StyleDimension(0f, 0f),
				IgnoresMouseInteraction = true
			});
			uIPanel.OnClick += Click_SearchArea;
			uIPanel.OnRightClick += RightClick_SearchArea;
			uISearchBar.OnContentsChanged += OnSearchContentsChanged;
			uIPanel.Append(uISearchBar);
			uISearchBar.OnStartTakingInput += OnStartTakingInput;
			uISearchBar.OnEndTakingInput += OnEndTakingInput;
			uISearchBar.OnNeedingVirtualKeyboard += OpenVirtualKeyboardWhenNeeded;
			uISearchBar.OnCancledTakingInput += OnCancledInput;
			UIImageButton uIImageButton2 = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/SearchCancel", (AssetRequestMode)1))
			{
				HAlign = 1f,
				VAlign = 0.5f,
				Left = new StyleDimension(-2f, 0f)
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
				SoundEngine.PlaySound(11);
			}
			else
			{
				SoundEngine.PlaySound(12);
			}
		}

		private void searchCancelButton_OnMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(12);
		}

		private void OnCancledInput()
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
			_searchBoxPanel.BorderColor = new Color(35, 40, 83);
		}

		private void OpenVirtualKeyboardWhenNeeded()
		{
			int maxInputLength = 40;
			UIVirtualKeyboard uIVirtualKeyboard = new UIVirtualKeyboard(Language.GetText("UI.PlayerNameSlot").Value, _searchString, OnFinishedSettingName, GoBackHere, 3, allowEmpty: true);
			uIVirtualKeyboard.SetMaxInputLength(maxInputLength);
			IngameFancyUI.OpenUIState(uIVirtualKeyboard);
		}

		private static UserInterface GetCurrentInterface()
		{
			UserInterface activeInstance = UserInterface.ActiveInstance;
			if (Main.gameMenu)
			{
				return Main.MenuUI;
			}
			return Main.InGameUI;
		}

		private void OnFinishedSettingName(string name)
		{
			string contents = name.Trim();
			_searchBar.SetContents(contents);
			GoBackHere();
		}

		private void GoBackHere()
		{
			IngameFancyUI.Close();
			_searchBar.ToggleTakingText();
			Main.CreativeMenu.GamepadMoveToSearchButtonHack = true;
		}

		public int GetItemsPerLine()
		{
			return _itemGrid.GetItemsPerLine();
		}

        public override void Recalculate()
        {
            base.Recalculate();
			if (_itemGrid != null) _itemGrid.Recalculate();
        }
    }
}