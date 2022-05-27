using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;

namespace SatelliteStorage.UIElements
{
    class UiCraftResultBg : UIPanel
    {
        public static bool Hidden = true;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Hidden) return;
            base.Draw(spriteBatch);
        }
    }
}
