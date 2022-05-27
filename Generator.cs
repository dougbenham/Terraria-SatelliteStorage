using System.Collections.Generic;
using SatelliteStorage.Utils;

namespace SatelliteStorage;

public class Generator
{
	public int chance;

	private readonly List<int> dropsChances = new();
	public List<int[]> drops = new();

	public Generator(int chance)
	{
		this.chance = chance;
	}

	public Generator AddDrop(int type, int count, int chance, int chanceType)
	{
		drops.Add(new int[4] { type, count, chance, chanceType });
		dropsChances.Add(chance);
		return this;
	}

	public int GetRandomDropIndex()
	{
		var index = RandomUtils.Roulette(dropsChances);
		return index;
	}

	public int[] GetDropData(int index) // 0 - type, 1 - count, 2 - chance
	{
		return drops[index];
	}
}