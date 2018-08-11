using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
	Texture2D tex;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if (GUILayout.Button("Reset"))
		{
			(target as Map).Reset();
		}
		if (EditorApplication.isPlaying && GUILayout.Button("Get Current"))
		{
			(target as Map).CopyCurrent();
		}
		if (GUILayout.Button("Redraw"))
		{
			tex = null;
		}
		if (tex == null)
			CreateImage();
		var rect = EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		EditorGUI.DrawPreviewTexture(rect, tex, null, ScaleMode.ScaleToFit);
		EditorGUILayout.EndVertical();
	}

	void CreateImage()
	{
		Map map = (Map)target;
		tex = new Texture2D(map.width, map.height);
		var colors = tex.GetPixels();
		for (int i = 0; i < map.map.Length; i++)
		{
			switch (map.map[i])
			{
				case 1:
					colors[i] = Color.black;
					break;
				case 2:
					colors[i] = Color.green;
					break;
				case 3:
					colors[i] = Color.red;
					break;
				default:
					colors[i] = Color.white;
					break;
			}
		}
		tex.SetPixels(colors);
		tex.filterMode = FilterMode.Point;
		tex.Apply();
	}
}