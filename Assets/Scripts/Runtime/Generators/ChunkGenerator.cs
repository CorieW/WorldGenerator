using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
	Texture2D chunkTexture;

	World world;

	public Vector2Int chunkPos;
	public Chunk chunk;

	Texture2D[,] chunkTileTextures;

	BiomeLayer[,] chunkBiomeLayers;
	SpawnableFeature[,] chunkSpawnableFeatures;

	bool generated; // Has the chunk been generated

	public static ChunkGenerator Create(World world, Vector2Int chunkPos)
	{
		if (chunkPos.x >= 0 && chunkPos.x <= world.width)
		{
			if (chunkPos.y >= 0 && chunkPos.y <= world.height)
			{
				GameObject chunkObject = new GameObject();
				chunkObject.name = "Chunk(" + chunkPos.x + ", " + chunkPos.y + ")";
				float chunkWorldSpaceInterval = (float)(world.chunkSize * world.tileSize) / 100;
				chunkObject.transform.position = chunkPos * new Vector2(chunkWorldSpaceInterval, chunkWorldSpaceInterval);

				SpriteRenderer sr = chunkObject.AddComponent<SpriteRenderer>();
				sr.sortingOrder = -1;

				ChunkGenerator cg = chunkObject.AddComponent<ChunkGenerator>();
				cg.world = world;
				cg.chunkPos = chunkPos;
				cg.chunkBiomeLayers = new BiomeLayer[world.chunkSize, world.chunkSize];
				cg.chunkTileTextures = new Texture2D[world.chunkSize, world.chunkSize];
				cg.chunkSpawnableFeatures = new SpawnableFeature[world.chunkSize, world.chunkSize];

				cg.chunkTexture = new Texture2D(world.chunkSize * world.tileSize, world.chunkSize * world.tileSize, TextureFormat.ARGB32, false);
				cg.chunkTexture.filterMode = FilterMode.Point;
				sr.sprite = Sprite.Create(cg.chunkTexture, new Rect(Vector2.zero, new Vector2(world.chunkSize * world.tileSize, world.chunkSize * world.tileSize)), Vector2.zero);

				cg.chunk = new Chunk(world, chunkPos);

				return cg;
			}
		}
		return null;
	}

	void Start()
	{
		StartCoroutine(Generate());
	}

	IEnumerator Generate()
	{
		Thread thread = new Thread(GenerateChunkTileTextures);
		thread.Start();

		while (thread.IsAlive)
		{
			yield return new WaitForFixedUpdate();
		}

		DrawChunkTileTextures();

		thread = new Thread(GenerateChunkPopulation);
		thread.Start();

		while (thread.IsAlive)
		{
			yield return new WaitForFixedUpdate();
		}

		PopulateChunk();

		generated = true;
	}

	// Generates the tile textures to be applied to the chunk texture
	void GenerateChunkTileTextures()
	{ // NEEDS TO CHECK WHETHER THE STORAGE HAS ANYTHING STORED FOR THIS CHUNK
		float[,] chunkHeightMap = world.heightGenerator.GenerateHeightMap(world.chunkSize, world.chunkSize, chunkPos);
		float[,] chunkClimateMap = world.climateGenerator.GenerateClimateNoiseMap(world.chunkSize, world.chunkSize, chunkPos);
		float[,] chunkBiomeMap = world.biomeGenerator.GenerateBiomeNoiseMap(world.chunkSize, world.chunkSize, chunkPos);

		for (int chunkTileY = 0; chunkTileY < world.chunkSize; chunkTileY++)
		{
			for (int chunkTileX = 0; chunkTileX < world.chunkSize; chunkTileX++)
			{
				Biome biome = world.biomeGenerator.getBiome(chunkBiomeMap[chunkTileX, chunkTileY], chunkHeightMap[chunkTileX, chunkTileY], chunkClimateMap[chunkTileX, chunkTileY]);
				BiomeLayer biomeLayer = world.biomeGenerator.getBiomeLayer(biome, chunkHeightMap[chunkTileX, chunkTileY], chunkClimateMap[chunkTileX, chunkTileY]);

				chunkBiomeLayers[chunkTileX, chunkTileY] = biomeLayer;

				chunkTileTextures[chunkTileX, chunkTileY] = biomeLayer.groundTile.texture;
			}
		}
	}

	// Draws the tile textures onto the chunk texture
	void DrawChunkTileTextures()
	{
		for (int chunkTileY = 0; chunkTileY < world.chunkSize; chunkTileY++)
		{
			for (int chunkTileX = 0; chunkTileX < world.chunkSize; chunkTileX++)
			{
				chunkTexture.SetPixels(chunkTileX * world.chunkSize, chunkTileY * world.chunkSize, world.chunkSize, world.chunkSize, chunkTileTextures[chunkTileX, chunkTileY].GetPixels());
			}
		}

		chunkTexture.Apply();
	}

	// Generates the features to be instantiated as children of the chunk
	void GenerateChunkPopulation()
	{
		float[,] chunkFeatureMap = world.featureGenerator.GenerateObjectNutrientsMap(world.chunkSize, world.chunkSize, chunkPos);

		for (int chunkTileY = 0; chunkTileY < world.chunkSize; chunkTileY++)
		{
			for (int chunkTileX = 0; chunkTileX < world.chunkSize; chunkTileX++)
			{
				SpawnableFeature spawnableFeature = world.featureGenerator.getSpawnableFeature(chunkFeatureMap[chunkTileX, chunkTileY], chunkBiomeLayers[chunkTileX, chunkTileY], (chunkPos * world.chunkSize) + new Vector2Int(chunkTileX, chunkTileY));

				chunkSpawnableFeatures[chunkTileX, chunkTileY] = spawnableFeature;
			}
		}
	}

	// Populates the chunk with features
	void PopulateChunk()
	{
		float chunkWorldSpaceInterval = (float)(world.chunkSize * world.tileSize) / 100;
		float featureSpaceInterval = chunkWorldSpaceInterval / (float)world.chunkSize;

		for (int chunkTileY = 0; chunkTileY < world.chunkSize; chunkTileY++)
		{
			for (int chunkTileX = 0; chunkTileX < world.chunkSize; chunkTileX++)
			{
				SpawnableFeature chunkSpawnableFeature = chunkSpawnableFeatures[chunkTileX, chunkTileY];

				if (chunkSpawnableFeature != null)
				{
					Vector2Int worldPos = (chunkPos * 16) + new Vector2Int(chunkTileX, chunkTileY);

					float rndXOffset = Random.Range(-chunkSpawnableFeature.randomOffset, chunkSpawnableFeature.randomOffset);
					float rndYOffset = Random.Range(-chunkSpawnableFeature.randomOffset, chunkSpawnableFeature.randomOffset);

					GameObject tileObject = Instantiate(chunkSpawnableFeature.feature.gameObject, Vector2.zero, Quaternion.identity, transform); // Spawning the object in the chunk
					tileObject.transform.localPosition = new Vector2(chunkTileX + rndXOffset + 0.5f, chunkTileY + rndYOffset + 0.5f) * featureSpaceInterval; // The +0.5 is performed to centre the feature to the centre of the tile
				}
			}
		}
	}

	public bool hasChunkGenerated()
	{
		return generated;
	}

	// Unloads the chunk
	public void UnloadChunk()
	{ // NEED TO SAVE THE CHUNK INFORMATION IN STORAGE SO IT CAN BE RELOADED
		Destroy(gameObject);
	}
}
