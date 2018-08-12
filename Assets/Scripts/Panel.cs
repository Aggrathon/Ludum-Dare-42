using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour {

	public GameObject[] collapsedObjects;
	public GameObject[] uncollapsedObjects;

	public float collapsedSize;
	public float uncollapsedSize;

	[Header("Left Panel")]
	public Transform buildButtons;
	public TMP_Dropdown levelSelect;
	[Header("Right Panel")]
	public Transform stateButtons;

	MouseManager mm;
	GameProgression gp;

	public void Test()
	{

	}

	public void Collapse()
	{
		for (int i = 0; i < collapsedObjects.Length; i++)
		{
			collapsedObjects[i].SetActive(true);
		}
		for (int i = 0; i < uncollapsedObjects.Length; i++)
		{
			uncollapsedObjects[i].SetActive(false);
		}
		(transform.GetChild(0) as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, collapsedSize);
	}

	public void Uncollapse()
	{
		for (int i = 0; i < collapsedObjects.Length; i++)
		{
			collapsedObjects[i].SetActive(false);
		}
		for (int i = 0; i < uncollapsedObjects.Length; i++)
		{
			uncollapsedObjects[i].SetActive(true);
		}
		(transform.GetChild(0) as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, uncollapsedSize);
	}

	public void SetBuildType(int cpt)
	{
		mm.SetComponentToBuild(mm.previews[cpt].component);
		for (int i = 0; i < buildButtons.childCount; i++)
		{
			buildButtons.GetChild(i).GetComponent<Button>().interactable = i != cpt;
		}
	}

	public void SetState(int i)
	{
		for (int j = 0; j < stateButtons.childCount; j++)
			stateButtons.GetChild(j).GetComponent<Button>().interactable = true;
		stateButtons.GetChild(i).GetComponent<Button>().interactable = false;
		gp.currentState = i;
	}

	private void Awake()
	{
		gp = FindObjectOfType<GameProgression>();
		Uncollapse();
		if (levelSelect)
		{
			var list = new List<TMP_Dropdown.OptionData>();
			var levels = gp.maps;
			for (int i = 0; i < levels.Length; i++)
			{
				list.Add(new TMP_Dropdown.OptionData(levels[i].title));
			}
			levelSelect.options = list;
			levelSelect.onValueChanged.AddListener(gp.LoadLevel);
			gp.onLevelChanged.AddListener(() => { levelSelect.value = gp.currentLevel; });
		}
		if (buildButtons)
		{
			mm = FindObjectOfType<MouseManager>();
			for (int i = buildButtons.childCount; i < mm.previews.Length; i++)
			{
				Instantiate(buildButtons.GetChild(0), buildButtons);
			}
			for (int i = 0; i < mm.previews.Length; i++)
			{
				int j = i;
				buildButtons.GetChild(i).GetComponent<Button>().onClick.AddListener(() => SetBuildType(j));
				buildButtons.GetChild(i).GetChild(1).GetComponent<Image>().sprite = mm.previews[i].icon;
				buildButtons.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = mm.previews[i].name;
			}
			gp.onLevelChanged.AddListener(() => {
				for (int k = 0; k < buildButtons.childCount; k++)
					buildButtons.GetChild(k).gameObject.SetActive((gp.currentMap.activeComponents & (1 << k)) > 0);
			});
		}
		if (stateButtons)
		{
			gp.onLevelChanged.AddListener(() => {
				var map = gp.currentMap;
				for (int i = stateButtons.childCount; i < map.states.Length; i++)
				{
					var b = Instantiate(stateButtons.GetChild(0), stateButtons).GetComponent<Button>();
					int j = i;
					b.onClick.RemoveAllListeners();
					b.onClick.AddListener(() => SetState(j));
				}
				for (int i = map.states.Length; i < stateButtons.childCount; i++)
					stateButtons.GetChild(i).gameObject.SetActive(false);
				for (int i = 0; i < map.states.Length; i++)
				{
					var tr = stateButtons.GetChild(i);
					tr.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Status\n<color=blue>Unknown</color>";
					tr.GetChild(1).GetComponent<Image>().sprite = map.DrawState(i);
				}
			});
			gp.onStateChanged.AddListener(() => SetState(gp.currentState));
		}
	}
}
