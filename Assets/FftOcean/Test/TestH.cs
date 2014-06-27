using UnityEngine;
using System.Collections;
using FFTWSharp;
using System.Runtime.InteropServices;

public class TestH : MonoBehaviour {
	public int n = 32;
	public float L = 32f;
	public Vector2 wind = new Vector2(1f, 0f);
	public float heightScale = 0.01f;

	private Texture2D _tex;
	private Material _mat;

	private K _k;
	private Phillips _phillips;
	private H0 _h0;
	private W _w;
	private H _h;

	private ComplexArray _fftIn, _fftOut;
	private fftwf_plan _fftPlan;
	private float[] _heightField;

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

		_fftIn = new ComplexArray(_h.Current);
		_fftOut = new ComplexArray(_h.Current);
		_fftPlan = fftwf_plan.dft_2d(n, n, _fftIn, _fftOut, fftw_direction.Backward, fftw_flags.Estimate);
		_heightField = new float[_h.Current.Length];
	}

	void Update() {
		_h.Jump(Time.timeSinceLevelLoad);
		_fftIn.SetData(_h.Current);
		_fftPlan.Execute();
		_fftOut.GetData(_heightField);

		var colors = _tex.GetPixels();
		var fftScale = 1f / Mathf.Sqrt(n);
		for (var y = 0; y < n; y++) {
			for (var x = 0; x < n; x++) {
				var i = x + y * n;
				var hx = fftScale * heightScale * _heightField[2 * i];
				colors[i] = ColorUtil.EncodeFloatRGBA(hx);
			}
		}
		_tex.SetPixels(colors);
		_tex.Apply();
	}

	public class ComplexArray : fftwf_complexarray {

		public ComplexArray(float[] data) : base(data) {}

		public void GetData(float[] data) {
			Marshal.Copy(Handle, data, 0, data.Length);
		}
	}
}
