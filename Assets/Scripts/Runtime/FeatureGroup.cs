using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FeatureGroup // Feature Groups can be used to create things like forests
{
	public string name;

	[Space]
	public Color mapColor;

	[Space]
	[Range(0,1)]
	public float minAllowedNutrients;
	[Range(0, 1)]
	public float maxAllowedNutrients;

	[Space]
	public SpawnableFeature[] spawnableFeatures;
}
