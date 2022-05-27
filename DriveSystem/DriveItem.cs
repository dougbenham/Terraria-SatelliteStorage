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

		public int Type;
		public int Prefix;
		public int Context = 0;
		public int Recipe = -1;

		public DriveItem()
		{
			Stack = 0;
		}
		
		public static DriveItem FromItem(Item item)
		{
			var driveItem = new DriveItem();
			driveItem.Stack = item.stack;
			driveItem.Type = item.type;
			driveItem.Prefix = item.prefix;
			return driveItem;
		}

		public Item ToItem()
		{
			var item = new Item();
			item.type = Type;
			item.SetDefaults(item.type);
			item.stack = Stack;
			if (item.stack > item.maxStack)
				item.stack = item.maxStack;
			item.prefix = Prefix;
			return item;
		}
	}
}