using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameProgression : MonoBehaviour {

	[System.NonSerialized] public List<Map> maps;
	[SerializeField] MapList maplist;
	public UnityIntEvent onLevelChanged;
	public UnityIntEvent onStateChanged;
	public UnityIntBoolEvent onStateValidated;

	private Circuit circuit;
	private int _currentState;
	private int outputStates;

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
				onStateChanged.Invoke(value);
			}
		}
	}
	public bool isStateCorrect { get { return outputStates == (1 << currentMap.outputs) - 1; } }

	private void Awake()
	{
		maps = maplist.GetAllMaps();
		circuit = FindObjectOfType<Circuit>();
	}

	void Start () {
		LoadLevel(0);
	}

	public void LoadLevel(int index)
	{
		if (index >= maps.Count)
		{
			SceneManager.LoadScene(0);
			return;
		}
		circuit.Setup(maps[index]);
		currentLevel = index;
		_currentState = 0;
		onLevelChanged.Invoke(index);
		onStateChanged.Invoke(0);
		outputStates = 0;
	}

	public bool GetInputStatus(int input)
	{
		return (currentMap.states[currentState].x & (1<<input)) > 0;
	}

	public bool GetOutputStatus(int input)
	{
		return (currentMap.states[currentState].y & (1 << input)) > 0;
	}

	public void SetOutputState(int index, bool s)
	{
		if (s)
			outputStates = outputStates | (1 << index);
		else
			outputStates = outputStates - (outputStates & (1 << index));
	}
	
}

[System.Serializable]
public class UnityIntEvent : UnityEvent<int> { }
[System.Serializable]
public class UnityIntBoolEvent : UnityEvent<int, bool> { }
