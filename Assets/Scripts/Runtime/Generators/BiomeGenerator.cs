using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
	public World world;

	[Header("Biome Generator Settings")]
	[Tooltip("The biome used when no biome meets the spawning criteria")]
	public Biome defaultBiome; // The biome used when no biome meets the spawning criteria
	public Biome[] biomes; // Get this working so modifying the biomes and such is easier than writing continuous switch case statements

	[Header("Perlin Noise Settings")]
	[Tooltip("NOTE: If the texture isn't the resolution of planet size*chunkSize then the texture will repeat")]
	public Texture2D biomeMap; // DOESN'T WORK YET
	[Space]
	public int seed;
	public float scale;
	public int octaves = 6;
	public float persistance = 0.5f;
	public float lacunarity = 2f;
	public float step;

	void Start()
	{
	}

	public float[,] GenerateBiomeNoiseMap(int width, int height, Vector2Int chunkPos)
	{
		return PerlinNoise.GeneratePerlinNoise(width, height, seed, scale, 6, new Vector2(chunkPos.x, chunkPos.y) * world.chunkSize, persistance, lacunarity, step, PerlinNoise.NormalizeMode.Global);
	}

	public Biome getBiome(float noiseVal, float height, float heat)
	{
		foreach (Biome biome in biomes)
		{
			if (height >= biome.minHeight && height <= biome.maxHeight)
			{
				if (heat >= biome.minTemp && heat <= biome.maxTemp)
				{
					return biome;
				}
			}
		}
		return defaultBiome;
	}

	public BiomeLayer getBiomeLayer(Biome biome, float height, float heat)
	{
		foreach (BiomeLayer biomeLayer in biome.biomeLayers)
		{
			if (height >= biomeLayer.minHeight && height <= biomeLayer.maxHeight)
			{
				if (heat >= biomeLayer.minTemp && heat <= biomeLayer.maxTemp)
				{
					return biomeLayer;
				}
			}
		}
		return biome.defaultBiomeLayer;
	}
}