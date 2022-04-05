using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;


namespace SatelliteStorage.Items
{
    
    class HellstoneGeneratorItem : BaseItemsGeneratorItem
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();
            Item.value = 500;
            Item.createTile = ModContent.TileType<Tiles.HellstoneGeneratorTile>();
            Item.rare = 2;
            generatorType = (byte)SatelliteStorage.GeneratorTypes.HellstoneGenerator;
        }

        public override void AddGeneratorRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Items.QuartzModule>(), 1)
            .AddIngredient(ItemID.HellstoneBar, 25)
            .AddTile(TileID.Anvils)
            .Register();
        }
    } 
    
    class MeteoriteGeneratorItem : BaseItemsGeneratorItem
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();
            Item.value = 500;
            Item.createTile = ModContent.TileType<Tiles.MeteoriteGeneratorTile>();
            Item.rare = 1;
            generatorType = (byte)SatelliteStorage.GeneratorTypes.MeteoriteGenerator;
        }

        public override void AddGeneratorRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Items.QuartzModule>(), 1)
            .AddIngredient(ItemID.MeteoriteBar, 25)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }    
    
    class ShroomiteGeneratorItem : BaseItemsGeneratorItem
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();
            Item.value = 500;
            Item.createTile = ModContent.TileType<Tiles.ShroomiteGeneratorTile>();
            Item.rare = 7;
            generatorType = (byte)SatelliteStorage.GeneratorTypes.ShroomiteGenerator;
        }

        public override void AddGeneratorRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Items.QuartzModule>(), 1)
            .AddIngredient(ItemID.ShroomiteBar, 25)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }    
    
    class SpectreGeneratorItem : BaseItemsGeneratorItem
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();
            Item.value = 500;
            Item.createTile = ModContent.TileType<Tiles.SpectreGeneratorTile>();
            Item.rare = 7;
            generatorType = (byte)SatelliteStorage.GeneratorTypes.SpectreGenerator;
        }

        public override void AddGeneratorRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Items.QuartzModule>(), 1)
            .AddIngredient(ItemID.SpectreBar, 25)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }    
    
    class LuminiteGeneratorItem : BaseItemsGeneratorItem
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();
            Item.value = 500;
            Item.createTile = ModContent.TileType<Tiles.LuminiteGeneratorTile>();
            Item.rare = 10;
            generatorType = (byte)SatelliteStorage.GeneratorTypes.LuminiteGenerator;
        }

        public override void AddGeneratorRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Items.QuartzModule>(), 1)
            .AddIngredient(ItemID.LunarBar, 25)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }    
    
    class ChlorophyteGeneratorItem : BaseItemsGeneratorItem
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();
            Item.value = 500;
            Item.createTile = ModContent.TileType<Tiles.ChlorophyteGeneratorTile>();
            Item.rare = 7;
            generatorType = (byte)SatelliteStorage.GeneratorTypes.ChlorophyteGenerator;
        }

        public override void AddGeneratorRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Items.QuartzModule>(), 1)
            .AddIngredient(ItemID.ChlorophyteBar, 25)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }    
    
    class HallowedGeneratorItem : BaseItemsGeneratorItem
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();
            Item.value = 500;
            Item.createTile = ModContent.TileType<Tiles.HallowedGeneratorTile>();
            Item.rare = 4;
            generatorType = (byte)SatelliteStorage.GeneratorTypes.HallowedGenerator;
        }

        public override void AddGeneratorRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Items.QuartzModule>(), 1)
            .AddIngredient(ItemID.HallowedBar, 25)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }

    class SoulGeneratorItem : BaseItemsGeneratorItem
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();
            Item.value = 500;
            Item.createTile = ModContent.TileType<Tiles.SoulGeneratorTile>();
            Item.rare = 3;
            generatorType = (byte)SatelliteStorage.GeneratorTypes.SoulGenerator;
        }

        public override void AddGeneratorRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Items.QuartzModule>(), 1)
            .AddIngredient(ModContent.ItemType<Items.SoulBar>(), 25)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }

    class PowerGeneratorItem : BaseItemsGeneratorItem
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();
            Item.value = 500;
            Item.createTile = ModContent.TileType<Tiles.PowerGeneratorTile>();
            Item.rare = 5;
            generatorType = (byte)SatelliteStorage.GeneratorTypes.PowerGenerator;
        }

        public override void AddGeneratorRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Items.QuartzModule>(), 1)
            .AddIngredient(ModContent.ItemType<Items.PowerBar>(), 25)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
