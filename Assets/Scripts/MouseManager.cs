using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour {

	enum DragType
	{
		build,
		move,
		destroy
	}

	public LineRenderer deletionMarker;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	Camera camera;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
	EventSystem es;
	Circuit circuit;

	bool isDragging;
	Vector3 startPos;
	DragType drag;


	private void Start()
	{
		es = EventSystem.current;
		camera = Camera.main;
		circuit = FindObjectOfType<Circuit>();
	}

	void Update ()
	{
		if (isDragging && es.IsPointerOverGameObject()) {
			isDragging = false;
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (es.IsPointerOverGameObject())
				return;
			startPos = camera.ScreenToWorldPoint(Input.mousePosition);
			startPos.x = Mathf.Round(startPos.x);
			startPos.y = Mathf.Round(startPos.y);
			CircuitTile tile;
			if (circuit.GetTileAt(startPos, out tile) && !tile.unbuildable)
			{
				isDragging = true;
				if (tile.component == null)
					drag = DragType.build;
				else
				{
					//TODO: Check wire&wire
					drag = DragType.move;
				}
			}
		}
		else if (Input.GetMouseButtonDown(1))
		{
			if (es.IsPointerOverGameObject())
				return;
			isDragging = true;
			startPos = camera.ScreenToWorldPoint(Input.mousePosition);
			drag = DragType.destroy;
		}
		if (isDragging)
		{
			switch (drag)
			{
				case DragType.build:
					DragBuild();
					break;
				case DragType.move:
					DragMove();
					break;
				case DragType.destroy:
					DragDelete();
					break;
			}
		}
	}

	void DragBuild()
	{

	}

	void DragMove()
	{

	}

	void DragDelete()
	{
		Vector3 endPos = camera.ScreenToWorldPoint(Input.mousePosition);
		if (Input.GetMouseButtonUp(1))
		{
			isDragging = false;
			deletionMarker.gameObject.SetActive(false);
			circuit.DeleteArea(startPos, endPos);
		}
		else
		{
			float topx = Mathf.Ceil(Mathf.Max(startPos.x, endPos.x));
			float topy = Mathf.Ceil(Mathf.Max(startPos.y, endPos.y));
			float botx = Mathf.Floor(Mathf.Min(startPos.x, endPos.x));
			float boty = Mathf.Floor(Mathf.Min(startPos.y, endPos.y));
			deletionMarker.gameObject.SetActive(true);
			deletionMarker.SetPosition(0, new Vector3(topx, topy));
			deletionMarker.SetPosition(1, new Vector3(topx, boty));
			deletionMarker.SetPosition(2, new Vector3(botx, boty));
			deletionMarker.SetPosition(3, new Vector3(botx, topy));
			deletionMarker.SetPosition(4, new Vector3(topx, topy));
			deletionMarker.SetPosition(5, new Vector3(topx, boty));
			deletionMarker.SetPosition(6, new Vector3(botx, topy));
			deletionMarker.SetPosition(7, new Vector3(botx, boty));
			deletionMarker.SetPosition(8, new Vector3(topx, topy));
		}
	}

}
