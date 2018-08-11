using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Circuit : MonoBehaviour {
	[Header("Objects")]
	[SerializeField] SpriteRenderer outline;
	[SerializeField] TextMeshProUGUI title;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	[SerializeField] Camera camera;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

	private CircuitTile[] circuit;
	private Map map;

	public void Setup(Map map)
	{
		//camera.orthographicSize = Mathf.Max(map.width, map.height+1)*0.5f;
		this.map = map;

		var layout = map.map;
		circuit = new CircuitTile[map.height * map.width];
		int w = map.width;
		int input = 0;
		int output = 0;
		for (int i = 0; i < map.height; i++)
		{
			for (int j = 0; j < map.width; j++)
			{
				int index = i * w + j;
				circuit[index].x = j;
				circuit[index].y = i;
				switch (layout[index])
				{
					case 1:
						circuit[index].unbuildable = true;
						circuit[index].inputIndex = -1;
						circuit[index].outputIndex = -1;
						break;
					case 2:
						circuit[index].unbuildable = true;
						circuit[index].inputIndex = input++;
						circuit[index].outputIndex = -1;
						break;
					case 3:
						circuit[index].unbuildable = true;
						circuit[index].inputIndex = -1;
						circuit[index].outputIndex = output++;
						break;
					default:
						break;
				}
			}
		}

		DrawOutline();
		title.text = map.title;
		title.transform.parent.position = new Vector3(0, map.height/2, 0);
	}

	void DrawOutline()
	{
		Texture2D tex = new Texture2D(map.width*4, map.height*4);
		var cols = tex.GetPixels();
		for (int i = 0; i < circuit.Length; i++)
		{
			if (circuit[i].unbuildable)
			{
				cols[i * 4 + 0] = new Color(0, 0, 0, 0);
				cols[i * 4 + 1] = new Color(0, 0, 0, 0);
				cols[i * 4 + 2] = new Color(0, 0, 0, 0);
				cols[i * 4 + 3] = new Color(0, 0, 0, 0);
			}
			else
			{
				cols[i * 4 + 0] = new Color(1, 1, 1, 1);
				cols[i * 4 + 1] = new Color(1, 1, 1, 1);
				cols[i * 4 + 2] = new Color(1, 1, 1, 1);
				cols[i * 4 + 3] = new Color(1, 1, 1, 1);
			}
		}
		for (int i = map.height-1; i >= 0; i--)
		{
			if (i != 0)
				Array.Copy(cols, i * tex.width, cols, i * 4 * tex.width, tex.width);
			Array.Copy(cols, i * tex.width, cols, (i * 4 + 1) * tex.width, tex.width);
			Array.Copy(cols, i * tex.width, cols, (i * 4 + 2) * tex.width, tex.width);
			Array.Copy(cols, i * tex.width, cols, (i * 4 + 3) * tex.width, tex.width);
		}
		tex.SetPixels(cols);
		tex.wrapMode = TextureWrapMode.Clamp;
		tex.filterMode = FilterMode.Point;
		tex.Apply();
		outline.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 4, 8, SpriteMeshType.FullRect, new Vector4(0,0,0,0), false);
	}

	public CircuitTile[] GetCircuit()
	{
		return circuit;
	}

	public bool GetTileAt(Vector3 pos, out CircuitTile tile)
	{
		int x = Mathf.RoundToInt(pos.x + (float)map.width * 0.5f);
		int y = Mathf.RoundToInt(pos.y + (float)map.height * 0.5f);
		if (x < 0 || x >= map.width || y < 0 || y >= map.height)
		{
			tile = new CircuitTile() { unbuildable = true };
			return false;
		}
		tile = circuit[y * map.width + x];
		return true;
	}

	public void DeleteArea(Vector3 a, Vector3 b)
	{
		a.x += (float)map.width * 0.5f;
		b.x += (float)map.width * 0.5f;
		a.y += (float)map.height * 0.5f;
		b.y += (float)map.height * 0.5f;
		int minx = Mathf.Min(map.width-1, Mathf.CeilToInt(Mathf.Min(a.x, b.x)));
		int maxx = Mathf.Max(0, Mathf.CeilToInt(Mathf.Max(a.x, b.x)));
		int miny = Mathf.Min(map.height - 1, Mathf.FloorToInt(Mathf.Min(a.y, b.y)));
		int maxy = Mathf.Max(0, Mathf.FloorToInt(Mathf.Max(a.y, b.y)));
		for (int j = miny; j <= maxy; j++)
		{
			for (int i = minx; i <= maxx; i++)
			{
				circuit[j * map.width + i].component.SetActive(false);
				circuit[j * map.width + i].component = null;
			}
		}
	}
}

[Serializable]
public struct CircuitTile
{
	public int x;
	public int y;
	public bool unbuildable;
	public int inputIndex;
	public int outputIndex;
	public GameObject component;
}
