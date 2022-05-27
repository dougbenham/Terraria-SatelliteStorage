using Terraria;
using Terraria.ModLoader;

namespace SatelliteStorage.Global
{
    class SatelliteStorageGlobalItem : GlobalItem
    {
        public override bool OnPickup(Item item, Player player)
        {
            if (SatelliteStorage.GetUIState((int)UI.UITypes.DriveChest))
            {
                DriveSystem.DriveChestSystem.checkRecipesRefresh = false;
            }

            return base.OnPickup(item, player);
        }

        public override void OnConsumeItem(Item item, Player player)
        {
            if (SatelliteStorage.GetUIState((int)UI.UITypes.DriveChest))
            {
                DriveSystem.DriveChestSystem.checkRecipesRefresh = false;
            }

            base.OnConsumeItem(item, player);
        }
    }
}
