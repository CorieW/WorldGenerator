using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarker : MonoBehaviour
{
	public World world;

	[Space]
	[Header("Marker Information")]
	public string name;
	public Sprite mapSprite;

	[Space]
	[Header("Marker Display Settings")]
	public bool displayAtChunk;
	public bool displayAtTile;

	void Start()
	{
		world = FindObjectOfType<World>();
		world.worldMap.AddMarker(this);
	}

	// Returns the chunk position
	public Vector2Int getChunkPosition()
	{
		Vector2 chunkPos = getTilePosition() / world.chunkSize;

		return new Vector2Int(Mathf.FloorToInt(chunkPos.x), Mathf.FloorToInt(chunkPos.y));
	}

	// Returns the exact tile position of the object
	public Vector2Int getTilePosition()
	{
		float chunkWorldSpaceInterval = (float)(world.chunkSize * world.tileSize) / 100;
		float sceneTileSize = chunkWorldSpaceInterval / (float)world.chunkSize;

		return new Vector2Int(Mathf.FloorToInt(transform.position.x / sceneTileSize), Mathf.FloorToInt(transform.position.y / sceneTileSize));
	}

	void OnDestroy()
	{ 
		// TODO: Save to storage then load somehow
	}
}
