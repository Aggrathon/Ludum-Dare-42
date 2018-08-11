using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Map", menuName = "Scriptable Object/Map")]
public class Map : ScriptableObject
{
	public int width = 10;
	public int height = 10;
	public int inputs = 2;
	public int outputs = 2;
	[SerializeField] private int[] _map;

	public int[] map
	{
		get
		{
			if (_map == null || _map.Length == 0 || _map.Length != width * height)
				Reset();
			return map;
		}

		set
		{
			map = value;
		}
	}

	public void Reset()
	{
		map = new int[height * width];
		for (int i = 0; i < height * width; i += width)
		{
			map[i] = 1;
			map[i + width - 1] = 1;
		}
		for (int i = 0; i < width; i++)
		{
			map[i] = 1;
			map[width * height - 1 - i] = 1;
		}
		float inter = (float)(height) / (float)(inputs + 1);
		for (int i = 0; i < inputs; i++)
		{
			map[Mathf.FloorToInt((i + 1) * inter) * width] = 2;
		}
		inter = (float)(height) / (float)(outputs + 1);
		for (int i = 0; i < outputs; i++)
		{
			map[Mathf.FloorToInt((i + 1) * inter) * width + width - 1] = 3;
		}
	}

	public void CopyCurrent()
	{
		var current = FindObjectOfType<Circuit>();
		if (current == null)
		{
			Debug.LogError("Could not find an active circuit to copy");
			return;
		}
		//TODO
	}
}
