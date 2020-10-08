using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FeatureGenerator : MonoBehaviour
{
	public World world;

	public System.Random rnd = new System.Random();

	[Header("Perlin Noise Settings")]
	[Tooltip("NOTE: If the texture isn't the resolution of planet size*chunkSize then the texture will repeat")]
	public Texture2D biomeMap; // DOESN'T WORK YET
	[Space]

	public int seed;
	public float scale;P
	public int octaves = 6;
	public float persistance = 0.5f;
	public float lacunarity = 2f;
	public float step;

	void Start()
	{
	}

	public float[,] GenerateObjectNutrientsMap(int width, int height, Vector2Int chPunkPos)
	{
		return PerlinNoise.GeneratePerlinNoise(width, height, seed, scale, 6, new Vector2(chunkPos.x, chunkPos.y) * world.chunkSize, persistance, lacunarity, step, PerlinNoise.NormalizeMode.Global);
	}

	public FeatureGroup getFeatureGroup(float nutrients, BiomeLayer biomeLayer, Vector2 worldPos)
	{
		foreach (FeatureGroup featureGroup in biomeLayer.featureGroups)
		{
			if (nutrients >= featureGroup.minAllowedNutrients && nutrients <= featureGroup.maxAllowedNutrients)
			{
				return featureGroup;
			}
		}
		return null;
	}

	public SpawnableFeature getSpawnableFeature(float nutrients, BiomeLayer biomeLayer, Vector2Int worldPos)
	{
		int worldWidthSize = world.width * world.chunkSize;

		foreach (FeatureGroup featureGroup in biomeLayer.featureGroups)
		{
			if (nutrients >= featureGroup.minAllowedNutrients && nutrients <= featureGroup.maxAllowedNutrients)
			{
				foreach (SpawnableFeature spawnableFeature in featureGroup.spawnableFeatures)
				{
					float chance = rnd.Next(100000000);
					chance = chance / 100000000;

					int rndSpacing = rnd.Next(spawnableFeature.spacing.y);
					rndSpacing = Mathf.Clamp(rndSpacing, 1, rndSpacing); // Prevents the spacing from being below 1

					if (worldPos.x % rndSpacing == 0 && worldPos.y % rndSpacing == 0)
					{
						if (chance <= spawnableFeature.spawnChance)
						{
							return spawnableFeature;
						}
					}
				}
			}
		}
		
		return null;
	}
}
// To note: Random chance was taken away from feature generation because the random function requires time to generate correctly.
// There could be a solution there by generating giving a seed that is just the same as the sum of the position of the tile

// Use noise scale as a grouping consistency for where trees are placed. If low consistency, the trees will be randomly dotted.
// Group size
// Distance in between each other
// Chance of spawning