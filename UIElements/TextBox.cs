using System;
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
	class TextBox : UIPanel
	{
		internal bool focused;
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
		internal bool unfocusOnEnter = true;
		internal bool unfocusOnTab = true;
		public Color textColor = Color.Black;
		public float textScale = 1;
		public Vector2 textPosition = new(4, 2);
		public int visibleTextCount = 10;
		
		public TextBox()
		{
			SetPadding(0);
			BackgroundColor = Color.White;
			BorderColor = Color.White;
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
			if (!ContainsPoint(Main.MouseScreen) && (Main.mouseLeft || Main.mouseRight)) // This solution is fine, but we need a way to cleanly "unload" a UIElement
			{
				// TODO, figure out how to refocus without triggering unfocus while clicking enable button.
				Unfocus();
			}
			base.Update(gameTime);
		}

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
		
		private static bool JustPressed(Keys key)
		{
			return Main.inputText.IsKeyDown(key) && !Main.oldInputText.IsKeyDown(key);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			var hitbox = GetInnerDimensions().ToRectangle();
			
			base.DrawSelf(spriteBatch);

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
				spriteBatch.DrawString(FontAssets.MouseText.Value, hintText, drawPos, color, 0, new(0, 0), textScale, SpriteEffects.None, 0);

			}
			else
			{
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
		}
	}
}