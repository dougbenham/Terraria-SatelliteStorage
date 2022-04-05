using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SatelliteStorage.DriveSystem;
using System.Collections.Generic;

namespace SatelliteStorage.Global
{
    class SatelliteStorageGlobalRecipe : GlobalRecipe
    {
        /*
        public override bool RecipeAvailable(Recipe recipe)
        {
            if (!SatelliteStorage.GetUIState((int)UI.UITypes.DriveChest)) return base.RecipeAvailable(recipe);

            bool defaultValue = base.RecipeAvailable(recipe);

            DriveChestSystem.GetItems().ForEach(DriveItem =>
            {
                recipe.requiredItem.ForEach(c =>
                {
                   // if (c.type == DriveItem.type) recipe. //recipe.AddIngredient(DriveItem.type, DriveItem.stack);
                });
            });
            SatelliteStorage.Debug(recipe.createItem.Name);

            //recipe.requiredItem.Find(v => v.type )
            return defaultValue;
        }
        */

        public override void OnCraft(Item item, Recipe recipe)
        {
            /*
            if (SatelliteStorage.GetUIState((int)UI.UITypes.DriveChest))
            {
                Player player = Main.LocalPlayer;

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    List<RecipeItemsUses> uses = DriveChestSystem.GetItemsUsesForCraft(player.inventory, DriveChestSystem.oldPlayerInv, recipe);
                    uses.ForEach(u =>
                    {
                        Item item = new Item();
                        item.type = u.type;
                        item.SetDefaults(item.type);
                        item.stack = 1;

                        if (u.from == 0)
                        {
                            player.inventory[u.slot].stack -= u.stack;
                            if (player.inventory[u.slot].stack <= 0) player.inventory[u.slot].TurnToAir();
                        }
                        else
                        {
                            DriveChestSystem.SubItem(u.type, u.stack);
                        }

                        //SatelliteStorage.Debug(item.Name + "("+u.stack+") from "+ u.from + (u.from == 0 ? (" at slot " + u.slot) : ""));
                    });

                }

                DriveChestSystem.ResearchRecipes();
                return;
            }
            */
            base.OnCraft(item, recipe);
        }
    }


}
