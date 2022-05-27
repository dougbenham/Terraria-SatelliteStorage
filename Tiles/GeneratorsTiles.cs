using SatelliteStorage.Items;
using Terraria.ModLoader;

namespace SatelliteStorage.Tiles
{
    class HellstoneGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            ItemDrop = ModContent.ItemType<HellstoneGeneratorItem>();
            GeneratorType = (byte)SatelliteStorage.GeneratorTypes.HellstoneGenerator;
        }
    }

    class MeteoriteGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            ItemDrop = ModContent.ItemType<MeteoriteGeneratorItem>();
            GeneratorType = (byte)SatelliteStorage.GeneratorTypes.MeteoriteGenerator;
        }
    }
    
    class ShroomiteGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            ItemDrop = ModContent.ItemType<ShroomiteGeneratorItem>();
            GeneratorType = (byte)SatelliteStorage.GeneratorTypes.ShroomiteGenerator;
        }
    }    
    
    class SpectreGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            ItemDrop = ModContent.ItemType<SpectreGeneratorItem>();
            GeneratorType = (byte)SatelliteStorage.GeneratorTypes.SpectreGenerator;
        }
    }

    class LuminiteGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            ItemDrop = ModContent.ItemType<LuminiteGeneratorItem>();
            GeneratorType = (byte)SatelliteStorage.GeneratorTypes.LuminiteGenerator;
        }
    }
    
    class ChlorophyteGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            ItemDrop = ModContent.ItemType<ChlorophyteGeneratorItem>();
            GeneratorType = (byte)SatelliteStorage.GeneratorTypes.ChlorophyteGenerator;
        }
    }
    
    class HallowedGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            ItemDrop = ModContent.ItemType<HallowedGeneratorItem>();
            GeneratorType = (byte)SatelliteStorage.GeneratorTypes.HallowedGenerator;
        }
    }
    
    class SoulGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            ItemDrop = ModContent.ItemType<SoulGeneratorItem>();
            GeneratorType = (byte)SatelliteStorage.GeneratorTypes.SoulGenerator;
        }
    }

    class PowerGeneratorTile : BaseItemsGeneratorTile
    {
        public override void SetGeneratorDefaults()
        {
            base.SetGeneratorDefaults();

            ItemDrop = ModContent.ItemType<PowerGeneratorItem>();
            GeneratorType = (byte)SatelliteStorage.GeneratorTypes.PowerGenerator;
        }
    }
}
