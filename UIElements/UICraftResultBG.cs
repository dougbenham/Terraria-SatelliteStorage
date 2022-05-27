using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;

namespace SatelliteStorage.UIElements
{
    class UICraftResultBG : UIPanel
    {
        public static bool hidden = true;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (hidden) return;
            base.Draw(spriteBatch);
        }
    }
}
