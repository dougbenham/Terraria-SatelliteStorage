using Terraria.ModLoader;
using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader.IO;
using Terraria.ID;
using log4net;

namespace SatelliteStorage.Utils
{
    public class ItemSerializer
    {
        public static TagCompound SaveItem(Item item)
        {
            TagCompound tag = new TagCompound();
            tag["type"] = item.type;
            tag["stack"] = item.stack;
            tag["prefix"] = item.prefix;
            return tag;
        }

        public static Item LoadItem(TagCompound tag)
        {
            Item item = new Item();
            item.SetDefaults(tag.GetInt("type"));
            item.stack = tag.GetInt("stack");
            int prefix = tag.GetInt("prefix");
            if (prefix != 0) item.prefix = prefix;
            return item;
        }

        public static ModPacket WriteItemsToPacket(List<Item> items, ModPacket packet)
        {
            packet.Write7BitEncodedInt(items.Count);

            for(int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                packet.Write7BitEncodedInt(item.type);
                packet.Write7BitEncodedInt(item.stack);
                packet.Write7BitEncodedInt(item.prefix);
            }

            return packet;
        }

        public static List<Item> ReaderToItems(BinaryReader reader)
        {
            List<Item> items = new List<Item>();

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                Item item = new Item();
                item.type = reader.ReadInt32();
                item.stack = reader.ReadInt32();
                item.prefix = reader.ReadInt32();
            }

            return items;
        }
    }
}
