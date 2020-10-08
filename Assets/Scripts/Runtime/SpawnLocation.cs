using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpawnLocation // This script can be used to check whether someone or something can spawn somewhere
{
	// Randomly searches world for a chunk that the player can spawn at (Chances of this being slow in some situations)
	public static Vector2Int getRandomSpawnTile(World world, Biome[] allowedBiomes, Tile[] allowedTiles)
	{
		bool[,] checkedChunks = new bool[world.width, world.height];
		System.Random rnd = new System.Random();

		while (true)
		{
			int rndX = rnd.Next(world.width);
			int rndY = rnd.Next(world.height);

			if (checkedChunks[rndX, rndY]) // Skip checking this chunk if it has already been checked
			{
				continue;
			}

			SimpleChunk simpleChunk = world.GetSimpleChunkAt(new Vector2Int(rndX, rndY));

			if (doesBiomesContain(allowedBiomes, simpleChunk.biome)) // Is this a spawnable biome?
			{
				Chunk chunk = world.GetChunkAt(new Vector2Int(rndX, rndY));

				for (int y = 0; y < world.chunkSize; y++)
				{
					for (int x = 0; x < world.chunkSize; x++)
					{
						if (doesTilesContain(allowedTiles, chunk.tiles[x, y]))
						{
							return new Vector2Int((rndX * world.chunkSize) + x, (rndY * world.chunkSize) + y);
						}
					}
				}
			}
		}
	}

	static bool doesBiomesContain(Biome[] biomes, Biome searchedBiome)
	{
		foreach(Biome biome in biomes)
		{
			if(searchedBiome.name == biome.name)
			{
				return true;
			}
		}

		return false;
	}

	static bool doesTilesContain(Tile[] tiles, Tile searchedTile)
	{
		foreach (Tile tile in tiles)
		{
			if (searchedTile.name == tile.name)
			{
				return true;
			}
		}

		return false;
	}
}
