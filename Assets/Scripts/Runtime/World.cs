using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class World : MonoBehaviour
{
	public int tileSize;

	[Header("World Properties")]
	public int width; // Maximum for this could be 1000
	public int height; // Maximum for this could be 1000
	public int chunkSize;

	[Header("References")]
	public WorldMap worldMap;

	[Space]
	public HeightGenerator heightGenerator;
	public ClimateGenerator climateGenerator;
	public BiomeGenerator biomeGenerator;
	public FeatureGenerator featureGenerator;

	// Start is called before the first frame update
	void Start()
	{
		if (chunkSize < 1) // Can't go under 1 because a chunk needs to have atleast 1 block, otherwise it's pointless
		{
			chunkSize = 1;
		}
	}

	// At the moment it just generates a simple chunk, probably fine performance-wise
	public SimpleChunk GetSimpleChunkAt(Vector2Int chunkPos) // TODO: Pull from memory and storage
	{
		float[,] chunkHeightMap = heightGenerator.GenerateHeightMap(1, 1, new Vector2Int(chunkPos.x, chunkPos.y));
		float[,] chunkClimateMap = climateGenerator.GenerateClimateNoiseMap(1, 1, new Vector2Int(chunkPos.x, chunkPos.y));
		float[,] chunkBiomeMap = biomeGenerator.GenerateBiomeNoiseMap(1, 1, new Vector2Int(chunkPos.x, chunkPos.y));
		float[,] chunkFeatureMap = featureGenerator.GenerateObjectNutrientsMap(1, 1, new Vector2Int(chunkPos.x, chunkPos.y));

		Biome biome = biomeGenerator.getBiome(chunkBiomeMap[0, 0], chunkHeightMap[0, 0], chunkClimateMap[0, 0]);
		BiomeLayer biomeLayer = biomeGenerator.getBiomeLayer(biome, chunkHeightMap[0, 0], chunkClimateMap[0, 0]);
		FeatureGroup featureGroup = featureGenerator.getFeatureGroup(chunkFeatureMap[0, 0], biomeLayer, Vector2.zero);

		return new SimpleChunk(biome, biomeLayer);
	}

	// At the moment it just generates the default chunk at the chunk pos (Misses out the features)
	public Chunk GetChunkAt(Vector2Int chunkPos) // TODO: Pull from memory and storage
	{
		Chunk chunk = new Chunk(this, chunkPos);

		float[,] chunkHeightMap = heightGenerator.GenerateHeightMap(chunkSize, chunkSize, chunkPos);
		float[,] chunkClimateMap = climateGenerator.GenerateClimateNoiseMap(chunkSize, chunkSize, chunkPos);
		float[,] chunkBiomeMap = biomeGenerator.GenerateBiomeNoiseMap(chunkSize, chunkSize, chunkPos);

		for (int chunkTileY = 0; chunkTileY < chunkSize; chunkTileY++)
		{
			for (int chunkTileX = 0; chunkTileX < chunkSize; chunkTileX++)
			{
				Biome biome = biomeGenerator.getBiome(chunkBiomeMap[chunkTileX, chunkTileY], chunkHeightMap[chunkTileX, chunkTileY], chunkClimateMap[chunkTileX, chunkTileY]);
				BiomeLayer biomeLayer = biomeGenerator.getBiomeLayer(biome, chunkHeightMap[chunkTileX, chunkTileY], chunkClimateMap[chunkTileX, chunkTileY]);

				chunk.tiles[chunkTileX, chunkTileY] = biomeLayer.groundTile;
			}
		}

		return chunk;
	}
}