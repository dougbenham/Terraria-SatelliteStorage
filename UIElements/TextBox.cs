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
		internal bool Focused;
		private int _maxLength = 60;
		public string HintText = "";
		public string CurrentString = "";
		private int _textBlinkerCount;
		private int _textBlinkerState;
		public event Action OnFocus;
		public event Action OnUnfocus;
		public event Action OnTextChanged;
		public event Action OnTabPressed;
		public event Action OnEnterPressed;
		internal bool UnfocusOnEnter = true;
		internal bool UnfocusOnTab = true;
		public Color TextColor = Color.Black;
		public float TextScale = 1;
		public Vector2 TextPosition = new(4, 2);
		public int VisibleTextCount = 10;
		
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
			if (Focused)
			{
				Focused = false;
				Main.blockInput = false;

				OnUnfocus?.Invoke();
			}
		}

		public void Focus()
		{
			if (!Focused)
			{
				Main.clrInput();
				Focused = true;
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
			if (CurrentString != text)
			{
				CurrentString = text;
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

			if (Focused)
			{
				PlayerInput.WritingText = true;
				Main.instance.HandleIME();
				var newString = Main.GetInputText(CurrentString);
				if (!newString.Equals(CurrentString))
				{
					CurrentString = newString;
					OnTextChanged?.Invoke();
				}
				else
				{
					CurrentString = newString;
				}

				if (JustPressed(Keys.Tab))
				{
					if (UnfocusOnTab) Unfocus();
					OnTabPressed?.Invoke();
				}
				if (JustPressed(Keys.Enter))
				{
					Main.drawingPlayerChat = false;
					if (UnfocusOnEnter) Unfocus();
					OnEnterPressed?.Invoke();
				}
				if (++_textBlinkerCount >= 20)
				{
					_textBlinkerState = (_textBlinkerState + 1) % 2;
					_textBlinkerCount = 0;
				}
				Main.instance.DrawWindowsIMEPanel(new(98f, Main.screenHeight - 36));
			}
			var displayString = CurrentString;
			var space = GetDimensions();
			var color = TextColor;
			if (CurrentString.Length == 0)
			{
			}
			var drawPos = space.Position() + TextPosition;
			if (CurrentString.Length == 0 && !Focused)
			{
				color *= 0.5f;
				spriteBatch.DrawString(FontAssets.MouseText.Value, HintText, drawPos, color, 0, new(0, 0), TextScale, SpriteEffects.None, 0);

			}
			else
			{
				var displayValue = displayString;

				
				if (displayValue.Length > VisibleTextCount+1)
                {
					var substFrom = displayString.Length - VisibleTextCount - 1;
					if (substFrom <= 0) substFrom = 0;
					var substCount = VisibleTextCount;
					if (substCount <= 0) substCount = 0;

					displayValue = displayValue.Substring(substFrom, substCount);
				}

				if (_textBlinkerState == 1 && Focused)
				{
					displayValue = displayValue + "|";
				}
				
				spriteBatch.DrawString(FontAssets.MouseText.Value, displayValue, drawPos, color, 0, new(0,0), TextScale, SpriteEffects.None, 0);
			}
		}
	}
}