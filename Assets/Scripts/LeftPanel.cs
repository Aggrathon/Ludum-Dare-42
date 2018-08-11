﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeftPanel : MonoBehaviour {

	public GameObject[] collapsedObjects;
	public GameObject[] uncollapsedObjects;

	public float collapsedSize;
	public float uncollapsedSize;

	public TMP_Dropdown levelSelect;

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
	}
}