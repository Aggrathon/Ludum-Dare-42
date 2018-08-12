using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

	public void StartGame()
	{
		SceneManager.LoadScene(1);
	}
}
