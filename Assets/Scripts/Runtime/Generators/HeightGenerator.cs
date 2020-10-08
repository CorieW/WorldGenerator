using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightGenerator : MonoBehaviour
{
	public World world;

	[Header("Perlin Noise Settings")]
	[Tooltip("NOTE: If the texture isn't the resolution of planet size*chunkSize then the texture will repeat")]
	public Texture2D heightMap; // DOESN'T WORK YET
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

	public float[,] GenerateHeightMap(int width, int height, Vector2Int chunkPos)
	{
		float[,] noiseMap = PerlinNoise.GeneratePerlinNoise(width, height, seed, scale, 6, new Vector2(chunkPos.x, chunkPos.y) * world.chunkSize, persistance, lacunarity, step, PerlinNoise.NormalizeMode.Global);

		// The below code basically adds a falloff to the height map, preventing the land from bordering the edges of the world
		int falloffEdgeDistance = 50;
		Vector2Int falloffCentreDistance = new Vector2Int(Mathf.RoundToInt(world.width / 2) - falloffEdgeDistance, Mathf.RoundToInt(world.height / 2) - falloffEdgeDistance); // The distance from the centre of the world at which the islands height starts to fall off to 0

		chunkPos = chunkPos - new Vector2Int(Mathf.RoundToInt(world.width / 2), Mathf.RoundToInt(world.height / 2));
		chunkPos = new Vector2Int(Mathf.Abs(chunkPos.x), Mathf.Abs(chunkPos.y)) - new Vector2Int(falloffCentreDistance.x, falloffCentreDistance.y);

		Vector2Int maxValue = new Vector2Int(Mathf.RoundToInt(world.width / 2), Mathf.RoundToInt(world.height / 2)) - new Vector2Int(falloffCentreDistance.x, falloffCentreDistance.y);
		Vector2 edgeClosenessVector = new Vector2((float)chunkPos.x / (float)maxValue.x, (float)chunkPos.y / (float)maxValue.y); // Convert it to a value between 0 and 1

		if (edgeClosenessVector.x < 0)
		{
			edgeClosenessVector = new Vector2(0, edgeClosenessVector.y);
		}
		if (edgeClosenessVector.y < 0)
		{
			edgeClosenessVector = new Vector2(edgeClosenessVector.x, 0);
		}

		// Finding the length of a diagonal line
		// FORMULAE = A^2 = B^2 + C^2
		float edgeCloseness = (edgeClosenessVector.x * edgeClosenessVector.x) + (edgeClosenessVector.y * edgeClosenessVector.y);
		edgeCloseness = Mathf.Sqrt(edgeCloseness);

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - edgeCloseness);
			}
		}

		return noiseMap;
	}
}

// WORKING BUT UGLY HEIGHT MAP FALLOFF CODE
// - Could improve this by adding some sort of noise to the falloff, therefore it'll be impossible or very unlikely to get any straight cut-offs because of the falloff