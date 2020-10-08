using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
	int pixelsPerChunk;

	World world;

	[Header("Generation Variables")]
	public float timeTaken;
	public float generationPercentage;
	public int chunkX;
	public int chunkY;

	Texture2D texture;
	Color[] chunkColors;

	BiomeLayer[,] biomeLayers;
	Feature[,] features;

	bool appliedTexture;

	// Start is called before the first frame update
	void Start()
    {
		if (pixelsPerChunk < 1) // The value 0 or below will increase the overall size of the generated world, meaning more detail will show than it should, possibly resulting in issues
		{
			pixelsPerChunk = 1;
		}
		else if (pixelsPerChunk > world.chunkSize) // A value for blocksPerPixel over the value of chunkSize will generate not result at all
		{
			pixelsPerChunk = world.chunkSize;
		}

		StartCoroutine(Generate());
	}

	public static WorldGenerator Create(World world, WorldMap worldMap, int pixelsPerChunk)
	{
		worldMap.sr.sortingOrder = -1;

		WorldGenerator wg = worldMap.gameObject.AddComponent<WorldGenerator>();

		int worldWidth = world.width * pixelsPerChunk;
		int worldHeight = world.height * pixelsPerChunk;

		wg.world = world;
		wg.pixelsPerChunk = pixelsPerChunk;
		wg.biomeLayers = new BiomeLayer[worldWidth, worldHeight];
		wg.chunkColors = new Color[worldWidth * worldHeight];
		wg.features = new Feature[worldWidth, worldHeight];

		wg.texture = new Texture2D(worldWidth, worldHeight, TextureFormat.ARGB32, false);
		wg.texture.filterMode = FilterMode.Point;
		worldMap.sr.sprite = Sprite.Create(wg.texture, new Rect(Vector2.zero, new Vector2(worldWidth, worldHeight)), Vector2.zero);

		return wg;
	}

	IEnumerator Generate()
	{
		Thread thread = new Thread(GenerateWorldChunkColors);
		thread.Start();

		while (thread.IsAlive)
		{
			yield return new WaitForFixedUpdate();
		}

		DrawWorldChunkColors();
	}

	// Generates the chunk colors to be applied to the world texture
	void GenerateWorldChunkColors()
	{
		int worldWidth = world.width * pixelsPerChunk;
		int worldHeight = world.height * pixelsPerChunk;

		for (chunkY = 0; chunkY < worldHeight; chunkY++)
		{
			for (chunkX = 0; chunkX < worldWidth; chunkX++)
			{
				float[,] chunkHeightMap = world.heightGenerator.GenerateHeightMap(pixelsPerChunk, pixelsPerChunk, new Vector2Int(chunkX, chunkY));
				float[,] chunkClimateMap = world.climateGenerator.GenerateClimateNoiseMap(pixelsPerChunk, pixelsPerChunk, new Vector2Int(chunkX, chunkY));
				float[,] chunkBiomeMap = world.biomeGenerator.GenerateBiomeNoiseMap(pixelsPerChunk, pixelsPerChunk, new Vector2Int(chunkX, chunkY));
				float[,] chunkFeatureMap = world.featureGenerator.GenerateObjectNutrientsMap(pixelsPerChunk, pixelsPerChunk, new Vector2Int(chunkX, chunkY));

				for (int chunkTileY = 0; chunkTileY < pixelsPerChunk; chunkTileY++)
				{
					for (int chunkTileX = 0; chunkTileX < pixelsPerChunk; chunkTileX++)
					{
						int x = (chunkX * pixelsPerChunk) + chunkTileX;
						int y = (chunkY * pixelsPerChunk) + chunkTileY;

						Biome biome = world.biomeGenerator.getBiome(chunkBiomeMap[chunkTileX, chunkTileY], chunkHeightMap[chunkTileX, chunkTileY], chunkClimateMap[chunkTileX, chunkTileY]);
						BiomeLayer biomeLayer = world.biomeGenerator.getBiomeLayer(biome, chunkHeightMap[chunkTileX, chunkTileY], chunkClimateMap[chunkTileX, chunkTileY]);
						FeatureGroup featureGroup = world.featureGenerator.getFeatureGroup(chunkFeatureMap[chunkTileX, chunkTileY], biomeLayer, Vector2.zero);

						if (featureGroup != null)
						{
							chunkColors[(y * worldWidth) + x] = featureGroup.mapColor;
						}
						else
						{
							chunkColors[(y * worldWidth) + x] = biomeLayer.groundTile.mapColor;
						}
					}
				}
			}
		}
	}

	// Draws the tile textures onto the chunk texture
	void DrawWorldChunkColors()
	{
		texture.SetPixels(chunkColors);

		texture.Apply();
	}
}