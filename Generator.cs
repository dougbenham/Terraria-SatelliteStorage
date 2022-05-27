using System.Collections.Generic;
using SatelliteStorage.Utils;

namespace SatelliteStorage;

public class Generator
{
	private readonly List<int> _dropsChances = new();

	public readonly int Chance;
	public readonly List<int[]> Drops = new();

	public Generator(int chance)
	{
		this.Chance = chance;
	}

	public Generator AddDrop(int type, int count, int chance, int chanceType)
	{
		Drops.Add(new int[4] { type, count, chance, chanceType });
		_dropsChances.Add(chance);
		return this;
	}

	public int GetRandomDropIndex()
	{
		var index = RandomUtils.Roulette(_dropsChances);
		return index;
	}

	public int[] GetDropData(int index) // 0 - type, 1 - count, 2 - chance
	{
		return Drops[index];
	}
}