using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
	World world;

	public Tile[,] tiles;
	public List<Feature> features = new List<Feature>();
	public Vector2Int chunkPos;

	public Chunk(World world, Vector2Int chunkPos)
	{
		this.world = world;
		this.chunkPos = chunkPos;

		tiles = new Tile[world.chunkSize, world.chunkSize];
	}
}
public class SimpleChunk // Holds some very simple data about the chunk, instead of all of the data about the chunk (Simplified chunk)
{
	public Biome biome;
	public BiomeLayer biomeLayer;

	public SimpleChunk(Biome biome, BiomeLayer biomeLayer)
	{
		this.biome = biome;
		this.biomeLayer = biomeLayer;
	}
}