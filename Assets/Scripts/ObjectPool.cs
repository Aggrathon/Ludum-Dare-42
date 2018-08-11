using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

	static ObjectPool instance;

	Dictionary<GameObject, LinkedList<GameObject>> pool;

	private void OnEnable()
	{
		instance = this;
		pool = new Dictionary<GameObject, LinkedList<GameObject>>();
	}

	private void OnDisable()
	{
		instance = this;
	}

	public static GameObject Spawn(GameObject prefab, Vector3 position)
	{
		if (instance == null)
			return null;
		GameObject go;
		if (instance.pool.ContainsKey(prefab))
		{
			var list = instance.pool[prefab];
			if (list.Count > 0)
			{
				go = list.First.Value;
				list.RemoveFirst();
				go.transform.position = position;
				go.SetActive(true);
				return go;
			}
		}
		go = GameObject.Instantiate(prefab, position, Quaternion.identity);
		go.AddComponent<PoolObject>().prefab = prefab;
		return go;
	}

	public static void Despawn(PoolObject po, GameObject prefab)
	{
		if (instance == null)
			return;
		if (instance.pool.ContainsKey(prefab))
		{
			instance.pool[prefab].AddLast(po.gameObject);
		}
		else
		{
			var list = new LinkedList<GameObject>();
			list.AddLast(po.gameObject);
			instance.pool.Add(prefab, list);
		}
	}
}

public class PoolObject : MonoBehaviour
{
	internal GameObject prefab;
	private void OnDisable()
	{
		ObjectPool.Despawn(this, prefab);
	}
}
