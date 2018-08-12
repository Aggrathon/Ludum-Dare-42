using UnityEngine;
using System.Collections;

public class CircuitOutput : ACircuitComponent
{
	public int stabilizingTicks = 10;
	public SpriteRenderer status;
	public Color onColor = Color.yellow;
	public Color shouldOnColor = Color.blue;
	public Color offColor = Color.black;
	public Color shouldOffColor = Color.red;

	GameProgression gp;
	bool shouldPower;
	bool prevPower;
	int counter;
	Circuit circuit;
	CircuitTile tile;

	public override bool isOn(IntVector origin)
	{
		return false;
	}

	public override void PostTick()
	{
	}

	public override void PreTick()
	{
	}

	public override void Setup(Circuit circuit, CircuitTile tile)
	{
		this.circuit = circuit;
		this.tile = tile;
		tile.obj = this;
		if (gp == null)
			gp = FindObjectOfType<GameProgression>();
		shouldPower = gp.GetOutputStatus(tile.index);
		status.color = shouldPower ? shouldOnColor : offColor;
	}

	public override void Tick()
	{
		CircuitTile t;
		bool power = false;
		var pos = tile.localPosition.RotatedStep(tile.rotation+2);
		if (circuit.GetTileAt(pos, out t) && t.obj != null)
			power = t.obj.isOn(tile.localPosition);
		if (power == prevPower && shouldPower == power)
		{
			counter++;
			if (counter >= stabilizingTicks)
			{
				//TODO: Mark Output Correct
			}
		}
		else
		{
			counter = 0;
			//TODO: Mark Output Incorrect
		}
		prevPower = power;
		if (shouldPower)
			if (power)
				status.color = onColor;
			else
				status.color = shouldOnColor;
		else
			if (power)
				status.color = shouldOffColor;
			else
				status.color = offColor;
	}

	public override void TrySetOn()
	{
	}
}
