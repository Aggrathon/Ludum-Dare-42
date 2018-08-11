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
	

}
