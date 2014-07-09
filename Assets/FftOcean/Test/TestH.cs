using UnityEngine;
using System.Collections;
using FFTWSharp;
using System.Runtime.InteropServices;

public class TestH : MonoBehaviour {
	public int n = 32;
	public float L = 32f;
	public Vector2 wind = new Vector2(1f, 0f);
	public float heightScale = 0.01f;
	public float lambda = 0.01f;

	public Material mat;

	private K _k;
	private Phillips _phillips;
	private H0 _h0;
	private W _w;
	private H _h;
	private D _d;

	private FFT _fft;
	private Texture2D _texPu, _texPv, _texPw;
	private Color[] _cPu, _cPv, _cPw;
	private float[] _pu, _pv, _pw;

	// Use this for initialization
	void Start () {
		_texPu = new Texture2D(n, n, TextureFormat.RGBA32, false, true);
		_texPv = new Texture2D(n, n, TextureFormat.RGBA32, false, true);
		_texPw = new Texture2D(n, n, TextureFormat.RGBA32, false, true);
		_cPu = _texPu.GetPixels();
		_cPv = _texPv.GetPixels();
		_cPw = _texPw.GetPixels();
		mat.SetFloat("_L", L);
		mat.SetTexture("_XTex", _texPu);
		mat.SetTexture("_YTex", _texPv);
		mat.SetTexture("_ZTex", _texPw);

		_k = new K(n, L);
		_phillips = new Phillips(_k, wind.magnitude, wind.normalized);
		_h0 = new H0(_phillips);
		_w = new W(_k);
		_h = new H(_h0, _w);
		_d = new D(n, _k, _h);

		_fft = new FFT(n);
		_pu = new float[2 * n * n];
		_pv = new float[_pu.Length];
		_pw = new float[_pu.Length];
	}

	void Update() {
		_h.Jump(Time.timeSinceLevelLoad);
		_d.Jump(Time.timeSinceLevelLoad);
		float scalePu, scalePv, scalePw;
		_fft.Execute(_d.Dx, out scalePu, _pu);
		_fft.Execute(_d.Dy, out scalePv, _pv);
		_fft.Execute(_h.Current, out scalePw, _pw);

		var scalePuInv = 1f / scalePu;
		var scalePvInv = 1f / scalePv;
		var scalePwInv = 1f / scalePw;

		for (var y = 0; y < n; y++) {
			for (var x = 0; x < n; x++) {
				var i = x + y * n;
				var pu = _pu[i] * scalePuInv + 0.5f;
				var pv = _pv[i] * scalePvInv + 0.5f;
				var pw = _pw[i] * scalePwInv + 0.5f;
				_cPu[i] = ColorUtil.EncodeFloatRGBA2(pu);
				_cPv[i] = ColorUtil.EncodeFloatRGBA2(pv);
				_cPw[i] = ColorUtil.EncodeFloatRGBA2(pw);
			}
		}
		_texPu.SetPixels(_cPu);
		_texPv.SetPixels(_cPv);
		_texPw.SetPixels(_cPw);
		_texPu.Apply();
		_texPv.Apply();
		_texPw.Apply();

		mat.SetFloat("_ScaleX", scalePu);
		mat.SetFloat("_ScaleY", scalePv);
		mat.SetFloat("_ScaleZ", scalePw);
	}

	public class ComplexArray : fftwf_complexarray {

		public ComplexArray(float[] data) : base(data) {}

		public void GetData(float[] data) {
			Marshal.Copy(Handle, data, 0, data.Length);
		}
	}
}
