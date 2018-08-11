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
		if (gateType == ComponentType.Not)
		{
			if (circuit.GetTileAt(tile.localPosition.RotatedStep(tile.rotation + 2), out t) && t.obj != null)
				nextPower = !t.obj.isOn();
			else
				nextPower = true;
			return;
		}
		bool first = false;
		bool second = false;
		if (circuit.GetTileAt(tile.localPosition.RotatedStep(tile.rotation + 1), out t) && t.obj != null)
			first = t.obj.isOn();
		if (circuit.GetTileAt(tile.localPosition.RotatedStep(tile.rotation + 3), out t) && t.obj != null)
			second = t.obj.isOn();
		switch (gateType)
		{
			case ComponentType.And:
				nextPower = first & second;
				break;
			case ComponentType.Or:
				nextPower = first | second;
				break;
			case ComponentType.Nor:
				nextPower = !(first | second);
				break;
			case ComponentType.Nand:
				nextPower = !(first & second);
				break;
			case ComponentType.Xor:
				nextPower = first ^ second;
				break;
		}
	}

	public override void TrySetOn()
	{
	}
}
