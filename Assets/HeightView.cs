using UnityEngine;
using System.Collections;
using FFTWSharp;
using System.Runtime.InteropServices;

public class HeightView : MonoBehaviour {
	public int N;
	public float length;
	public Vector2 wind;
	public float amplitude;
	
	private HeightSpectrum _spec;
	private Texture2D _tex;
	private float[] _height;
	private System.IntPtr _fftIn, _fftOut, _fftPlan;

	void Start () {
		_spec = new HeightSpectrum(N, length, wind, amplitude);
		_height = new float[2 * N * N];
		_fftIn = fftwf.malloc(8 * N * N);
		_fftOut = fftwf.malloc(8 * N * N);
		_fftPlan = fftwf.dft_2d(N, N, _fftIn, _fftOut, fftw_direction.Backward, fftw_flags.Estimate);

		renderer.sharedMaterial.mainTexture = _tex = new Texture2D(N, N, TextureFormat.RGB24, false);
		_tex.filterMode = FilterMode.Point;
		_tex.anisoLevel = 0;
	}

	void OnDestroy() {
		fftwf.destroy_plan(_fftPlan);
		fftwf.free(_fftIn);
		fftwf.free(_fftOut);
	}

	void Update () {
		var spectrum = _spec.Spectrum(Time.timeSinceLevelLoad);
		var specShift = new float[spectrum.Length];
		HeightSpectrum.ShiftComplex(spectrum, specShift, N);
		spectrum = specShift;

		Marshal.Copy(spectrum, 0, _fftIn, spectrum.Length);
		fftwf.execute(_fftPlan);
		Marshal.Copy(_fftOut, _height, 0, _height.Length);

		var amp = 1f / Mathf.Sqrt(N * N);
		var colors = _tex.GetPixels();
		for (var i = 0; i < colors.Length; i++) {
			var h = _height[2 * i];
			h = amp * 0.5f * (h + 1f);
			colors[i] = new Color(h, h, h, 1f);
		}
		_tex.SetPixels(colors);
		_tex.Apply();
	}
}
