using UnityEngine;
using System.Collections;

public abstract class ACircuitComponent : MonoBehaviour
{
	public abstract void PreTick();
	public abstract void Tick();
	public abstract void PostTick();
	public abstract bool isOn();
	public abstract void Setup(Circuit circuit, IntVector pos);
	public abstract void TrySetOn();
}
