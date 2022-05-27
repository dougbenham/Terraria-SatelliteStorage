using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SatelliteStorage.DriveSystem;
using SatelliteStorage.UIElements;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;

namespace SatelliteStorage.UI
{
	public class DriveChestUi : BaseUiState
	{
		public static DriveChestUi Instance;

		private readonly float[] _buttonScale = new float[2];
		private readonly bool[] _buttonHovered = new bool[2];
		private Item _itemOnMouse;
		private bool _isDrawing;
		private int _currentRecipe = -1;
		private bool _isMouseDownOnCraftItem;
		private UserInterface _driveChestInterface;
		private GameTime _lastUpdateUiGameTime;
		private double _lastCraftsResearchTime;
		private UiItemsDisplay _display;
		private UiCraftDisplay _craftDisplay;
		private UiCraftRecipe _craftRecipe;
		private UIPanel _craftResultPanel;
		private float _windowWidth;
		private float _windowHeight;
		private Vector2 _buttonsPos;
		private Vector2 _craftResultSlotPos;
		private Item _craftResultItem = new(5, 5);
		private int _craftOnMouseRecipe = -1;
		private double _mouseDownOnCraftItemTime;
		private bool _mouseDownOnCraftItemToggle;
		private double _mouseDownItemCraftCooldown;

		public void UpdateHover(int id, bool hovering)
		{
			if (hovering)
			{
				if (!_buttonHovered[id])
				{
					SoundEngine.PlaySound(SoundID.MenuTick);
				}

				_buttonHovered[id] = true;
				_buttonScale[id] += 0.05f;
				if (_buttonScale[id] > 1f)
				{
					_buttonScale[id] = 1f;
				}
			}
			else
			{
				_buttonHovered[id] = false;
				_buttonScale[id] -= 0.05f;
				if (_buttonScale[id] < 0.75f)
				{
					_buttonScale[id] = 0.75f;
				}
			}
		}

		public static void ReloadItems()
		{
			if (Instance == null)
				return;
			if (!Instance.GetState())
				return;

			Instance._display.RebuildPage();
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			CalculateSize();

			_driveChestInterface = new();
			
			/*
			mainPanel = new UIPanel();
			mainPanel.Width.Set(width, 0);
			mainPanel.Height.Set(height, 0);
			mainPanel.HAlign = mainPanel.VAlign = 0.5f;

			mainPanel.OnMouseDown += OnPanelMouseDown;

			mainPanel.OnMouseOver += (UIMouseEvent evt, UIElement listeningElement) =>
			{
				mouseOver = true;
			};

			mainPanel.OnMouseOut += (UIMouseEvent evt, UIElement listeningElement) =>
			{
				mouseOver = false;
			};

			Append(mainPanel);

			scrollbar = new UIScrollbar();
			scrollbar.Width.Set(20, 0);
			scrollbar.Height.Set(height/100*90, 0);
			scrollbar.VAlign = 0.5f;
			scrollbar.HAlign = 1f;
			mainPanel.Append(scrollbar);


			searchTextBox = new UIElements.TextBox();
			searchTextBox.hintText = Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChestSearch");
			searchTextBox.BackgroundColor.R = 35;
			searchTextBox.BackgroundColor.G = 40;
			searchTextBox.BackgroundColor.B = 83;
			searchTextBox.BorderColor.R = 38;
			searchTextBox.BorderColor.G = 44;
			searchTextBox.BorderColor.B = 88;
			searchTextBox.textColor.R = 255;
			searchTextBox.textColor.G = 255;
			searchTextBox.textColor.B = 255;
			searchTextBox.textScale = 0.8f;
			searchTextBox.visibleTextCount = 15;
			searchTextBox.textPosition = new Vector2(10, 4);
			searchTextBox.Width.Set(width / 100 * 82, 0);
			searchTextBox.Height.Set(25, 0);
			searchTextBox.VAlign = 0.01f;
			searchTextBox.HAlign = 0.01f;
			
			mainPanel.Append(searchTextBox);

			itemsCoverPanel = new UIPanel();
			mainPanel.Append(itemsCoverPanel);

			itemsPanel = new UIPanel();

			itemsPanel.Width.Set(width / 100 * 80, 0);
			itemsPanel.Height.Set(99999, 0);
			//itemsPanel.Height.Set(height / 100 * 77, 0);
			itemsPanel.BackgroundColor = new Color(0, 0, 0, 0);
			itemsPanel.BorderColor = new Color(0, 0, 0, 0);

			itemsCoverPanel.OnMouseDown += (UIMouseEvent evt, UIElement listeningElement) =>
			{
				Vector2 mousePos = evt.MousePosition;
				foreach(UIElements.DriveItemPanel driveItemPanel in driveItemsPanels)
                {
					Rectangle area = driveItemPanel.GetViewCullingArea();

					if (mousePos.WithinRange(new Vector2(area.X+(area.Width/2), area.Y + (area.Width / 2)), area.Width/2)) {

						Player player = Main.LocalPlayer;
						Item mouseItem = player.inventory[58];

						if (!mouseItem.IsAir || !Main.mouseItem.IsAir) return;

						if (Main.netMode == NetmodeID.SinglePlayer)
						{
							Item takeItem = DriveChestSystem.TakeItem(driveItemPanel.item.type, driveItemPanel.item.prefix);
							if (takeItem == null) return;
							Main.mouseItem = takeItem;
							UpdateItems(DriveChestSystem.GetItems());
							SoundEngine.PlaySound(7);
						}

						if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
							if (SatelliteStorage.TakeDriveChestItemSended) return;
							SatelliteStorage.TakeDriveChestItemSended = true;

							ModPacket packet = SatelliteStorage.Instance.GetPacket();
							packet.Write((byte)SatelliteStorage.MessageType.TakeDriveChestItem);
							packet.Write((byte)player.whoAmI);
							packet.Write7BitEncodedInt(driveItemPanel.item.type);
							packet.Write7BitEncodedInt(driveItemPanel.item.prefix);
							packet.Send();
							packet.Close();
						}

						return;
					}
				}
			};

			itemsCoverPanel.Width.Set(width / 100 * 80, 0);
			itemsCoverPanel.Height.Set(height / 100 * 77, 0);
			itemsCoverPanel.VAlign = 0.87f;
			itemsCoverPanel.HAlign = 0.1f;
			itemsCoverPanel.OverflowHidden = true;
			itemsCoverPanel.Append(itemsPanel);
			
			UIElements.TextButton depositAllButton = new UIElements.TextButton(Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChestDepositAll"));
			depositAllButton.HAlign = 0.5f;
			depositAllButton.VAlign = 0.5f;
			depositAllButton.Left.Set(225, 0);
			depositAllButton.Top.Set(-115, 0);

			depositAllButton.OnMouseDown += (UIMouseEvent evt, UIElement listeningElement) =>
			{
				TryDepositItems(true);
			};

			Append(depositAllButton);

			UIElements.TextButton quickStackButton = new UIElements.TextButton(Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChestQuickStack"));
			quickStackButton.HAlign = 0.5f;
			quickStackButton.VAlign = 0.5f;
			quickStackButton.Left.Set(225, 0);
			quickStackButton.Top.Set(-85, 0);

			quickStackButton.OnMouseDown += (UIMouseEvent evt, UIElement listeningElement) =>
			{
				TryDepositItems(false);
			};

			Append(quickStackButton);
			
			*/
			_display = new(this);

			Append(_display);

			_craftDisplay = new(this);

			Append(_craftDisplay);

			_craftRecipe = new(this);
			Append(_craftRecipe);

			_craftResultPanel = new UiCraftResultBg();
			Append(_craftResultPanel);

			CalculateSize();

			_craftDisplay.OnRecipeChoosen = recipe =>
			{
				_currentRecipe = recipe;
				_craftRecipe.SetRecipe(recipe);
				SoundEngine.PlaySound(SoundID.MenuTick);
			};

			OnMouseDown += (_, _) =>
			{
				if (_craftOnMouseRecipe > -1)
				{
					if (Main.keyState.IsKeyDown(Keys.LeftControl) || Main.keyState.IsKeyDown(Keys.RightControl))
					{
						while (TryCraftItem())
							;
						return;
					}

					_mouseDownOnCraftItemToggle = false;
					_isMouseDownOnCraftItem = true;
					TryCraftItem();
				}
			};

			OnMouseUp += (_, _) => { _isMouseDownOnCraftItem = false; };
		}

		private bool TryCraftItem()
		{
			if (_craftOnMouseRecipe <= -1)
				return false;

			var recipe = Main.recipe[_craftOnMouseRecipe];
			var player = Main.LocalPlayer;
			var mouseItem = player.inventory[58];

			var isMouseItemAir = mouseItem.IsAir && Main.mouseItem.IsAir;
			var isMouseItemSame = mouseItem.type == recipe.createItem.type;
			if (!isMouseItemAir && !isMouseItemSame)
				return false;

			if (isMouseItemSame)
			{
				if (mouseItem.stack + recipe.createItem.stack > mouseItem.maxStack)
					return false;
			}

			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				var uses = DriveChestSystem.GetItemUsesForCraft(player.inventory, recipe);
				if (uses == null)
					return false;
				uses.ForEach(u =>
				{
					var item = new Item();
					item.type = u.Type;
					item.SetDefaults(item.type);
					item.stack = 1;

					if (u.From == 0)
					{
						player.inventory[u.Slot].stack -= u.Stack;
						if (player.inventory[u.Slot].stack <= 0)
							player.inventory[u.Slot].TurnToAir();
					}
					else
					{
						DriveChestSystem.SubItem(u.Type, u.Stack);
					}
				});

				if (isMouseItemAir)
				{
					Main.mouseItem = recipe.createItem.Clone();
				}
				else
				{
					Main.mouseItem.stack += recipe.createItem.stack;
				}

				SoundEngine.PlaySound(SoundID.Grab);
				DriveChestSystem.CheckRecipesRefresh = false;
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				var uses = DriveChestSystem.GetItemUsesForCraft(player.inventory, recipe);
				if (uses == null)
					return false;

				var packet = SatelliteStorage.Instance.GetPacket();
				packet.Write((byte) SatelliteStorage.MessageType.TryCraftRecipe);
				packet.Write((byte) player.whoAmI);
				packet.Write7BitEncodedInt(_craftOnMouseRecipe);
				packet.Send();
				packet.Close();
			}

			return true;
		}

		private void TryDepositItems(bool newItems = false)
		{
			var player = Main.LocalPlayer;
			var itemAdded = false;
			for (var i = 10; i < 54; i++)
			{
				var item = player.inventory[i];

				if (item != null && !item.favorited && !item.IsAir)
				{
					var driveItem = new DriveItem();
					driveItem.Type = item.type;
					driveItem.Stack = item.stack;
					driveItem.Prefix = item.prefix;

					if (Main.netMode == NetmodeID.SinglePlayer)
					{
						if ((newItems || DriveChestSystem.HasItem(driveItem)) && DriveChestSystem.AddItem(driveItem))
						{
							item.TurnToAir();
							itemAdded = true;
						}
					}

					if (Main.netMode == NetmodeID.MultiplayerClient)
					{
						var packet = SatelliteStorage.Instance.GetPacket();
						packet.Write((byte) SatelliteStorage.MessageType.DepositDriveChestItem);
						packet.Write((byte) player.whoAmI);
						packet.Write((byte) (newItems ? 1 : 0));
						packet.Write((byte) i);
						/*
						packet.Write7BitEncodedInt(item.type);
						packet.Write7BitEncodedInt(item.stack);
						packet.Write7BitEncodedInt(item.prefix);
						*/
						packet.Send();
						packet.Close();

						if ((newItems || DriveChestSystem.HasItem(driveItem)))
						{
							itemAdded = true;
						}
					}

				}
			}

			if (itemAdded)
			{
				SoundEngine.PlaySound(SoundID.Grab);
				if (Main.netMode == NetmodeID.SinglePlayer)
					ReloadItems();
			}
		}

		private Rectangle GetSlotHitbox(int startX, int startY)
		{
			return new(startX, startY, 44, 44);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			_craftOnMouseRecipe = -1;
			if (!_isDrawing)
				return;

			if (Main.CreativeMenu.Enabled)
				Main.CreativeMenu.CloseMenu();
			if (Main.editChest)
				Main.editChest = false;

			Main.LocalPlayer.chest = -1;

			if (Main.npcChatText.Length > 0)
				Main.CloseNPCChatOrSign();

			if (!Main.hidePlayerCraftingMenu)
				Main.hidePlayerCraftingMenu = true;

			base.Draw(spriteBatch);


			Main.inventoryScale = 0.755f;
			if (Terraria.Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, 73f, Main.instance.invBottom, 560f * Main.inventoryScale, 224f * Main.inventoryScale))
			{
				Main.player[Main.myPlayer].mouseInterface = true;
			}

			DrawButtons(spriteBatch);

			if (_craftDisplay != null && !UiCraftDisplay.Hidden)
			{
				Terraria.Utils.DrawBorderString(spriteBatch, Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChestRecipes"), _craftDisplay.GetDimensions().Position() + new Vector2(30, 47),
					Color.White, 1f);
			}

			if (_craftRecipe != null && !UiCraftRecipe.Hidden)
			{
				Terraria.Utils.DrawBorderString(spriteBatch, Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChestCraft"), _craftRecipe.GetDimensions().Position() + new Vector2(30, 47),
					Color.White, 1f);
			}

			if (_currentRecipe > -1 && !UiCraftRecipe.Hidden)
			{
				var recipe = Main.recipe[_currentRecipe];
				_craftResultItem = recipe.createItem;
				var itemSlotHitbox = GetSlotHitbox((int) _craftResultSlotPos.X, (int) _craftResultSlotPos.Y);
				var flag = false;
				if (IsMouseHovering && itemSlotHitbox.Contains(Main.MouseScreen.ToPoint()) && !PlayerInput.IgnoreMouseInterface)
				{
					Main.LocalPlayer.mouseInterface = true;

					ItemSlot.OverrideHover(ref _craftResultItem, 26);
					ItemSlot.MouseHover(ref _craftResultItem, 26);

					_craftOnMouseRecipe = _currentRecipe;
					flag = true;
				}
				else
				{
					_isMouseDownOnCraftItem = false;
				}

				UILinkPointNavigator.Shortcuts.CREATIVE_ItemSlotShouldHighlightAsSelected = flag;
				ItemSlot.Draw(spriteBatch, ref _craftResultItem, 26, itemSlotHitbox.TopLeft());
				UiCraftResultBg.Hidden = false;
			}
			else
			{
				UiCraftResultBg.Hidden = true;
			}
		}


		public override void OnUpdateUI(GameTime gameTime)
		{

			_isDrawing = false;

			if (_driveChestInterface?.CurrentState != null)
			{
				_isDrawing = true;
				var elapsedTime = gameTime.TotalGameTime.TotalMilliseconds - _lastCraftsResearchTime;

				_lastUpdateUiGameTime = gameTime;

				if (_isMouseDownOnCraftItem)
				{
					if (!_mouseDownOnCraftItemToggle)
					{
						_mouseDownItemCraftCooldown = gameTime.TotalGameTime.TotalMilliseconds;
						_mouseDownOnCraftItemTime = gameTime.TotalGameTime.TotalMilliseconds;
						_mouseDownOnCraftItemToggle = true;
					}
				}
				else
				{
					_mouseDownOnCraftItemToggle = false;
				}

				var mouseDownOnCraftItemElapsed = gameTime.TotalGameTime.TotalMilliseconds - _mouseDownOnCraftItemTime;

				if (_mouseDownOnCraftItemToggle && _isMouseDownOnCraftItem)
				{
					double waitTime = 999999;
					if (mouseDownOnCraftItemElapsed >= 1000)
						waitTime = 200;
					if (mouseDownOnCraftItemElapsed >= 1500)
						waitTime = 150;
					if (mouseDownOnCraftItemElapsed >= 2000)
						waitTime = 100;
					if (mouseDownOnCraftItemElapsed >= 2500)
						waitTime = 50;
					if (mouseDownOnCraftItemElapsed >= 3000)
						waitTime = 16;

					if (gameTime.TotalGameTime.TotalMilliseconds > _mouseDownItemCraftCooldown + waitTime)
					{
						_mouseDownItemCraftCooldown = gameTime.TotalGameTime.TotalMilliseconds;
						TryCraftItem();
					}

				}

				if (Main.recBigList)
					_isDrawing = false;

				CalculateSize();
				_driveChestInterface.Update(gameTime);

				var player = Main.LocalPlayer;
				var mouseItem = player.inventory[58];

				if (mouseItem.IsAir || Main.mouseItem.IsAir)
				{
					if (_itemOnMouse != null)
					{
						_itemOnMouse = null;
						DriveChestSystem.CheckRecipesRefresh = false;
					}
				}
				else
				{
					if (_itemOnMouse == null || _itemOnMouse.IsNotSameTypePrefixAndStack(mouseItem))
					{
						_itemOnMouse = mouseItem;
						DriveChestSystem.CheckRecipesRefresh = false;
					}
				}


				if (elapsedTime > 256 && (!DriveChestSystem.CheckRecipesRefresh || SatelliteStoragePlayer.CheckAdjChanged()))
				{
					DriveChestSystem.CheckRecipesRefresh = true;
					_lastCraftsResearchTime = gameTime.TotalGameTime.TotalMilliseconds;
					DriveChestSystem.ResearchRecipes();
					Instance._craftDisplay.RebuildPage();
					Instance._craftRecipe.RebuildPage();
				}
				
				if (!Main.playerInventory)
					SatelliteStorage.SetUiState((int) UiTypes.DriveChest, false);

			}
		}

		private void CalculateSize()
		{
			var width = GetDimensions().Width;
			var height = GetDimensions().Height;


			if (_windowHeight != height || _windowWidth != width)
			{
				_windowWidth = width;
				_windowHeight = height;

				if (_display != null)
				{
					_display.HAlign = 0.5f;
					_display.VAlign = 0.5f;

					_display.Width.Set(width * 0.3f, 0);
					_display.Height.Set(height * 0.6f, 0);
					_display.MinWidth.Set(500, 0);


					var smallWidth = false;
					if (width < 1650)
					{
						_display.Height.Set(height * 0.35f, 0);
						_display.VAlign = 0.85f;
						smallWidth = true;
					}

					_display.Recalculate();


					if (smallWidth)
					{
						_buttonsPos = new(
							_display.GetDimensions().X + 30,
							_display.GetDimensions().Y - 50
						);
					}
					else
					{
						_buttonsPos = new(
							_display.GetDimensions().X + _display.GetDimensions().Width + 15,
							_display.GetDimensions().Y + 70
						);
					}


					var craftDisplayWidth = MathF.Max(width * 0.2f, 150);
					var craftDisplayHeight = MathF.Max(_display.Height.Pixels * 0.77f, 150);
					_craftDisplay.Left.Set(_display.GetDimensions().X - craftDisplayWidth - 15, 0);
					_craftDisplay.Top.Set(_display.GetDimensions().Y + (_display.Height.Pixels - craftDisplayHeight), 0);
					_craftDisplay.Width.Set(craftDisplayWidth, 0);
					_craftDisplay.Height.Set(craftDisplayHeight, 0);

					_craftDisplay.Recalculate();

					if (smallWidth)
					{
						var craftRecipeWidth = MathF.Max(width * 0.1f, 150);
						var craftRecipeHeight = MathF.Max(_craftDisplay.Height.Pixels * 0.55f, 150);
						_craftRecipe.Left.Set(_craftDisplay.GetDimensions().X, 0);
						_craftRecipe.Top.Set(_craftDisplay.GetDimensions().Y - craftRecipeHeight + 25, 0);
						_craftRecipe.Width.Set(_craftDisplay.Width.Pixels, 0);
						_craftRecipe.Height.Set(craftRecipeHeight, 0);

						_craftRecipe.Recalculate();

						var craftResultWidth = _craftRecipe.Width.Pixels * 0.25f;
						var craftResultHeight = _craftRecipe.Width.Pixels * 0.25f;
						_craftResultPanel.Left.Set(_craftRecipe.GetDimensions().X + (_craftRecipe.Width.Pixels / 2) - (craftResultWidth / 2), 0);
						_craftResultPanel.Top.Set(_craftRecipe.GetDimensions().Y - craftResultHeight + 30, 0);
						_craftResultPanel.Width.Set(craftResultWidth, 0);
						_craftResultPanel.Height.Set(craftResultHeight, 0);
					}
					else
					{
						var craftRecipeWidth = MathF.Max(width * 0.1f, 150);
						var craftRecipeHeight = MathF.Max(_craftDisplay.Height.Pixels * 0.55f, 150);
						_craftRecipe.Left.Set(_craftDisplay.GetDimensions().X - craftRecipeWidth - 15, 0);
						_craftRecipe.Top.Set(_craftDisplay.GetDimensions().Y, 0);
						_craftRecipe.Width.Set(craftRecipeWidth, 0);
						_craftRecipe.Height.Set(craftRecipeHeight, 0);

						_craftRecipe.Recalculate();


						var craftResultWidth = _craftRecipe.Width.Pixels * 0.35f;
						_craftResultPanel.Left.Set(_craftRecipe.GetDimensions().X + (_craftRecipe.Width.Pixels / 2) - (craftResultWidth / 2), 0);
						_craftResultPanel.Top.Set(_craftRecipe.GetDimensions().Y + _craftRecipe.Height.Pixels + 15, 0);
						_craftResultPanel.Width.Set(_craftRecipe.Width.Pixels * 0.35f, 0);
						_craftResultPanel.Height.Set(_craftRecipe.Width.Pixels * 0.35f, 0);
					}



					_craftResultPanel.Recalculate();

					if (smallWidth)
					{
						_craftResultSlotPos = new(
							_craftResultPanel.GetDimensions().X + (_craftResultPanel.Width.Pixels / 2) - (40 / 2) - 1,
							_craftResultPanel.GetDimensions().Y + (_craftResultPanel.Height.Pixels / 2) - (40 / 2) + 1
						);
					}
					else
					{
						_craftResultSlotPos = new(
							_craftResultPanel.GetDimensions().X + (_craftResultPanel.Width.Pixels / 2) - (40 / 2),
							_craftResultPanel.GetDimensions().Y + (_craftResultPanel.Height.Pixels / 2) - (40 / 2)
						);
					}
				}
			}
		}

		public override void OnModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			var mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1)
			{
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"SatelliteStorage: DriveChestUI",
					delegate
					{
						if (_lastUpdateUiGameTime != null && _driveChestInterface?.CurrentState != null)
						{
							_driveChestInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
						}

						return true;
					},
					InterfaceScaleType.UI));
			}
		}

		public override void SetState(bool state)
		{
			if (state)
			{
				Instance = this;
				_driveChestInterface?.SetState(this);
				CalculateSize();
				ReloadItems();
				DriveChestSystem.CheckRecipesRefresh = false;
			}
			else
			{
				_isMouseDownOnCraftItem = false;
				_driveChestInterface?.SetState(null);

				for (var i = 0; i < 2; i++)
				{
					_buttonScale[i] = 0.75f;
					_buttonHovered[i] = false;
				}
			}
		}

		public override bool GetState()
		{
			return _driveChestInterface?.CurrentState != null;
		}

		private void DrawButtons(SpriteBatch spritebatch)
		{
			if (Instance._display == null)
				return;
			for (var i = 0; i < 2; i++)
			{
				DrawButton(spritebatch, i, (int) (Instance._buttonsPos.X), (int) (Instance._buttonsPos.Y));
			}
		}

		private void DrawButton(SpriteBatch spriteBatch, int id, int x, int y)
		{
			var player = Main.player[Main.myPlayer];

			var num = id;

			y += num * 26;
			var num2 = _buttonScale[id] * 1.3f;
			var text = "";
			switch (id)
			{
				case 0:
					text = Lang.inter[30].Value;
					break;
				case 1:
					text = Lang.inter[31].Value;
					break;
			}

			var vector = FontAssets.MouseText.Value.MeasureString(text);
			var color = Color.White * 0.97f * (1f - (255f - Main.mouseTextColor) / 255f * 0.5f);
			color.A = byte.MaxValue;
			x += (int) (vector.X * (num2 / 2f));
			var flag = Terraria.Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, (float) x - vector.X / 2f, y - 12, vector.X, 24f);
			if (_buttonHovered[id])
			{
				flag = Terraria.Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, (float) x - vector.X / 2f - 10f, y - 12, vector.X + 16f, 24f);
			}

			if (flag)
			{
				color = Main.OurFavoriteColor;
			}

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, new(x, y), color, 0f, vector / 2f, new(num2), -1f, 1.5f);
			vector *= num2;
			switch (id)
			{
				case 0:
					UILinkPointNavigator.SetPosition(500, new((float) x - vector.X * (num2 / 2f * 0.8f), y));
					break;
				case 1:
					UILinkPointNavigator.SetPosition(501, new((float) x - vector.X * (num2 / 2f * 0.8f), y));
					break;
			}

			if (!flag)
			{
				UpdateHover(id, hovering: false);
				return;
			}

			UpdateHover(id, hovering: true);
			if (PlayerInput.IgnoreMouseInterface)
			{
				return;
			}

			player.mouseInterface = true;
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				switch (id)
				{
					case 0:
						Instance.TryDepositItems(true);
						break;
					case 1:
						Instance.TryDepositItems();
						break;
				}

				Recipe.FindRecipes();
			}
		}
	}
}
