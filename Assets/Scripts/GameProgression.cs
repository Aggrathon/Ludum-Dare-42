using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameProgression : MonoBehaviour {

	public Map[] maps;
	public UnityEvent onLevelChanged;
	public UnityEvent onStateChanged;

	private Circuit circuit;
	private int _currentState;

	public Map currentMap { get { return maps[currentLevel]; } }
	public int currentLevel { get; private set; }
	public int currentState {
		get { return _currentState; }
		set {
			if (value < 0)
				value = 0;
			if (value >= currentMap.states.Length)
				value = currentMap.states.Length;
			if (value != _currentState)
			{
				_currentState = value;
				onStateChanged.Invoke();
			}
		}
	}

	void Start () {
		circuit = FindObjectOfType<Circuit>();
		LoadLevel(0);
	}

	public void LoadLevel(int index)
	{
		circuit.Setup(maps[index]);
		currentLevel = index;
		_currentState = 0;
		onLevelChanged.Invoke();
		onStateChanged.Invoke();
	}

	public bool GetInputStatus(int input)
	{
		return (currentMap.states[currentState].x & (1<<input)) > 0;
	}

	public bool GetOutputStatus(int input)
	{
		return (currentMap.states[currentState].y & (1 << input)) > 0;
	}
}
