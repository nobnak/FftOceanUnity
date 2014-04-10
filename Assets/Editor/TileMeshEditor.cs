using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TileMesh))]
public class TileMeshEditor : Editor {

	public override void OnInspectorGUI () {
		DrawDefaultInspector();
		if (GUILayout.Button("Bake")) {
			var tileMesh = (TileMesh)target;
			AssetDatabase.CreateAsset(tileMesh.Ocean, "Assets/OceanTile.asset");
		}
	}
}
