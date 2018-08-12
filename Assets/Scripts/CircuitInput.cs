using UnityEngine;
using System.Collections;

public class CircuitInput : ACircuitComponent
{
	public SpriteRenderer status;
	public Color onColor = Color.yellow;
	public Color offColor = Color.black;

	GameProgression gp;
	bool power;
	Circuit circuit;
	CircuitTile tile;

	public override bool isOn(IntVector origin)
	{
		return power;
	}

	public override void PostTick()
	{
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
		this.circuit = circuit;
		this.tile = tile;
		tile.obj = this;
		if (gp == null)
			gp = FindObjectOfType<GameProgression>();
		power = gp.GetInputStatus(tile.index);
		status.color = power ? onColor : offColor;
	}

	public override void Tick()
	{
	}

	public override void TrySetOn()
	{
	}
}
