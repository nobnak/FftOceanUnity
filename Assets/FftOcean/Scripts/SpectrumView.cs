﻿using UnityEngine;
using System.Collections;
using FFTWSharp;
using System.Runtime.InteropServices;

public class SpectrumView : MonoBehaviour {
	public int N;
	public float length;
	public Vector2 wind;
	public float amplitude;

	private HeightSpectrum _spec;
	private Texture2D _tex;

	void Start() {
		_spec = new HeightSpectrum(N, length, wind, amplitude);

		renderer.sharedMaterial.mainTexture = _tex = new Texture2D(N, N, TextureFormat.RGB24, false);
		_tex.filterMode = FilterMode.Point;
		_tex.anisoLevel = 0;
	}

	void Update() {
		var spectrum = _spec.SpectrumShift(Time.timeSinceLevelLoad);
		var s = spectrum;

		var colors = _tex.GetPixels();
		for (var i = 0; i < colors.Length; i++) {
			var sx = s[2 * i];
			var sy = s[2 * i + 1];
			colors[i] = new Color(Mathf.Abs(sx), Mathf.Abs(sy), 0f, 1f);
		}
		_tex.SetPixels(colors);
		_tex.Apply();
	}
}
