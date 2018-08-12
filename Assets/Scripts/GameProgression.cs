using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameProgression : MonoBehaviour {

	public Map[] maps;
	public UnityEvent onLevelChanged;

	private Circuit circuit;
	int loadedLevel;

	public Map currentMap { get { return maps[loadedLevel]; } }
	public int currentIndex { get { return loadedLevel; } }

	void Start () {
		circuit = FindObjectOfType<Circuit>();
		circuit.Setup(maps[0]);
		loadedLevel = 0;
		onLevelChanged.Invoke();
	}
	

	public void LoadLevel(int index)
	{
		circuit.Setup(maps[index]);
		loadedLevel = index;
		onLevelChanged.Invoke();
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
