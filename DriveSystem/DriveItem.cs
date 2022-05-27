using SatelliteStorage.Utils;
using Terraria;

namespace SatelliteStorage.DriveSystem
{
	public class DriveItem
	{
		private int _stack;

		public string StackText { get; private set; } = "0";

		public int Stack
		{
			get => _stack;
			set
			{
				_stack = value;
				StackText = StringUtils.GetStackCount(_stack);
			}
		}

		public int type;
		public int prefix;
		public int context = 0;
		public int recipe = -1;

		public DriveItem()
		{
			Stack = 0;
		}
		
		public static DriveItem FromItem(Item item)
		{
			var driveItem = new DriveItem();
			driveItem.Stack = item.stack;
			driveItem.type = item.type;
			driveItem.prefix = item.prefix;
			return driveItem;
		}

		public Item ToItem()
		{
			var item = new Item();
			item.type = type;
			item.SetDefaults(item.type);
			item.stack = Stack;
			if (item.stack > item.maxStack)
				item.stack = item.maxStack;
			item.prefix = prefix;
			return item;
		}
	}
}