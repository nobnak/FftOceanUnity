using UnityEngine;
using UnityEditor;
using System.Collections;

public static class SurfaceMaker {
	public const int VERTEX_LIMIT = 65534;

	[MenuItem("Custom/BuildSurface")]
	public static void Build() {
		var n = Mathf.FloorToInt(Mathf.Sqrt(VERTEX_LIMIT));
		var nEdges = n - 1;
		var vertices = new Vector3[n * n];
		var uvs = new Vector2[vertices.Length];
		var indices = new int[6 * nEdges * nEdges];

		var dx = 1f / nEdges;

		for (var y = 0; y < n; y++) {
			for (var x = 0; x < n; x++) {
				var i = x + y * n;
				vertices[i] = new Vector3(x * dx, 0f, y * dx);
				uvs[i] = new Vector2(x * dx, y * dx);
			}
		}

		var counter = 0;
		for (var y = 0; y < nEdges; y++) {
			for (var x = 0; x < nEdges; x++) {
				var offset = x + y * n;
				indices[counter++] = offset;
				indices[counter++] = offset + n + 1;
				indices[counter++] = offset + 1;
				indices[counter++] = offset;
				indices[counter++] = offset + n;
				indices[counter++] = offset + n + 1;
			}
		}

		var mesh = new Mesh();
		mesh.name = "Unit Surface";
		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.uv = uvs;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		AssetDatabase.CreateAsset(mesh, "Assets/FftOcean/Test/UnitSurface.asset");
	}
}
