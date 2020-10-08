using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnableFeature
{
	public string name;
	[Space]

	public Feature feature;
	[Space]

	[Range(0, 1)]
	public double spawnChance = 1;
	[Tooltip("The spacing between each of these features. X is min val, Y is max val.")]
	[Min(1)]
	public Vector2Int spacing; // I don't know if this random range is really necessary, considering spawnChance can generate similar results. This should be reconsidered for it's advantages.
	[Tooltip("A random offset that is a applied to the feature when it's spawned, this is too provide some variety to the generation.")]
	[Range(0, 0.5f)]
	public float randomOffset;
}
