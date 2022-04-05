using System.Linq;
using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.IO;
using Terraria.WorldBuilding;
using System;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;

namespace SatelliteStorage.UI
{
    public class BaseUIState : UIState
    {
        public virtual void OnUpdateUI(GameTime gameTime)
        {
        }

        public virtual void OnModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
        }

        public virtual void SetState(bool state)
        {
        }

        public virtual bool GetState()
        {
            return false;
        }
    }
}
