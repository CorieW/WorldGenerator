using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise
{
	public enum NormalizeMode { Local, Global }

	public static float[,] GeneratePerlinNoise(int width, int height, int seed, float scale, int octaves, Vector2 offset, float persistance = 0.5f, float lacunarity = 2, float step = 0, NormalizeMode normalizeMode = NormalizeMode.Local)
	{
		float[,] noiseMap = new float[width, height];

		System.Random prng = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];

		float maxPossibleHeight = 0;
		float amplitude = 1;
		float frequency = 1;

		for (int i = 0; i < octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + offset.x;
			float offsetY = prng.Next(-100000, 100000) + offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);

			maxPossibleHeight += amplitude;
			amplitude *= persistance;
		}

		if (scale <= 0)
		{
			scale = 0.0001f;
		}

		float maxLocalNoiseHeight = float.MinValue;
		float minLocalNoiseHeight = float.MaxValue;

		float halfWidth = width / 2f;
		float halfHeight = height / 2f;

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				amplitude = 1;
				frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++)
				{
					float sampleX = (float)(x - halfWidth + octaveOffsets[i].x) / scale * frequency;
					float sampleY = (float)(y - halfHeight + octaveOffsets[i].y) / scale * frequency;

					float pNoise = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					noiseHeight += pNoise * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noiseHeight > maxLocalNoiseHeight)
				{
					maxLocalNoiseHeight = noiseHeight;
				}
				else if (noiseHeight < minLocalNoiseHeight)
				{
					minLocalNoiseHeight = noiseHeight;
				}
				noiseMap[x, y] = noiseHeight;
			}
		}

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				if (step != 0)
				{
					noiseMap[x, y] = Mathf.Round(noiseMap[x, y] / step);
					noiseMap[x, y] = step * noiseMap[x, y];
					//Debug.Log(noiseMap[x, y]);
				}

				if (normalizeMode == NormalizeMode.Local)
				{
					noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
				}
				else
				{
					float normalizedHeight = (noiseMap[x, y] + 1) / (2f * maxPossibleHeight / 2f);
					noiseMap[x, y] = normalizedHeight;
				}

				noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y]);
			}
		}

		return noiseMap;
	}
}
