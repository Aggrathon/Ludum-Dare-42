using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Circuit : MonoBehaviour {
	public float tickRate = 1.0f;
	[Header("Objects")]
	[SerializeField] SpriteRenderer outline;
	[SerializeField] TextMeshProUGUI title;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	[SerializeField] Camera camera;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
	[Header("Prefabs")]
	[SerializeField] GameObject wirePrefab;
	[SerializeField] GameObject notPrefab;
	[SerializeField] GameObject andPrefab;
	[SerializeField] GameObject orPrefab;
	[SerializeField] GameObject norPrefab;
	[SerializeField] GameObject nandPrefab;
	[SerializeField] GameObject xorPrefab;
	[SerializeField] GameObject bridgePrefab;
	[SerializeField] GameObject inputPrefab;
	[SerializeField] GameObject outputPrefab;
	
	private CircuitTile[] circuit;
	private Map map;
	private bool dirty = true;
	private float cd;
	SoundManager sound;

	public void Setup(Map map)
	{
		if (sound == null)
			sound = FindObjectOfType<SoundManager>();
		if (circuit != null && circuit.Length > 0)
			for (int i = 0; i < circuit.Length; i++)
				if (circuit[i].obj != null)
					circuit[i].obj.gameObject.SetActive(false);
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
				circuit[index] = new CircuitTile() { localPosition = new IntVector(j, i), position = LocalToWorld(new IntVector(j, i)) };
				circuit[index].component = layout[index];
				switch (layout[index])
				{
					case ComponentType.Input:
						circuit[index].index = input++;
						break;
					case ComponentType.Output:
						circuit[index].index = output++;
						break;
				}
			}
		}

		DrawOutline();
		title.text = map.title;
		title.transform.parent.position = new Vector3(0, map.height/2, 0);
		dirty = true;
		cd = 0;
	}

	void DrawOutline()
	{
		Texture2D tex = new Texture2D(map.width*4, map.height*4);
		var cols = tex.GetPixels();
		for (int i = 0; i < circuit.Length; i++)
		{
			if (circuit[i].component == ComponentType.Unbuildable)// || circuit[i].component == ComponentType.Input || circuit[i].component == ComponentType.Output)
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

	public bool GetTileAt(IntVector pos, out CircuitTile tile)
	{
		if (!CheckLocalBounds(pos))
		{
			tile = new CircuitTile() { component = ComponentType.Unbuildable };
			return false;
		}
		tile = circuit[LocalToIndex(pos)];
		return true;
	}

	public void DeleteArea(IntVector al, IntVector bl, bool force = false)
	{
		int minx = Mathf.Max(0, Mathf.Min(al.x, bl.x));
		int maxx = Mathf.Min(map.width, Mathf.Max(al.x, bl.x)+1);
		int miny = Mathf.Max(0, Mathf.Min(al.y, bl.y));
		int maxy = Mathf.Min(map.height, Mathf.Max(al.y, bl.y) + 1);
		for (int j = miny; j < maxy; j++)
		{
			for (int i = minx; i < maxx; i++)
			{
				int index = j * map.width + i;
				if (!force && (circuit[index].component == ComponentType.Unbuildable || circuit[index].component == ComponentType.Input || circuit[index].component == ComponentType.Output))
					continue;
				circuit[index].component = ComponentType.Empty;
			}
		}
		dirty = true;
		if (force)
			DrawOutline();
	}

	public void DrawWire(IntVector al, IntVector bl)
	{
		int minx = Mathf.Max(0, Mathf.Min(al.x, bl.x));
		int maxx = Mathf.Min(map.width, Mathf.Max(al.x, bl.x) + 1);
		int miny = Mathf.Max(0, Mathf.Min(al.y, bl.y));
		int maxy = Mathf.Min(map.height, Mathf.Max(al.y, bl.y) + 1);
		for (int j = miny; j < maxy; j++)
		{
			for (int i = minx; i < maxx; i++)
			{
				int index = j * map.width + i;
				if (circuit[index].component == ComponentType.Empty)
					circuit[index].component = ComponentType.Wire;
			}
		}
		dirty = true;
		sound.Play(SoundManager.Soundtypes.Ritch);
	}

	public void BuildComponent(ComponentType ct, IntVector a)
	{
		if (!CheckLocalBounds(a))
			return;
		int index = LocalToIndex(a);
		if (circuit[index].component == ComponentType.Empty || circuit[index].component == ComponentType.Wire)
		{
			circuit[index].component = ct;
			circuit[index].rotation = 0;
			dirty = true;
			if (ct == ComponentType.Unbuildable)
			{
				DrawOutline();
			}
		}
		sound.Play(SoundManager.Soundtypes.Paper);
	}

	public void BuildBridge(IntVector a, IntVector b)
	{
		if (!CheckLocalBounds(a) || !CheckLocalBounds(b))
			return;
		if (a == b)
			return;
		var source = circuit[LocalToIndex(a)];
		var target = circuit[LocalToIndex(b)];
		if ((target.component != ComponentType.Empty && target.component != ComponentType.Wire) || (source.component != ComponentType.Empty && source.component != ComponentType.Wire))
			return;
		source.index = Bridge.numBridges;
		target.index = Bridge.numBridges++;
		source.component = ComponentType.Bridge;
		target.component = ComponentType.Bridge;
		dirty = true;
		sound.Play(SoundManager.Soundtypes.Bump);
	}

	public void MoveComponent(IntVector a, IntVector b)
	{
		if (!CheckLocalBounds(a) || !CheckLocalBounds(b))
			return;
		if (a.x == b.x && a.y == b.y)
		{
			circuit[LocalToIndex(a)].rotation++;
			dirty = true;
		}
		else
		{
			var source = circuit[LocalToIndex(a)];
			var target = circuit[LocalToIndex(b)];
			switch (source.component)
			{
				case ComponentType.Wire:
				case ComponentType.Empty:
				case ComponentType.Unbuildable:
				case ComponentType.Input:
				case ComponentType.Output:
					return;
			}
			if (target.component == ComponentType.Empty || target.component == ComponentType.Wire)
			{
				target.component = source.component;
				source.component = ComponentType.Empty;
				target.index = source.index;
				target.rotation = source.rotation;
				dirty = true;
			}
		}
	}

	public IntVector WorldToLocal(Vector3 a, bool limit = false)
	{
		var pos = new IntVector(Mathf.RoundToInt(a.x + (float)(map.width-1) * 0.5f), Mathf.RoundToInt(a.y + (float)(map.height-1) * 0.5f));
		if (limit)
		{
			pos.x = Mathf.Clamp(pos.x, 0, map.width - 1);
			pos.y = Mathf.Clamp(pos.y, 0, map.height - 1);
		}
		return pos;
	}

	public Vector3 LocalToWorld(IntVector a)
	{
		return new Vector3((float)a.x - (float)(map.width-1) * 0.5f, (float)a.y - (float)(map.height-1) * 0.5f);
	}

	public bool CheckLocalBounds(IntVector a)
	{
		return a.x >= 0 && a.x < map.width && a.y >= 0 && a.y < map.height;
	}

	int LocalToIndex(IntVector a)
	{
		if (!CheckLocalBounds(a))
			Debug.LogError("IntVector out of bounds (LocalToIndex)");
		return a.y * map.width + a.x;
	}

	void RecreateCircuit()
	{
		Bridge.Redraw();
		for (int i = 0; i < circuit.Length; i++)
		{
			if (circuit[i].obj != null)
			{
				circuit[i].obj.gameObject.SetActive(false);
				circuit[i].obj = null;
			}
		}
		for (int i = 0; i < circuit.Length; i++)
		{
			if (circuit[i].obj == null)
			{
				switch (circuit[i].component)
				{
					case ComponentType.Wire:
						ObjectPool.Spawn(wirePrefab, circuit[i].position).GetComponent<Wire>().Setup(this, circuit[i]);
						break;
					case ComponentType.Not:
						ObjectPool.Spawn(notPrefab, circuit[i].position).GetComponent<Gate>().Setup(this, circuit[i]);
						break;
					case ComponentType.And:
						ObjectPool.Spawn(andPrefab, circuit[i].position).GetComponent<Gate>().Setup(this, circuit[i]);
						break;
					case ComponentType.Or:
						ObjectPool.Spawn(orPrefab, circuit[i].position).GetComponent<Gate>().Setup(this, circuit[i]);
						break;
					case ComponentType.Nor:
						ObjectPool.Spawn(norPrefab, circuit[i].position).GetComponent<Gate>().Setup(this, circuit[i]);
						break;
					case ComponentType.Nand:
						ObjectPool.Spawn(nandPrefab, circuit[i].position).GetComponent<Gate>().Setup(this, circuit[i]);
						break;
					case ComponentType.Xor:
						ObjectPool.Spawn(xorPrefab, circuit[i].position).GetComponent<Gate>().Setup(this, circuit[i]);
						break;
					case ComponentType.Bridge:
						ObjectPool.Spawn(bridgePrefab, circuit[i].position).GetComponent<Bridge>().Setup(this, circuit[i]);
						break;
					case ComponentType.Input:
						ObjectPool.Spawn(inputPrefab, circuit[i].position).GetComponent<ACircuitComponent>().Setup(this, circuit[i]);
						break;
					case ComponentType.Output:
						ObjectPool.Spawn(outputPrefab, circuit[i].position).GetComponent<ACircuitComponent>().Setup(this, circuit[i]);
						break;
				}
			}
		}
	}

	private void Update()
	{
		if (dirty)
		{
			RecreateCircuit();
			dirty = false;
		}
		if (cd - Time.time < 0)
		{
			Tick();
		}
	}

	public void Tick()
	{
		for (int i = 0; i < circuit.Length; i++)
		{
			if (circuit[i].obj != null)
				circuit[i].obj.PreTick();
		}
		for (int i = 0; i < circuit.Length; i++)
		{
			if (circuit[i].obj != null)
				circuit[i].obj.Tick();
		}
		for (int i = 0; i < circuit.Length; i++)
		{
			if (circuit[i].obj != null)
				circuit[i].obj.PostTick();
		}
		cd = Time.time + tickRate;
	}
}

[Serializable]
public class CircuitTile
{
	public IntVector localPosition;
	public int rotation;
	public Vector3 position;
	public int index;
	public ComponentType component;
	public ACircuitComponent obj;
}

[Serializable]
public struct IntVector
{
	public int x;
	public int y;

	public IntVector(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public override string ToString()
	{
		return x + " " + y;
	}

	public IntVector RotatedStep(int rotation)
	{
		switch ((rotation % 4 + 4) % 4)
		{
			case 1:
				return new IntVector(x, y + 1);
			case 2:
				return new IntVector(x - 1, y);
			case 3:
				return new IntVector(x, y - 1);
			default:
				return new IntVector(x + 1, y);
		}
	}

	public override bool Equals(object obj)
	{
		if (!(obj is IntVector))
		{
			return false;
		}
		var vector = (IntVector)obj;
		return x == vector.x &&
			   y == vector.y;
	}

	public override int GetHashCode()
	{
		return (x << 15) + ((y << 15) >> 15);
	}

	public static bool operator ==(IntVector a, IntVector b)
	{
		return a.x == b.x && a.y == b.y;
	}

	public static bool operator !=(IntVector a, IntVector b)
	{
		return a.x != b.x || a.y != b.y;
	}
}

public enum ComponentType
{
	Wire,
	Not,
	And,
	Or,
	Nor,
	Nand,
	Xor,
	Bridge,
	Empty,
	Unbuildable,
	Input,
	Output
}
