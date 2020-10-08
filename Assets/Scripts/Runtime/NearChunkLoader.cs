using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NearChunkLoader : MonoBehaviour // TODO: DO SOMETHING WITH THE THREAD WHEN THE GAME HAS STOPPED
{
	public World world;

	[Space]
	[Tooltip("The object that the chunks will generate around")]
	public Transform target;

	[Space]
	public int chunkDistance;

	List<ChunkGenerator> chunkGenerators = new List<ChunkGenerator>();
	bool generatingChunks = false;

    // Start is called before the first frame update
    void Start()
    {
		if (world == null)
		{
			world = FindObjectOfType<World>();
		}
		if (world != null)
		{
			StartCoroutine(GenerateNearChunks());
		}
		else
		{
			Debug.Log("Can't generate near chunks, as no object of type 'World' can be found!");
		}
	}

	void Update()
	{
		if (!generatingChunks)
		{
			StartCoroutine(GenerateNearChunks());
		}
	}

	// Generates the chunks near the target, within the chunk distance
	public IEnumerator GenerateNearChunks()
	{
		generatingChunks = true;

		float worldSpaceChunkInterval = (float)(world.chunkSize * world.tileSize) / 100;

		for (int y = -Mathf.FloorToInt(chunkDistance / 2) + 1; y < Mathf.FloorToInt(chunkDistance/2); y++)
		{
			for (int x = -Mathf.FloorToInt(chunkDistance / 2) + 1; x < Mathf.FloorToInt(chunkDistance / 2); x++)
			{
				Vector2Int chunkPos = new Vector2Int(Mathf.RoundToInt(target.position.x / worldSpaceChunkInterval), Mathf.RoundToInt(target.position.y / worldSpaceChunkInterval)) + new Vector2Int(x, y);

				bool alreadyGenerated = false;
				foreach (ChunkGenerator chunkGenerator in chunkGenerators)
				{
					if (chunkGenerator.chunkPos == chunkPos)
					{
						alreadyGenerated = true;
					}
				}

				if (!alreadyGenerated)
				{
					ChunkGenerator cg = ChunkGenerator.Create(world, chunkPos);
					if (cg != null)
					{
						chunkGenerators.Add(cg);

						while (!cg.hasChunkGenerated()) // Wait for the chunk to generate before generating another chunk, don't want to use all threads.
						{
							yield return new WaitForFixedUpdate();
						}
					}
				}
			}
		}

		UnloadFarChunks();

		generatingChunks = false;
	}

	// Unloads the chunks that excel the chunk distance from the player
	public void UnloadFarChunks()
	{
		for (int i = 0; i < chunkGenerators.Count; i++)
		{
			if (target.position.x - chunkGenerators[i].transform.position.x >= chunkDistance || target.position.x - chunkGenerators[i].transform.position.x <= -chunkDistance) //10 - 2.56 (7.44) >= 3 || 10 - 2.56(7.44) <= -3
			{                                                                                                                                                                  //0 - 2.56 (-2.56) >= 3 || 0 - 2.56(-2.56) <= -3
				chunkGenerators[i].UnloadChunk();

				chunkGenerators.RemoveAt(i);

				i--;
			}
			else if (target.position.y - chunkGenerators[i].transform.position.y >= chunkDistance || target.position.y - chunkGenerators[i].transform.position.y <= -chunkDistance)
			{
				chunkGenerators[i].UnloadChunk();

				chunkGenerators.RemoveAt(i);

				i--;
			}
		}
	}
}
