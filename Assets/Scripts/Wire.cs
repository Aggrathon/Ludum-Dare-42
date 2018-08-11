using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class Wire : ACircuitComponent
{
	public Color onColor = Color.yellow;
	public Color offColor = Color.white;
	LineRenderer lr;
	List<Vector3> points;
	bool power;
	bool oldpower;

	private void Awake()
	{
		lr = GetComponent<LineRenderer>();
	}

	public override bool isOn()
	{
		return power;
	}

	public override void PostTick()
	{
		print("No Prev " + power + " " + oldpower);
		if (oldpower)
		{
			lr.startColor = onColor;
			lr.endColor = onColor;
		}
		else
		{
			lr.startColor = offColor;
			lr.endColor = offColor;
		}
		power = false;
	}

	public override void PreTick()
	{
	}

	public override void Tick()
	{
		oldpower = power;
	}

	public override void TrySetOn()
	{
		power = true;
	}

	public override void Setup(Circuit circuit, CircuitTile tile)
	{
		power = false;
		oldpower = false;
		if (points == null)
			points = new List<Vector3>();
		else
			points.Clear();
		if (tile.obj != null || tile.component != ComponentType.Wire)
		{
			gameObject.SetActive(false);
			return;
		}
		Recurse(circuit, tile.localPosition, tile.position);
		if (points.Count == 2)
		{
			points.Add(points[0]);
			points.Add(points[0]);
			points.Add(points[0]);
			points.Add(points[0]);
			points[0] += new Vector3(0.2f, 0);
			points[1] += new Vector3(0, 0.2f);
			points[2] += new Vector3(-0.2f, 0);
			points[3] += new Vector3(0, -0.2f);
			points[4] += new Vector3(0.2f, 0);
			points[5] += new Vector3(0, 0.2f);
		}
		lr.positionCount = points.Count;
		lr.SetPositions(points.ToArray());
	}

	void Recurse(Circuit circuit, IntVector pos, Vector3 prev)
	{
		CircuitTile tile;
		if (circuit.GetTileAt(pos, out tile))
		{
			switch (tile.component)
			{
				case ComponentType.Wire:
					if (tile.obj == null)
					{
						tile.obj = this;
						points.Add(tile.position);
						Recurse(circuit, new IntVector(tile.localPosition.x, tile.localPosition.y + 1), tile.position);
						Recurse(circuit, new IntVector(tile.localPosition.x, tile.localPosition.y - 1), tile.position);
						Recurse(circuit, new IntVector(tile.localPosition.x + 1, tile.localPosition.y), tile.position);
						Recurse(circuit, new IntVector(tile.localPosition.x - 1, tile.localPosition.y), tile.position);
						points.Add(prev);
					}
					break;
				case ComponentType.Unbuildable:
				case ComponentType.Empty:
					break;
				default:
					points.Add(prev * 0.5f + tile.position * 0.5f);
					points.Add(prev);
					break;
			}
		}
	}
}
