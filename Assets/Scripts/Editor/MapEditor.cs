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
			tex = null;
		}
		if (EditorApplication.isPlaying && GUILayout.Button("Get Current"))
		{
			(target as Map).CopyCurrent();
			Save();
			tex = null;
		}
		if (GUILayout.Button("Redraw"))
		{
			tex = null;
		}
		if (GUILayout.Button("Force Save"))
			Save();
		if (tex == null)
			CreateImage();
		var rect = EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		EditorGUI.DrawPreviewTexture(rect, tex, null, ScaleMode.ScaleToFit);
		EditorGUILayout.EndVertical();
	}

	void Save()
	{
		AssetDatabase.Refresh();
		EditorUtility.SetDirty(target);
		AssetDatabase.SaveAssets();
	}

	void CreateImage()
	{
		Map map = (Map)target;
		tex = new Texture2D(map.width, map.height);
		var colors = tex.GetPixels();
		var layout = map.map;
		for (int i = 0; i < map.map.Length; i++)
		{
			switch (layout[i])
			{
				case ComponentType.Wire:
					colors[i] = Color.cyan;
					break;
				case ComponentType.Empty:
					colors[i] = Color.white;
					break;
				case ComponentType.Unbuildable:
					colors[i] = Color.black;
					break;
				case ComponentType.Input:
					colors[i] = Color.green;
					break;
				case ComponentType.Output:
					colors[i] = Color.red;
					break;
				default:
					colors[i] = Color.blue;
					break;
			}
		}
		tex.SetPixels(colors);
		tex.filterMode = FilterMode.Point;
		tex.Apply();
	}
}