using Terraria.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

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
