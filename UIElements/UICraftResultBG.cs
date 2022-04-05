using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.UI.States;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using SatelliteStorage.DriveSystem;
using Terraria.GameContent.UI.Elements;
using Terraria;
using Terraria.ModLoader;

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
