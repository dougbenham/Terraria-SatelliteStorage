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
	public class DriveChestUI : BaseUIState
	{
		public static DriveChestUI Instance;

		private readonly float[] _buttonScale = new float[2];
		private readonly bool[] _buttonHovered = new bool[2];
		private Item _itemOnMouse;
		private bool _isDrawing;
		private int _currentRecipe = -1;
		private bool _isMouseDownOnCraftItem;
		private UserInterface driveChestInterface;
		private GameTime _lastUpdateUiGameTime;
		private double lastCraftsResearchTime;
		private UIItemsDisplay display;
		private UICraftDisplay craftDisplay;
		private UICraftRecipe craftRecipe;
		private UIPanel craftResultPanel;
		private float windowWidth;
		private float windowHeight;
		private Vector2 openPosition;
		private bool checkPosition;
		private Vector2 buttonsPos;
		private Vector2 craftResultSlotPos;
		private Item craftResultItem = new(5, 5);
		private int craftOnMouseRecipe = -1;
		private double mouseDownOnCraftItemTime;
		private bool mouseDownOnCraftItemToggle;
		private double mouseDownItemCraftCooldown;

		public void UpdateHover(int id, bool hovering)
		{
			if (hovering)
			{
				if (!_buttonHovered[id])
				{
					SoundEngine.PlaySound(12);
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

			Instance.display.RebuildPage();
		}


		public static void SetOpenedPosition(Vector2 pos)
		{
			Instance.checkPosition = true;
			Instance.openPosition = pos;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			CalculateSize();

			driveChestInterface = new();
			
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

							ModPacket packet = SatelliteStorage.instance.GetPacket();
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
			display = new(this);

			Append(display);

			craftDisplay = new(this);

			Append(craftDisplay);

			craftRecipe = new(this);
			Append(craftRecipe);

			craftResultPanel = new UICraftResultBG();
			Append(craftResultPanel);

			CalculateSize();

			craftDisplay.onRecipeChoosen += (int recipe) =>
			{
				_currentRecipe = recipe;
				craftRecipe.SetRecipe(recipe);
				SoundEngine.PlaySound(12);
			};

			OnMouseDown += (UIMouseEvent evt, UIElement listeningElement) =>
			{
				if (craftOnMouseRecipe > -1)
				{
					if (Main.keyState.IsKeyDown(Keys.LeftControl) || Main.keyState.IsKeyDown(Keys.RightControl))
					{
						while (TryCraftItem())
							;
						return;
					}

					mouseDownOnCraftItemToggle = false;
					_isMouseDownOnCraftItem = true;
					TryCraftItem();
				}
			};

			OnMouseUp += (UIMouseEvent evt, UIElement listeningElement) => { _isMouseDownOnCraftItem = false; };
		}

		private bool TryCraftItem()
		{
			if (craftOnMouseRecipe <= -1)
				return false;

			var recipe = Main.recipe[craftOnMouseRecipe];
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
					item.type = u.type;
					item.SetDefaults(item.type);
					item.stack = 1;

					if (u.from == 0)
					{
						player.inventory[u.slot].stack -= u.stack;
						if (player.inventory[u.slot].stack <= 0)
							player.inventory[u.slot].TurnToAir();
					}
					else
					{
						DriveChestSystem.SubItem(u.type, u.stack);
					}

					//SatelliteStorage.Debug(item.Name + "(" + u.stack + ") from " + u.from + (u.from == 0 ? (" at slot " + u.slot) : ""));
				});

				if (isMouseItemAir)
				{
					Main.mouseItem = recipe.createItem.Clone();
				}
				else
				{
					Main.mouseItem.stack += recipe.createItem.stack;
				}

				SoundEngine.PlaySound(7);
				DriveChestSystem.CheckRecipesRefresh = false;
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				var uses = DriveChestSystem.GetItemUsesForCraft(player.inventory, recipe);
				if (uses == null)
					return false;

				var packet = SatelliteStorage.instance.GetPacket();
				packet.Write((byte) SatelliteStorage.MessageType.TryCraftRecipe);
				packet.Write((byte) player.whoAmI);
				packet.Write7BitEncodedInt(craftOnMouseRecipe);
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

				if (item != null && !item.favorited && !item.IsAir && i != 58)
				{
					var driveItem = new DriveItem();

					driveItem.type = item.type;
					driveItem.stack = item.stack;
					driveItem.prefix = item.prefix;

					if (Main.netMode == NetmodeID.SinglePlayer)
					{
						if ((newItems ? true : DriveChestSystem.HasItem(driveItem)) && DriveChestSystem.AddItem(driveItem))
						{
							item.TurnToAir();
							itemAdded = true;
						}
					}

					if (Main.netMode == NetmodeID.MultiplayerClient)
					{
						var packet = SatelliteStorage.instance.GetPacket();
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

						if ((newItems ? true : DriveChestSystem.HasItem(driveItem)))
						{
							itemAdded = true;
						}
					}

				}
			}

			if (itemAdded)
			{
				SoundEngine.PlaySound(7);
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
			craftOnMouseRecipe = -1;
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

			if (craftDisplay != null && !UICraftDisplay.hidden)
			{
				Terraria.Utils.DrawBorderString(spriteBatch, Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChestRecipes"), craftDisplay.GetDimensions().Position() + new Vector2(30, 47),
					Color.White, 1f);
			}

			if (craftRecipe != null && !UICraftRecipe.hidden)
			{
				Terraria.Utils.DrawBorderString(spriteBatch, Language.GetTextValue("Mods.SatelliteStorage.UITitles.DriveChestCraft"), craftRecipe.GetDimensions().Position() + new Vector2(30, 47),
					Color.White, 1f);
			}

			if (_currentRecipe > -1 && !UICraftRecipe.hidden)
			{
				var recipe = Main.recipe[_currentRecipe];
				craftResultItem = recipe.createItem;
				var itemSlotHitbox = GetSlotHitbox((int) craftResultSlotPos.X, (int) craftResultSlotPos.Y);
				var cReativeItemSlotShouldHighlightAsSelected = false;
				if (IsMouseHovering && itemSlotHitbox.Contains(Main.MouseScreen.ToPoint()) && !PlayerInput.IgnoreMouseInterface)
				{
					Main.LocalPlayer.mouseInterface = true;

					ItemSlot.OverrideHover(ref craftResultItem, 26);
					//ItemSlot.LeftClick(ref inv, context);
					//ItemSlot.RightClick(ref inv, context);
					ItemSlot.MouseHover(ref craftResultItem, 26);

					craftOnMouseRecipe = _currentRecipe;
					cReativeItemSlotShouldHighlightAsSelected = true;
				}
				else
				{
					_isMouseDownOnCraftItem = false;
				}

				UILinkPointNavigator.Shortcuts.CREATIVE_ItemSlotShouldHighlightAsSelected = cReativeItemSlotShouldHighlightAsSelected;
				ItemSlot.Draw(spriteBatch, ref craftResultItem, 26, itemSlotHitbox.TopLeft());
				UICraftResultBG.hidden = false;
			}
			else
			{
				UICraftResultBG.hidden = true;
			}
		}


		public override void OnUpdateUI(GameTime gameTime)
		{

			_isDrawing = false;

			if (driveChestInterface?.CurrentState != null)
			{
				_isDrawing = true;
				var elapsedTime = gameTime.TotalGameTime.TotalMilliseconds - lastCraftsResearchTime;

				_lastUpdateUiGameTime = gameTime;

				if (_isMouseDownOnCraftItem)
				{
					if (!mouseDownOnCraftItemToggle)
					{
						mouseDownItemCraftCooldown = gameTime.TotalGameTime.TotalMilliseconds;
						mouseDownOnCraftItemTime = gameTime.TotalGameTime.TotalMilliseconds;
						mouseDownOnCraftItemToggle = true;
					}
				}
				else
				{
					mouseDownOnCraftItemToggle = false;
				}

				var mouseDownOnCraftItemElapsed = gameTime.TotalGameTime.TotalMilliseconds - mouseDownOnCraftItemTime;

				if (mouseDownOnCraftItemToggle && _isMouseDownOnCraftItem)
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

					if (gameTime.TotalGameTime.TotalMilliseconds > mouseDownItemCraftCooldown + waitTime)
					{
						mouseDownItemCraftCooldown = gameTime.TotalGameTime.TotalMilliseconds;
						TryCraftItem();
					}

				}

				if (Main.recBigList)
					_isDrawing = false;

				CalculateSize();
				driveChestInterface.Update(gameTime);

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
					lastCraftsResearchTime = gameTime.TotalGameTime.TotalMilliseconds;
					DriveChestSystem.ResearchRecipes();
					Instance.craftDisplay.RebuildPage();
					Instance.craftRecipe.RebuildPage();
				}

				if (checkPosition)
				{
					if (Main.LocalPlayer.position.Distance(openPosition) > 100)
						SatelliteStorage.SetUIState((int) UITypes.DriveChest, false);
				}

				if (!Main.playerInventory)
					SatelliteStorage.SetUIState((int) UITypes.DriveChest, false);

			}
		}

		private void CalculateSize()
		{
			var width = GetDimensions().Width;
			var height = GetDimensions().Height;


			if (windowHeight != height || windowWidth != width)
			{
				windowWidth = width;
				windowHeight = height;

				if (display != null)
				{
					display.HAlign = 0.5f;
					display.VAlign = 0.5f;

					display.Width.Set(width * 0.3f, 0);
					display.Height.Set(height * 0.6f, 0);
					display.MinWidth.Set(500, 0);


					var smallWidth = false;
					if (width < 1650)
					{
						display.Height.Set(height * 0.35f, 0);
						display.VAlign = 0.85f;
						smallWidth = true;
					}

					display.Recalculate();


					if (smallWidth)
					{
						buttonsPos = new(
							display.GetDimensions().X + 30,
							display.GetDimensions().Y - 50
						);
					}
					else
					{
						buttonsPos = new(
							display.GetDimensions().X + display.GetDimensions().Width + 15,
							display.GetDimensions().Y + 70
						);
					}


					var craftDisplayWidth = MathF.Max(width * 0.2f, 150);
					var craftDisplayHeight = MathF.Max(display.Height.Pixels * 0.77f, 150);
					craftDisplay.Left.Set(display.GetDimensions().X - craftDisplayWidth - 15, 0);
					craftDisplay.Top.Set(display.GetDimensions().Y + (display.Height.Pixels - craftDisplayHeight), 0);
					craftDisplay.Width.Set(craftDisplayWidth, 0);
					craftDisplay.Height.Set(craftDisplayHeight, 0);

					craftDisplay.Recalculate();

					if (smallWidth)
					{
						var craftRecipeWidth = MathF.Max(width * 0.1f, 150);
						var craftRecipeHeight = MathF.Max(craftDisplay.Height.Pixels * 0.55f, 150);
						craftRecipe.Left.Set(craftDisplay.GetDimensions().X, 0);
						craftRecipe.Top.Set(craftDisplay.GetDimensions().Y - craftRecipeHeight + 25, 0);
						craftRecipe.Width.Set(craftDisplay.Width.Pixels, 0);
						craftRecipe.Height.Set(craftRecipeHeight, 0);

						craftRecipe.Recalculate();

						var craftResultWidth = craftRecipe.Width.Pixels * 0.25f;
						var craftResultHeight = craftRecipe.Width.Pixels * 0.25f;
						craftResultPanel.Left.Set(craftRecipe.GetDimensions().X + (craftRecipe.Width.Pixels / 2) - (craftResultWidth / 2), 0);
						craftResultPanel.Top.Set(craftRecipe.GetDimensions().Y - craftResultHeight + 30, 0);
						craftResultPanel.Width.Set(craftResultWidth, 0);
						craftResultPanel.Height.Set(craftResultHeight, 0);
					}
					else
					{
						var craftRecipeWidth = MathF.Max(width * 0.1f, 150);
						var craftRecipeHeight = MathF.Max(craftDisplay.Height.Pixels * 0.55f, 150);
						craftRecipe.Left.Set(craftDisplay.GetDimensions().X - craftRecipeWidth - 15, 0);
						craftRecipe.Top.Set(craftDisplay.GetDimensions().Y, 0);
						craftRecipe.Width.Set(craftRecipeWidth, 0);
						craftRecipe.Height.Set(craftRecipeHeight, 0);

						craftRecipe.Recalculate();


						var craftResultWidth = craftRecipe.Width.Pixels * 0.35f;
						craftResultPanel.Left.Set(craftRecipe.GetDimensions().X + (craftRecipe.Width.Pixels / 2) - (craftResultWidth / 2), 0);
						craftResultPanel.Top.Set(craftRecipe.GetDimensions().Y + craftRecipe.Height.Pixels + 15, 0);
						craftResultPanel.Width.Set(craftRecipe.Width.Pixels * 0.35f, 0);
						craftResultPanel.Height.Set(craftRecipe.Width.Pixels * 0.35f, 0);
					}



					craftResultPanel.Recalculate();

					if (smallWidth)
					{
						craftResultSlotPos = new(
							craftResultPanel.GetDimensions().X + (craftResultPanel.Width.Pixels / 2) - (40 / 2) - 1,
							craftResultPanel.GetDimensions().Y + (craftResultPanel.Height.Pixels / 2) - (40 / 2) + 1
						);
					}
					else
					{
						craftResultSlotPos = new(
							craftResultPanel.GetDimensions().X + (craftResultPanel.Width.Pixels / 2) - (40 / 2),
							craftResultPanel.GetDimensions().Y + (craftResultPanel.Height.Pixels / 2) - (40 / 2)
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
						if (_lastUpdateUiGameTime != null && driveChestInterface?.CurrentState != null)
						{
							driveChestInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
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
				driveChestInterface?.SetState(this);
				CalculateSize();
				ReloadItems();
				DriveChestSystem.CheckRecipesRefresh = false;
			}
			else
			{
				checkPosition = false;
				_isMouseDownOnCraftItem = false;
				driveChestInterface?.SetState(null);

				for (var i = 0; i < 2; i++)
				{
					_buttonScale[i] = 0.75f;
					_buttonHovered[i] = false;
				}

			}
		}

		public override bool GetState()
		{
			return driveChestInterface?.CurrentState != null ? true : false;
		}

		private void DrawButtons(SpriteBatch spritebatch)
		{
			if (Instance.display == null)
				return;
			for (var i = 0; i < 2; i++)
			{
				DrawButton(spritebatch, i, (int) (Instance.buttonsPos.X), (int) (Instance.buttonsPos.Y));
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
			var color = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor) * num2;
			color = Color.White * 0.97f * (1f - (255f - (float) (int) Main.mouseTextColor) / 255f * 0.5f);
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
