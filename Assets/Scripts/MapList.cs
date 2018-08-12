using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName ="Maplist", menuName ="Scriptable Object/Maplist")]
public class MapList : ScriptableObject
{
	public List<Map> maps;
	public List<MapList> otherLists;

	public List<Map> GetAllMaps()
	{
		var list = new List<Map>();
		list.AddRange(maps);
		for (int i = 0; i < otherLists.Count; i++)
			list.AddRange(otherLists[i].GetAllMaps());
		return list;
	}
}
