using System;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;

namespace SatelliteStorage.UIElements
{
    class TextButton : UIPanel
    {
        public string Text;
        public float TextScale;
        private float _scaleOffset;
        private UIText _textElement;
        private bool _mouseOver;

        public TextButton(string text)
        {
            this.Text = text;
            TextScale = 1;

            Width.Set(MathF.Round(100 * text.Length/10), 0);
            Height.Set(MathF.Round(30 * text.Length / 10), 0);

            BackgroundColor = new(0, 0, 0, 0);
            BorderColor = new(0, 0, 0, 0);

            OnMouseOver += (_, _) =>
            {
                if (_mouseOver) return;
                SoundEngine.PlaySound(SoundID.MenuTick);
                _mouseOver = true;
            };

            OnMouseOut += (_, _) =>
            {
                if (!_mouseOver) return;
                _mouseOver = false;
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var color = Color.White;
            if (_mouseOver)
            {
                if (_scaleOffset < (float)TextScale * 0.2f) _scaleOffset += (TextScale * 0.05f);
                color = new(255, 231, 69, 255);
            }
            else
            {
                if (_scaleOffset > 0) _scaleOffset -= (TextScale * 0.05f);
            }

            if (_textElement != null)
            {
                _textElement.Deactivate(); 
                RemoveChild(_textElement);
            }
            
            _textElement = new(Text, TextScale + _scaleOffset);
            //textElement.DynamicallyScaleDownToWidth = true;
            _textElement.VAlign = 0.5f;
            //textElement.HAlign = 0.5f;
            _textElement.Left.Set(0, 0);
            _textElement.TextColor = color;
            _textElement.TextOriginX = 0;


            Append(_textElement);
        }
    }
}
