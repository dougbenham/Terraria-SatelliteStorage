using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SatelliteStorage.Utils
{
    public class ItemSerializer
    {
        public static TagCompound SaveItem(Item item)
        {
            var tag = new TagCompound();
            tag["type"] = item.type;
            tag["stack"] = item.stack;
            tag["prefix"] = item.prefix;
            return tag;
        }

        public static Item LoadItem(TagCompound tag)
        {
            var item = new Item();
            item.SetDefaults(tag.GetInt("type"));
            item.stack = tag.GetInt("stack");
            var prefix = tag.GetInt("prefix");
            if (prefix != 0) item.prefix = prefix;
            return item;
        }

        public static ModPacket WriteItemsToPacket(List<Item> items, ModPacket packet)
        {
            packet.Write7BitEncodedInt(items.Count);

            for(var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                packet.Write7BitEncodedInt(item.type);
                packet.Write7BitEncodedInt(item.stack);
                packet.Write7BitEncodedInt(item.prefix);
            }

            return packet;
        }

        public static List<Item> ReaderToItems(BinaryReader reader)
        {
            var items = new List<Item>();

            var count = reader.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                var item = new Item();
                item.type = reader.ReadInt32();
                item.stack = reader.ReadInt32();
                item.prefix = reader.ReadInt32();
            }

            return items;
        }
    }
}
