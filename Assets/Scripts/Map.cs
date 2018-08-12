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
	public int activeComponents = 7;
	public IntVector[] states;
	[SerializeField] private ComponentType[] _map;

	public ComponentType[] map
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
		_map = new ComponentType[height * width];
		for (int i = 0; i < height * width; i++)
		{
			_map[i] = ComponentType.Empty;
		}
		for (int i = 0; i < height * width; i += width)
		{
			_map[i] = ComponentType.Unbuildable;
			_map[i + width - 1] = ComponentType.Unbuildable;
		}
		for (int i = 0; i < width; i++)
		{
			_map[i] = ComponentType.Unbuildable;
			_map[width * height - 1 - i] = ComponentType.Unbuildable;
		}
		float inter = (float)(height) / (float)(inputs + 1);
		for (int i = 0; i < inputs; i++)
		{
			_map[Mathf.FloorToInt((i + 1) * inter) * width] = ComponentType.Input;
		}
		inter = (float)(height) / (float)(outputs + 1);
		for (int i = 0; i < outputs; i++)
		{
			_map[Mathf.FloorToInt((i + 1) * inter) * width + width - 1] = ComponentType.Output;
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
		map = new ComponentType[cc.Length];
		height = 0;
		width = 0;
		inputs = 0;
		outputs = 0;
		for (int i = 0; i < cc.Length; i++)
		{
			if (cc[i].localPosition.x >= width)
				width = cc[i].localPosition.x + 1;
			if (cc[i].localPosition.y >= height)
				height = cc[i].localPosition.y + 1;
			_map[i] = cc[i].component;
			if(cc[i].component == ComponentType.Input)
				inputs++;
			else if (cc[i].component == ComponentType.Output)
				outputs++;
		}
	}

	public Sprite DrawState(int index)
	{
		int m = Mathf.Max(inputs, outputs);
		Texture2D tex = new Texture2D(7, m * 2 + 1);
		var cols = tex.GetPixels();
		for (int i = 0; i < cols.Length; i++)
			cols[i] = Color.gray;
		for (int i = 3; i < cols.Length; i+=7)
			cols[i] = Color.clear;
		for (int i = 0; i < inputs; i++)
			cols[i * 2 * 7 + 7 + 1] = (states[index].x & (1 << i)) > 0 ? Color.yellow : Color.black;
		for (int i = 0; i < outputs; i++)
			cols[(i+1) * 2 * 7 - 2] = (states[index].y & (1 << i)) > 0 ? Color.yellow : Color.black;
		tex.SetPixels(cols);
		tex.filterMode = FilterMode.Point;
		tex.wrapMode = TextureWrapMode.Clamp;
		tex.Apply();
		return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1, 1, SpriteMeshType.FullRect);
	}
}
