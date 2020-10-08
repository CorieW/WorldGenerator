using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimateGenerator : MonoBehaviour
{
	public World world;

	[Header("Climate Properties")]
	[Tooltip("-1 = Freezing temperatures, 1 = Very hot temperatures")]
	[Range(-1, 1)]
	public float averageTemperatureScaler;

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

	public float[,] GenerateClimateNoiseMap(int width, int height, Vector2Int chunkPos)
	{
		float[,] climateMap = PerlinNoise.GeneratePerlinNoise(width, height, seed, scale, 6, new Vector2(chunkPos.x, chunkPos.y) * world.chunkSize, persistance, lacunarity, step, PerlinNoise.NormalizeMode.Global);

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				climateMap[x, y] = Mathf.Clamp(climateMap[x, y] + averageTemperatureScaler, 0, 1);
			}
		}

		return climateMap;
	}
}