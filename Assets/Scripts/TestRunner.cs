using UnityEngine;
using System.Collections;

public class TestRunner : MonoBehaviour
{
	public GameObject block;
	public TMPro.TextMeshProUGUI text;
	public Color warningColor = Color.red;
	public Color successColor = Color.green;
	public int maxTicks = 40;

	GameProgression gp;
	Circuit circuit;
	bool aborting;
	float timeout;
	int counter;
	bool successfull;

	public void Abort()
	{
		block.SetActive(false);
		text.text = "Aborting Test";
		text.color = warningColor;
		aborting = true;
		timeout = Time.time + 3.0f;
	}

	public void StartTest ()
	{
		if (gp == null)
			gp = FindObjectOfType<GameProgression>();
		if (circuit == null)
			circuit = FindObjectOfType<Circuit>();
		block.SetActive(true);
		aborting = false;
		gameObject.SetActive(true);
		RunTest(0);
		successfull = false;
	}

	void RunTest(int index)
	{
		if (index == gp.currentMap.states.Length)
		{
			aborting = true;
			timeout = Time.time + 2.0f;
			text.color = successColor;
			text.text = "Design Successfull";
			block.SetActive(false);
			gp.LoadLevel(gp.currentLevel + 1);
		}
		else
		{
			text.color = warningColor;
			text.SetText("Running Tests: " + (index + 1) + " / " + gp.currentMap.states.Length);
			gp.currentState = index;
			counter = 0;
		}
	}

	private void Update()
	{
		if (aborting)
		{
			if(timeout < Time.time)
			{
				if (successfull)
				{
					successfull = false;
					timeout = Time.time + 1.0f;
				}
				else
					gameObject.SetActive(false);
			}
		}
		else
		{
			circuit.Tick();
			counter++;
			if (gp.isStateCorrect)
			{
				gp.onStateValidated.Invoke(gp.currentState, true);
				RunTest(gp.currentState + 1);
			}
			else if (counter >= maxTicks)
			{
				gp.onStateValidated.Invoke(gp.currentState, false);
				block.SetActive(false);
				text.text = "Tests Failed";
				text.color = warningColor;
				aborting = true;
				timeout = Time.time + 4.0f;
			}
		}
	}
}
