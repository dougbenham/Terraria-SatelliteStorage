﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace SatelliteStorage.UIElements
{
	class TextBox : UIPanel//UITextPanel<string>
	{
		//private static readonly Asset<Texture2D> CloseButtonTexture = RecipeBrowser.instance.Assets.Request<Texture2D>("UIElements/closeButton", AssetRequestMode.ImmediateLoad);
		internal bool focused;

		//private int _cursor;
		//private int _frameCount;
		private int _maxLength = 60;

		public string hintText = "";
		public string currentString = "";
		private int textBlinkerCount;
		private int textBlinkerState;

		public event Action OnFocus;

		public event Action OnUnfocus;

		public event Action OnTextChanged;

		public event Action OnTabPressed;

		public event Action OnEnterPressed;

		//public event Action OnUpPressed;
		internal bool unfocusOnEnter = true;

		internal bool unfocusOnTab = true;

		public Color textColor = Color.Black;
		public float textScale = 1;
		public Vector2 textPosition = new(4, 2);
		public int visibleTextCount = 10;

		//public NewUITextBox(string text, float textScale = 1, bool large = false) : base("", textScale, large)
		public TextBox()
		{
			SetPadding(0);
			BackgroundColor = Color.White;
			BorderColor = Color.White;

			//			keyBoardInput.newKeyEvent += KeyboardInput_newKeyEvent;

			/*
			var closeButton = new UIHoverImageButton(CloseButtonTexture, "");
			closeButton.OnClick += (a, b) => SetText("");
			closeButton.Left.Set(-20f, 1f);
			//closeButton.Top.Set(0f, .5f);
			closeButton.VAlign = 0.5f;
			//closeButton.HAlign = 0.5f;
			Append(closeButton);
			*/

		}

		public override void Click(UIMouseEvent evt)
		{
			Focus();
			base.Click(evt);
		}

		public override void RightClick(UIMouseEvent evt)
		{
			base.RightClick(evt);
			SetText("");
		}

		public void SetUnfocusKeys(bool unfocusOnEnter, bool unfocusOnTab)
		{
			this.unfocusOnEnter = unfocusOnEnter;
			this.unfocusOnTab = unfocusOnTab;
		}

		//void KeyboardInput_newKeyEvent(char obj)
		//{
		//	// Problem: keyBoardInput.newKeyEvent only fires on regular keyboard buttons.

		//	if (!focused) return;
		//	if (obj.Equals((char)Keys.Back)) // '\b'
		//	{
		//		Backspace();
		//	}
		//	else if (obj.Equals((char)Keys.Enter))
		//	{
		//		Unfocus();
		//		Main.chatRelease = false;
		//	}
		//	else if (Char.IsLetterOrDigit(obj))
		//	{
		//		Write(obj.ToString());
		//	}
		//}

		public void Unfocus()
		{
			if (focused)
			{
				focused = false;
				Main.blockInput = false;

				OnUnfocus?.Invoke();
			}
		}

		public void Focus()
		{
			if (!focused)
			{
				Main.clrInput();
				focused = true;
				Main.blockInput = true;

				OnFocus?.Invoke();
			}
		}

		public override void Update(GameTime gameTime)
		{
			var MousePosition = new Vector2(Main.mouseX, Main.mouseY);
			if (!ContainsPoint(MousePosition) && (Main.mouseLeft || Main.mouseRight)) // This solution is fine, but we need a way to cleanly "unload" a UIElement
			{
				// TODO, figure out how to refocus without triggering unfocus while clicking enable button.
				Unfocus();
			}
			base.Update(gameTime);
		}

		//public void Write(string text)
		//{
		//	base.SetText(base.Text.Insert(this._cursor, text));
		//	this._cursor += text.Length;
		//	_cursor = Math.Min(Text.Length, _cursor);
		//	Recalculate();

		//	OnTextChanged?.Invoke();
		//}

		//public void WriteAll(string text)
		//{
		//	bool changed = text != Text;
		//	if (!changed) return;
		//	base.SetText(text);
		//	this._cursor = text.Length;
		//	//_cursor = Math.Min(Text.Length, _cursor);
		//	Recalculate();

		//	if (changed)
		//	{
		//		OnTextChanged?.Invoke();
		//	}
		//}

		public void SetText(string text)
		{
			if (text.Length > _maxLength)
			{
				text = text.Substring(0, _maxLength);
			}
			if (currentString != text)
			{
				currentString = text;
				OnTextChanged?.Invoke();
			}
		}

		public void SetTextMaxLength(int maxLength)
		{
			_maxLength = maxLength;
		}

		//public void Backspace()
		//{
		//	if (this._cursor == 0)
		//	{
		//		return;
		//	}
		//	base.SetText(base.Text.Substring(0, base.Text.Length - 1));
		//	Recalculate();
		//}

		/*public void CursorLeft()
		{
			if (this._cursor == 0)
			{
				return;
			}
			this._cursor--;
		}
		public void CursorRight()
		{
			if (this._cursor < base.Text.Length)
			{
				this._cursor++;
			}
		}*/

		private static bool JustPressed(Keys key)
		{
			return Main.inputText.IsKeyDown(key) && !Main.oldInputText.IsKeyDown(key);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			var hitbox = GetInnerDimensions().ToRectangle();

			// Draw panel
			base.DrawSelf(spriteBatch);
			//	Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Yellow);

			if (focused)
			{
				PlayerInput.WritingText = true;
				Main.instance.HandleIME();
				var newString = Main.GetInputText(currentString);
				if (!newString.Equals(currentString))
				{
					currentString = newString;
					OnTextChanged?.Invoke();
				}
				else
				{
					currentString = newString;
				}

				if (JustPressed(Keys.Tab))
				{
					if (unfocusOnTab) Unfocus();
					OnTabPressed?.Invoke();
				}
				if (JustPressed(Keys.Enter))
				{
					Main.drawingPlayerChat = false;
					if (unfocusOnEnter) Unfocus();
					OnEnterPressed?.Invoke();
				}
				if (++textBlinkerCount >= 20)
				{
					textBlinkerState = (textBlinkerState + 1) % 2;
					textBlinkerCount = 0;
				}
				Main.instance.DrawWindowsIMEPanel(new(98f, Main.screenHeight - 36));
			}
			var displayString = currentString;
			var space = GetDimensions();
			var color = textColor;
			if (currentString.Length == 0)
			{
			}
			var drawPos = space.Position() + textPosition;
			if (currentString.Length == 0 && !focused)
			{
				color *= 0.5f;
				//Utils.DrawBorderString(spriteBatch, hintText, new Vector2(space.X, space.Y), Color.Gray, 1f);
				spriteBatch.DrawString(FontAssets.MouseText.Value, hintText, drawPos, color, 0, new(0, 0), textScale, SpriteEffects.None, 0);

			}
			else
			{
				//Utils.DrawBorderString(spriteBatch, displayString, drawPos, Color.White, 1f);


				var displayValue = displayString;

				
				if (displayValue.Length > visibleTextCount+1)
                {
					var substFrom = displayString.Length - visibleTextCount - 1;
					if (substFrom <= 0) substFrom = 0;
					var substCount = visibleTextCount;
					if (substCount <= 0) substCount = 0;

					displayValue = displayValue.Substring(substFrom, substCount);
				}

				if (textBlinkerState == 1 && focused)
				{
					displayValue = displayValue + "|";
				}
				
				spriteBatch.DrawString(FontAssets.MouseText.Value, displayValue, drawPos, color, 0, new(0,0), textScale, SpriteEffects.None, 0);
			}

			//			CalculatedStyle innerDimensions2 = base.GetInnerDimensions();
			//			Vector2 pos2 = innerDimensions2.Position();
			//			if (IsLarge)
			//			{
			//				pos2.Y -= 10f * TextScale * TextScale;
			//			}
			//			else
			//			{
			//				pos2.Y -= 2f * TextScale;
			//			}
			//			//pos2.X += (innerDimensions2.Width - TextSize.X) * 0.5f;
			//			if (IsLarge)
			//			{
			//				Utils.DrawBorderStringBig(spriteBatch, Text, pos2, TextColor, TextScale, 0f, 0f, -1);
			//				return;
			//			}
			//			Utils.DrawBorderString(spriteBatch, Text, pos2, TextColor, TextScale, 0f, 0f, -1);
			//
			//			this._frameCount++;
			//
			//			CalculatedStyle innerDimensions = base.GetInnerDimensions();
			//			Vector2 pos = innerDimensions.Position();
			//			DynamicSpriteFont spriteFont = base.IsLarge ? Main.fontDeathText : FontAssets.MouseText.Value;
			//			Vector2 vector = new Vector2(spriteFont.MeasureString(base.Text.Substring(0, this._cursor)).X, base.IsLarge ? 32f : 16f) * base.TextScale;
			//			if (base.IsLarge)
			//			{
			//				pos.Y -= 8f * base.TextScale;
			//			}
			//			else
			//			{
			//				pos.Y -= 1f * base.TextScale;
			//			}
			//			if (Text.Length == 0)
			//			{
			//				Vector2 hintTextSize = new Vector2(spriteFont.MeasureString(hintText.ToString()).X, IsLarge ? 32f : 16f) * TextScale;
			//				pos.X += 5;//(hintTextSize.X);
			//				if (base.IsLarge)
			//				{
			//					Utils.DrawBorderStringBig(spriteBatch, hintText, pos, Color.Gray, base.TextScale, 0f, 0f, -1);
			//					return;
			//				}
			//				Utils.DrawBorderString(spriteBatch, hintText, pos, Color.Gray, base.TextScale, 0f, 0f, -1);
			//				pos.X -= 5;
			//				//pos.X -= (innerDimensions.Width - hintTextSize.X) * 0.5f;
			//			}
			//
			//			if (!focused) return;
			//
			//			pos.X += /*(innerDimensions.Width - base.TextSize.X) * 0.5f*/ +vector.X - (base.IsLarge ? 8f : 4f) * base.TextScale + 6f;
			//			if ((this._frameCount %= 40) > 20)
			//			{
			//				return;
			//			}
			//			if (base.IsLarge)
			//			{
			//				Utils.DrawBorderStringBig(spriteBatch, "|", pos, base.TextColor, base.TextScale, 0f, 0f, -1);
			//				return;
			//			}
			//			Utils.DrawBorderString(spriteBatch, "|", pos, base.TextColor, base.TextScale, 0f, 0f, -1);
		}
	}
}