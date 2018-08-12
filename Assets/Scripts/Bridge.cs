using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class Bridge : ACircuitComponent
{
	bool power;
	Circuit circuit;
	CircuitTile tile;
	Bridge other;
	LineRenderer lr;

	public static int numBridges = 0;
	static Dictionary<int, Bridge> matcher;

	public static void Redraw()
	{
		if (matcher == null)
			matcher = new Dictionary<int, Bridge>();
		else
			matcher.Clear();
	}

	public override bool isOn()
	{
		return power;
	}

	public override void PostTick()
	{
		power = power | other.power;
	}

	public override void PreTick()
	{
		if(power)
			Spread();
	}

	public override void Setup(Circuit circuit, CircuitTile tile)
	{
		power = false;
		this.circuit = circuit;
		this.tile = tile;
		tile.obj = this;
		if (matcher.ContainsKey(tile.index))
		{
			var b = matcher[tile.index];
			b.Connect(this);
			Connect(b);
		}
		else
		{
			matcher.Add(tile.index, this);
		}
	}

	public void Connect(Bridge other)
	{
		this.other = other;
		if (lr == null)
			lr = GetComponent<LineRenderer>();
		Vector3 dir = Vector2.Perpendicular(transform.position-other.transform.position);
		dir = dir.normalized * 0.42f;
		lr.SetPosition(0, transform.position + dir);
		lr.SetPosition(1, other.transform.position + dir);
	}

	public override void Tick()
	{
		if(power)
		{
			power = false;
		}
		CircuitTile t;
		if (circuit.GetTileAt(tile.localPosition.RotatedStep(0), out t) && t.obj != null && t.obj.isOn())
			power = true;
		else if (circuit.GetTileAt(tile.localPosition.RotatedStep(1), out t) && t.obj != null && t.obj.isOn())
			power = true;
		else if (circuit.GetTileAt(tile.localPosition.RotatedStep(2), out t) && t.obj != null && t.obj.isOn())
			power = true;
		else if (circuit.GetTileAt(tile.localPosition.RotatedStep(3), out t) && t.obj != null && t.obj.isOn())
			power = true;
	}

	public override void TrySetOn()
	{
		if (!power)
		{
			power = true;
			Spread();
		}
	}

	void Spread()
	{
		CircuitTile t;
		if (circuit.GetTileAt(tile.localPosition.RotatedStep(0), out t) && t.obj != null)
			t.obj.TrySetOn();
		else if (circuit.GetTileAt(tile.localPosition.RotatedStep(1), out t) && t.obj != null)
			t.obj.TrySetOn();
		else if (circuit.GetTileAt(tile.localPosition.RotatedStep(2), out t) && t.obj != null)
			t.obj.TrySetOn();
		else if (circuit.GetTileAt(tile.localPosition.RotatedStep(3), out t) && t.obj != null)
			t.obj.TrySetOn();
	}

	private void OnDisable()
	{
		if (tile.component != ComponentType.Bridge)
		{
			other.tile.component = ComponentType.Empty;
			other.tile.obj = null;
			other.gameObject.SetActive(false);
			matcher.Remove(other.tile.index);
		}
	}
}
