using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour
{
	public GameObject[] levels;

	private void Awake()
	{
		FindObjectOfType<GameProgression>().onLevelChanged.AddListener(ShowTutorial);
	}

	public void ShowTutorial(int index)
	{
		for (int i = 0; i < levels.Length; i++)
			if (levels[i] != null)
				levels[i].SetActive(false);
		if (index < levels.Length)
			if (levels[index] != null)
				levels[index].SetActive(true);
	}
}
