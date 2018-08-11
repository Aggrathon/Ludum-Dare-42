using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Map", menuName = "Scriptable Object/Map")]
public class Map : ScriptableObject
{
	public string title;
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
			return _map;
		}

		set
		{
			_map = value;
		}
	}

	public void Reset()
	{
		_map = new int[height * width];
		for (int i = 0; i < height * width; i += width)
		{
			_map[i] = 1;
			_map[i + width - 1] = 1;
		}
		for (int i = 0; i < width; i++)
		{
			_map[i] = 1;
			_map[width * height - 1 - i] = 1;
		}
		float inter = (float)(height) / (float)(inputs + 1);
		for (int i = 0; i < inputs; i++)
		{
			_map[Mathf.FloorToInt((i + 1) * inter) * width] = 2;
		}
		inter = (float)(height) / (float)(outputs + 1);
		for (int i = 0; i < outputs; i++)
		{
			_map[Mathf.FloorToInt((i + 1) * inter) * width + width - 1] = 3;
		}
	}

	public void CopyCurrent()
	{
		Circuit current = FindObjectOfType<Circuit>();
		if (current == null)
		{
			Debug.LogError("Could not find an active circuit to copy");
			return;
		}
		var cc = current.GetCircuit();
		map = new int[cc.Length];
		height = 0;
		width = 0;
		inputs = 0;
		outputs = 0;
		for (int i = 0; i < cc.Length; i++)
		{
			if (cc[i].x >= width)
				width = cc[i].x + 1;
			if (cc[i].y >= height)
				height = cc[i].y + 1;
			if (cc[i].unbuildable)
			{
				if (cc[i].inputIndex >= 0)
				{
					inputs++;
					_map[i] = 2;
				}
				else if (cc[i].outputIndex >= 0)
				{
					outputs++;
					_map[i] = 3;
				}
				else
				{
					_map[i] = 1;
				}
			}
		}
	}
}
