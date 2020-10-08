using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome : MonoBehaviour
{
	[Space]
	[Range(0, 1)]
	public float maxHeight;
	[Range(0, 1)]
	public float minHeight;

	[Space]
	[Range(0, 1)]
	public float maxTemp;
	[Range(0, 1)]
	public float minTemp; // The temperatures that this biome can spawn in

	[Space]
	[Tooltip("The biome used when no biome meets the spawning criteria")]
	public BiomeLayer defaultBiomeLayer;
	public BiomeLayer[] biomeLayers;
}
[System.Serializable]
public class BiomeLayer // Useful for acquiring things like beaches and mountains in biomes
{
	public string name;

	[Space]
	[Range(0, 1)]
	public float maxHeight;
	[Range(0, 1)]
	public float minHeight;

	[Space]
	[Range(0, 1)]
	public float maxTemp;
	[Range(0, 1)]
	public float minTemp; // The temperatures that this biome can spawn in

	[Space]
	public Tile groundTile;
	public FeatureGroup[] featureGroups;
}