using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
	public World world;

	public SpriteRenderer sr;

	List<MarkerRepresentitive> markerRepresentitives = new List<MarkerRepresentitive>();

	// Start is called before the first frame update
	void Start()
    {
		WorldGenerator.Create(world, this, 1);
	}

	// Update is called once per frame
	void Update()
    {
		foreach (MarkerRepresentitive markerRepresentitive in markerRepresentitives)
		{
			MapMarker mapMarker = markerRepresentitive.mapMarker;

			if (mapMarker != null)
			{
				if (mapMarker.displayAtChunk)
				{
					markerRepresentitive.representitive.transform.position = getWorldMapPositionOfChunkPosition(mapMarker.getChunkPosition());
				}
				else if (mapMarker.displayAtTile)
				{
					markerRepresentitive.representitive.transform.position = getWorldMapPositionOfTilePosition(mapMarker.getTilePosition());
				}
			}
		}
    }

	public void AddMarker(MapMarker marker)
	{
		MarkerRepresentitive mr = new MarkerRepresentitive(marker);
		markerRepresentitives.Add(mr);
	}

	Vector2 getWorldMapPositionOfChunkPosition(Vector2Int chunkPos)
	{
		return (new Vector2(chunkPos.x, chunkPos.y) / 100) + new Vector2(transform.position.x, transform.position.y);
	}

	Vector2 getWorldMapPositionOfTilePosition(Vector2Int tilePos)
	{
		return (new Vector2(tilePos.x, tilePos.y) / world.chunkSize / 100) + new Vector2(transform.position.x, transform.position.y);
	}

	public class MarkerRepresentitive // Represents a specific map marker and the UI object of the marker
	{
		public MapMarker mapMarker;
		public GameObject representitive;

		public MarkerRepresentitive(MapMarker mapMarker)
		{
			this.mapMarker = mapMarker;
			representitive = CreateRepresentitive();
		}

		GameObject CreateRepresentitive()
		{
			GameObject newRepresentitive = new GameObject();
			newRepresentitive.name = mapMarker.name + " Marker";

			SpriteRenderer sr = newRepresentitive.AddComponent<SpriteRenderer>();
			sr.sprite = mapMarker.mapSprite;
			sr.sortingOrder = 2;

			return newRepresentitive;
		}
	}

}