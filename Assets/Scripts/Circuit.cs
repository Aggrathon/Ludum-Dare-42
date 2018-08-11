using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circuit : MonoBehaviour {

	[Header("Data")]
	[SerializeField] GameObject[] components;
	[NonSerialized] public CircuitTile[] circuit;
	[NonSerialized] public Map map;
	[Header("Objects")]
	[SerializeField] Transform background;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	[SerializeField] Camera camera;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword


	public void Setup(Map map)
	{
		background.localScale = new Vector3(map.width, map.height, 1);
		camera.orthographicSize = Mathf.Max(map.width, map.height);
		this.map = map;

		var layout = map.map;
		circuit = new CircuitTile[map.height * map.width];
		int w = map.width;
		int input = 0;
		int output = 0;
		for (int i = 0; i < map.height; i++)
		{
			for (int j = 0; j < map.width; j++)
			{
				int index = i * w + j;
				circuit[index].x = j;
				circuit[index].y = i;
				switch (layout[index])
				{
					case 1:
						circuit[index].unbuildable = true;
						circuit[index].inputIndex = -1;
						circuit[index].outputIndex = -1;
						break;
					case 2:
						circuit[index].unbuildable = true;
						circuit[index].inputIndex = input++;
						circuit[index].outputIndex = -1;
						break;
					case 3:
						circuit[index].unbuildable = true;
						circuit[index].inputIndex = -1;
						circuit[index].outputIndex = output++;
						break;
					default:
						break;
				}
			}
		}
	}
}

[Serializable]
public struct CircuitTile
{
	public int x;
	public int y;
	public bool unbuildable;
	public int inputIndex;
	public int outputIndex;
}
