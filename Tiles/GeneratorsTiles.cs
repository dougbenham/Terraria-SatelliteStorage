using SatelliteStorage.Items;
using Terraria.ModLoader;

namespace SatelliteStorage.Tiles
{
    class HellstoneGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<HellstoneGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.HellstoneGenerator;
        }
    }

    class MeteoriteGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<MeteoriteGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.MeteoriteGenerator;
        }
    }
    
    class ShroomiteGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<ShroomiteGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.ShroomiteGenerator;
        }
    }    
    
    class SpectreGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<SpectreGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.SpectreGenerator;
        }
    }

    class LuminiteGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<LuminiteGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.LuminiteGenerator;
        }
    }
    
    class ChlorophyteGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<ChlorophyteGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.ChlorophyteGenerator;
        }
    }
    
    class HallowedGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<HallowedGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.HallowedGenerator;
        }
    }
    
    class SoulGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<SoulGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.SoulGenerator;
        }
    }

    class PowerGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            itemDrop = ModContent.ItemType<PowerGeneratorItem>();
            generatorType = (byte)SatelliteStorage.GeneratorTypes.PowerGenerator;
        }
    }
}
