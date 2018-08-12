using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgression : MonoBehaviour {

	public Map[] maps;

	private Circuit circuit;
	
	void Start () {
		circuit = FindObjectOfType<Circuit>();
		circuit.Setup(maps[0]);
	}
	

	public void LoadLevel(int index)
	{
		circuit.Setup(maps[index]);
	}

	public bool GetInputStatus(int input)
	{
		return true;
	}

	public bool GetOutputStatus(int input)
	{
		return Random.value > 0.5f;
	}
}
