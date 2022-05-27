using Terraria;

namespace SatelliteStorage.DriveSystem
{
    public class DriveItem
    {
        private int _stack;
        private string _stackText = "0";
        public string stackText
        {
            get
            {
                return _stackText;
            }
        }

        public int stack
        {
            get {
                return _stack;
            }
            set {
                _stack = value;
                _stackText = Utils.StringUtils.GetStackCount(_stack);
            }
        }

        public int type = 0;
        public int prefix = 0;
        public int context = 0;
        public int recipe = -1;


        public DriveItem()
        {
            stack = 0;
        }

        public DriveItem SetStack(int stack)
        {
            this.stack = stack;
            return this;
        }

        public DriveItem SetType(int type)
        {
            this.type = type;
            return this;
        }

        public DriveItem SetPrefix(int prefix)
        {
            this.prefix = prefix;
            return this;
        }

        public static DriveItem FromItem(Item item)
        {
            DriveItem driveItem = new DriveItem();
            driveItem.stack = item.stack;
            driveItem.type = item.type;
            driveItem.prefix = item.prefix;
            return driveItem;
        }

        public Item ToItem()
        {
            Item item = new Item();
            item.type = type;
            item.SetDefaults(item.type);
            item.stack = stack;
            if (item.stack > item.maxStack) item.stack = item.maxStack;
            item.prefix = prefix;
            return item;
        }
    }
}
