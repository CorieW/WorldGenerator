using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
	public World world;
	[Space]

	[Header("Spawning Settings")]
	public bool spawnAtRandomLocation;
	public Biome[] spawnBiomes;
	public Tile[] spawnTiles;

	// Start is called before the first frame update
	void Start()
    {
		if(world == null)
		{
			world = FindObjectOfType<World>();
		}

		if (spawnAtRandomLocation)
		{
			SpawnAtRandomLocation();
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SpawnAtRandomLocation()
	{
		Vector2 tilePos = SpawnLocation.getRandomSpawnTile(world, spawnBiomes, spawnTiles);

		float chunkWorldSpaceInterval = (float)(world.chunkSize * world.tileSize) / 100;
		float featureSpaceInterval = chunkWorldSpaceInterval / (float)world.chunkSize;

		transform.position = new Vector2(tilePos.x * featureSpaceInterval, tilePos.y * featureSpaceInterval);
	}
}
