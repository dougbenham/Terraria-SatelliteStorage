using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace SatelliteStorage.Tiles
{
    class HellstoneGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<Items.HellstoneGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.HellstoneGenerator;
        }
    }

    class MeteoriteGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<Items.MeteoriteGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.MeteoriteGenerator;
        }
    }
    
    class ShroomiteGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<Items.ShroomiteGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.ShroomiteGenerator;
        }
    }    
    
    class SpectreGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<Items.SpectreGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.SpectreGenerator;
        }
    }

    class LuminiteGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<Items.LuminiteGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.LuminiteGenerator;
        }
    }
    
    class ChlorophyteGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<Items.ChlorophyteGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.ChlorophyteGenerator;
        }
    }
    
    class HallowedGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<Items.HallowedGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.HallowedGenerator;
        }
    }
    
    class SoulGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<Items.SoulGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.SoulGenerator;
        }
    }

    class PowerGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<Items.PowerGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.PowerGenerator;
        }
    }
}
