using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeftPanel : MonoBehaviour {

	public GameObject[] collapsedObjects;
	public GameObject[] uncollapsedObjects;
	public Transform buildButtons;

	public float collapsedSize;
	public float uncollapsedSize;

	public TMP_Dropdown levelSelect;

	MouseManager mm;

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

	private void Start()
	{
		Uncollapse();
		var list = new List<TMP_Dropdown.OptionData>();
		var gp = FindObjectOfType<GameProgression>();
		var levels = gp.maps;
		for (int i = 0; i < levels.Length; i++)
		{
			list.Add(new TMP_Dropdown.OptionData(levels[i].title));
		}
		levelSelect.options = list;
		levelSelect.onValueChanged.AddListener(gp.LoadLevel);
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
	}
}
