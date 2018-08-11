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

	[System.Serializable]
	public struct ComponentPreview
	{
		public string name;
		public Sprite icon;
		public ComponentType component;
	}

	public LineRenderer deletionMarker;
	public LineRenderer wireMarker;
	public SpriteRenderer ghostMarker;

	public ComponentPreview[] previews;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	Camera camera;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
	EventSystem es;
	Circuit circuit;

	bool isDragging;
	IntVector startPos;
	DragType drag;
	ComponentType build = ComponentType.Empty;


	private void Start()
	{
		es = EventSystem.current;
		camera = Camera.main;
		circuit = FindObjectOfType<Circuit>();
	}

	void Update ()
	{
		if (isDragging && es.IsPointerOverGameObject()) {
			deletionMarker.gameObject.SetActive(false);
			wireMarker.gameObject.SetActive(false);
			ghostMarker.gameObject.SetActive(false);
			isDragging = false;
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (es.IsPointerOverGameObject())
				return;
			startPos = circuit.WorldToLocal(camera.ScreenToWorldPoint(Input.mousePosition), true);
			CircuitTile tile;
			if (circuit.GetTileAt(startPos, out tile) && tile.component != ComponentType.Unbuildable && tile.component != ComponentType.Input && tile.component != ComponentType.Output)
			{
				isDragging = true;
				if (tile.component == ComponentType.Empty || tile.component == ComponentType.Wire)
				{
					drag = DragType.build;
					if (build != ComponentType.Wire)
					{
						ghostMarker.gameObject.SetActive(true);
						for (int i = 0; i < previews.Length; i++)
							if (previews[i].component == build)
							{
								ghostMarker.sprite = previews[i].icon;
								break;
							}

					}
					else
					{
						wireMarker.gameObject.SetActive(true);
					}
				}
				else
					drag = DragType.move;
			}
		}
		else if (Input.GetMouseButtonDown(1))
		{
			if (es.IsPointerOverGameObject())
				return;
			isDragging = true;
			startPos = circuit.WorldToLocal(camera.ScreenToWorldPoint(Input.mousePosition));
			drag = DragType.destroy;
			deletionMarker.gameObject.SetActive(true);
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
		IntVector endPos = circuit.WorldToLocal(camera.ScreenToWorldPoint(Input.mousePosition), true);
		if (build == ComponentType.Wire)
		{
			IntVector start = new IntVector(Mathf.Min(startPos.x, endPos.x), Mathf.Min(startPos.y, endPos.y));
			IntVector end = new IntVector(Mathf.Max(startPos.x, endPos.x), Mathf.Max(startPos.y, endPos.y));
			if (end.x - start.x > end.y - start.y)
			{
				start.y = startPos.y;
				end.y = startPos.y;
			}
			else
			{
				start.x = startPos.x;
				end.x = startPos.x;
			}
			if (Input.GetMouseButtonUp(0))
			{
				wireMarker.gameObject.SetActive(false);
				isDragging = false;
				circuit.DrawWire(start, end);
			}
			else
			{
				wireMarker.SetPosition(0, circuit.LocalToWorld(start));
				wireMarker.SetPosition(1, circuit.LocalToWorld(end));
			}
		}
		else
		{
			if (Input.GetMouseButtonUp(0))
			{
				ghostMarker.gameObject.SetActive(false);
				isDragging = false;
				circuit.BuildComponent(build, endPos);
			}
			else
			{
				ghostMarker.transform.position = circuit.LocalToWorld(endPos);
			}
		}
	}

	void DragMove()
	{

	}

	void DragDelete()
	{
		IntVector endPos = circuit.WorldToLocal(camera.ScreenToWorldPoint(Input.mousePosition));
		if (Input.GetMouseButtonUp(1))
		{
			isDragging = false;
			deletionMarker.gameObject.SetActive(false);
			circuit.DeleteArea(startPos, endPos);
		}
		else
		{
			Vector3 top = circuit.LocalToWorld(new IntVector(Mathf.Max(startPos.x, endPos.x), Mathf.Max(startPos.y, endPos.y))) + new Vector3(0.5f, 0.5f);
			Vector3 bot = circuit.LocalToWorld(new IntVector(Mathf.Min(startPos.x, endPos.x), Mathf.Min(startPos.y, endPos.y))) - new Vector3(0.5f, 0.5f);
			deletionMarker.SetPosition(0, new Vector3(top.x, top.y));
			deletionMarker.SetPosition(1, new Vector3(top.x, bot.y));
			deletionMarker.SetPosition(2, new Vector3(bot.x, bot.y));
			deletionMarker.SetPosition(3, new Vector3(bot.x, top.y));
			deletionMarker.SetPosition(4, new Vector3(top.x, top.y));
			deletionMarker.SetPosition(5, new Vector3(top.x, bot.y));
			deletionMarker.SetPosition(6, new Vector3(bot.x, top.y));
			deletionMarker.SetPosition(7, new Vector3(bot.x, bot.y));
			deletionMarker.SetPosition(8, new Vector3(top.x, top.y));
		}
	}


	public void SetComponentToBuild(ComponentType ctb)
	{
		build = ctb;
	}
}

