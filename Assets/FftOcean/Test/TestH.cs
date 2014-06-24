using UnityEngine;
using System.Collections;

public class TestH : MonoBehaviour {
	public int n = 32;
	public float L = 32f;
	public Vector2 wind = new Vector2(1f, 0f);
	public float colorGain = 1f;

	private Texture2D _tex;
	private Material _mat;

	private K _k;
	private Phillips _phillips;
	private H0 _h0;
	private W _w;
	private H _h;

	// Use this for initialization
	void Start () {
		_tex = new Texture2D(n, n, TextureFormat.RGB24, false);
		_tex.filterMode = FilterMode.Point;
		renderer.sharedMaterial.mainTexture = _tex;

		_k = new K(n, L);
		_phillips = new Phillips(_k, wind.magnitude, wind.normalized);
		_h0 = new H0(_phillips);
		_w = new W(_k);
		_h = new H(_h0, _w);

	}

	void Update() {
		_h.Jump(Time.timeSinceLevelLoad);

		var colors = _tex.GetPixels();
		for (var y = 0; y < n; y++) {
			for (var x = 0; x < n; x++) {
				var i = x + y * n;
				var h = _h[x, y];
				colors[i] = new Color(colorGain * h.x + 0.5f, colorGain * h.y + 0.5f, 0f);
			}
		}
		_tex.SetPixels(colors);
		_tex.Apply();
	}
}
