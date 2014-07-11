using UnityEngine;
using System.Collections;

public class SurfaceFiller : MonoBehaviour {
	public Mesh mesh;
	public Material mat;
	public int n = 8;
	public float L = 32f;

	void Start () {
		for (var y = 0; y < n; y++) {
			for (var x = 0; x < n; x++) {
				var go = new GameObject("Uniform Mesh");
				var mf = go.AddComponent<MeshFilter>();
				var mr = go.AddComponent<MeshRenderer>();

				go.transform.parent = transform;
				go.transform.localPosition = new Vector3(L * x, 0f, L * y);
				go.transform.localScale = new Vector3(L, 1f, L);
				mf.sharedMesh = mesh;
				mr.sharedMaterial = mat;
			}
		}
	}
}
