using System;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;

namespace SatelliteStorage.UIElements
{
    class TextButton : UIPanel
    {
        public string text;
        public float textScale;
        private float scaleOffset;
        private UIText textElement;
        private bool mouseOver;

        public TextButton(string text)
        {
            this.text = text;
            textScale = 1;

            Width.Set(MathF.Round(100 * text.Length/10), 0);
            Height.Set(MathF.Round(30 * text.Length / 10), 0);

            BackgroundColor = new(0, 0, 0, 0);
            BorderColor = new(0, 0, 0, 0);

            OnMouseOver += (_, _) =>
            {
                if (mouseOver) return;
                SoundEngine.PlaySound(SoundID.MenuTick);
                mouseOver = true;
            };

            OnMouseOut += (_, _) =>
            {
                if (!mouseOver) return;
                mouseOver = false;
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var color = Color.White;
            if (mouseOver)
            {
                if (scaleOffset < (float)textScale * 0.2f) scaleOffset += (textScale * 0.05f);
                color = new(255, 231, 69, 255);
            }
            else
            {
                if (scaleOffset > 0) scaleOffset -= (textScale * 0.05f);
            }

            if (textElement != null)
            {
                textElement.Deactivate(); 
                RemoveChild(textElement);
            }
            
            textElement = new(text, textScale + scaleOffset);
            //textElement.DynamicallyScaleDownToWidth = true;
            textElement.VAlign = 0.5f;
            //textElement.HAlign = 0.5f;
            textElement.Left.Set(0, 0);
            textElement.TextColor = color;
            textElement.TextOriginX = 0;


            Append(textElement);
        }
    }
}
