using UnityEngine;
using System.Collections;

public class Gate : ACircuitComponent
{
	bool nextPower;
	bool power;
	CircuitTile tile;
	Circuit circuit;

	public ComponentType gateType;

	public override bool isOn()
	{
		return power;
	}

	public override void PostTick()
	{
		power = nextPower;
	}

	public override void PreTick()
	{
		if (power)
		{
			CircuitTile t;
			var pos = tile.localPosition.RotatedStep(tile.rotation);
			if (circuit.GetTileAt(pos, out t) && t.obj != null)
				t.obj.TrySetOn();
		}
	}

	public override void Setup(Circuit circuit, CircuitTile tile)
	{
		tile.obj = this;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90 * tile.rotation));
		this.tile = tile;
		this.circuit = circuit;
		power = false;
	}

	public override void Tick()
	{
		CircuitTile t;
		switch (gateType)
		{
			case ComponentType.Not:
				if (circuit.GetTileAt(tile.localPosition.RotatedStep(tile.rotation + 2), out t) && t.obj != null)
				{
					nextPower = !t.obj.isOn();
				}
				else
				{
					nextPower = true;
				}
				break;
			case ComponentType.And:
				if (circuit.GetTileAt(tile.localPosition.RotatedStep(tile.rotation + 1), out t) && t.obj != null)
					nextPower = t.obj.isOn();
				if (nextPower && circuit.GetTileAt(tile.localPosition.RotatedStep(tile.rotation - 1), out t) && t.obj != null)
					nextPower = t.obj.isOn();
				break;
			case ComponentType.Or:
				if (circuit.GetTileAt(tile.localPosition.RotatedStep(tile.rotation + 1), out t) && t.obj != null)
					nextPower = t.obj.isOn();
				if (!nextPower && circuit.GetTileAt(tile.localPosition.RotatedStep(tile.rotation - 1), out t) && t.obj != null)
					nextPower = t.obj.isOn();
				break;
			case ComponentType.Nor:
				if (circuit.GetTileAt(tile.localPosition.RotatedStep(tile.rotation + 1), out t) && t.obj != null)
					nextPower = !t.obj.isOn();
				if (nextPower && circuit.GetTileAt(tile.localPosition.RotatedStep(tile.rotation - 1), out t) && t.obj != null)
					nextPower = !t.obj.isOn();
				break;
			case ComponentType.Nand:
				if (circuit.GetTileAt(tile.localPosition.RotatedStep(tile.rotation + 1), out t) && t.obj != null)
					nextPower = !t.obj.isOn();
				if (!nextPower && circuit.GetTileAt(tile.localPosition.RotatedStep(tile.rotation - 1), out t) && t.obj != null)
					nextPower = !t.obj.isOn();
				break;
			case ComponentType.Xor:
				if (circuit.GetTileAt(tile.localPosition.RotatedStep(tile.rotation + 1), out t) && t.obj != null)
					nextPower = t.obj.isOn();
				if (nextPower && circuit.GetTileAt(tile.localPosition.RotatedStep(tile.rotation - 1), out t) && t.obj != null)
					nextPower = t.obj.isOn() != nextPower;
				break;
		}
	}

	public override void TrySetOn()
	{
	}
}
