using SatelliteStorage.DriveSystem;
using SatelliteStorage.UI;
using Terraria;
using Terraria.ModLoader;

namespace SatelliteStorage.Global
{
    class SatelliteStorageGlobalItem : GlobalItem
    {
        public override bool OnPickup(Item item, Player player)
        {
            if (SatelliteStorage.GetUIState((int)UITypes.DriveChest))
            {
                DriveChestSystem.CheckRecipesRefresh = false;
            }

            return base.OnPickup(item, player);
        }

        public override void OnConsumeItem(Item item, Player player)
        {
            if (SatelliteStorage.GetUIState((int)UITypes.DriveChest))
            {
                DriveChestSystem.CheckRecipesRefresh = false;
            }

            base.OnConsumeItem(item, player);
        }
    }
}
