using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using SatelliteStorage.DriveSystem;

namespace SatelliteStorage.UIElements
{
    class DriveItemPanel : UIPanel
    {
        private UIItemIcon itemIcon;
        private UIText stackText;
        public Item item;

        public DriveItemPanel(DriveItem driveItem)
        {
            item = new Item();
            item.type = driveItem.type;
            item.SetDefaults(item.type);
            item.stack = driveItem.stack;
            item.prefix = driveItem.prefix;

            itemIcon = new UIItemIcon(item, false);

            itemIcon.VAlign = 0.5f;
            itemIcon.HAlign = 0.5f;

            BorderColor = new Color(0, 0, 0, 0);

            stackText = new UIText(Utils.StringUtils.GetStackCount(item.stack), 0.77f);
            stackText.Top.Set(12, 0);
            stackText.Left.Set(-6, 0);

            Append(itemIcon);
            Append(stackText);
        }
    }
}
