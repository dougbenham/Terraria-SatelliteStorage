using System.Collections.Generic;
using System.IO;
using SatelliteStorage.DriveSystem;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SatelliteStorage.Utils
{
    public class DriveItemsSerializer
    {
        public static TagCompound SaveDriveItem(DriveItem item)
        {
            var tag = new TagCompound();
            tag["type"] = item.Type;
            tag["stack"] = item.Stack;
            tag["prefix"] = item.Prefix;
            return tag;
        }

        public static DriveItem LoadDriveItem(TagCompound tag)
        {
            var item = new DriveItem();
            item.Type = tag.GetInt("type");
            item.Stack = tag.GetInt("stack");
            var prefix = tag.GetInt("prefix");
            if (prefix != 0) item.Prefix = prefix;
            return item;
        }

        public static ModPacket WriteDriveItemsToPacket(List<DriveItem> items, ModPacket packet)
        {
            packet.Write7BitEncodedInt(items.Count);

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                packet.Write7BitEncodedInt(item.Type);
                packet.Write7BitEncodedInt(item.Stack);
                packet.Write7BitEncodedInt(item.Prefix);
            }

            return packet;
        }

        public static List<DriveItem> ReadDriveItems(BinaryReader reader)
        {
            var items = new List<DriveItem>();

            var count = reader.Read7BitEncodedInt();

            for (var i = 0; i < count; i++)
            {
                var item = new DriveItem();
                item.Type = reader.Read7BitEncodedInt();
                item.Stack = reader.Read7BitEncodedInt();
                item.Prefix = reader.Read7BitEncodedInt();
                items.Add(item);
            }

            return items;
        }
    }
}
