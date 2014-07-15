using UnityEngine;
using System.Collections;
using FFTWSharp;
using System.Runtime.InteropServices;

public class TestH : MonoBehaviour {
	public int n = 32;
	public float L = 32f;
	public Vector2 wind = new Vector2(1f, 0f);

	public Material floatDecoderMat;
	public Material displacementMat;

	private K _k;
	private Phillips _phillips;
	private H0 _h0;
	private W _w;
	private H _h;
	private Eta _eta;

	private FFT _fft;
	private Texture2D _texPw, _texEtaX, _texEtaY;
	private Color[] _cPw, _cEtaX, _cEtaY;
	private float[] _pw, _etaX, _etaY;
	private float _maxPw = 0f, _maxEtaX = 0f, _maxEtaY = 0f;
	private RenderTexture _texUvw;

	void Start () {
		_texPw = new Texture2D(n, n, TextureFormat.RGBA32, false, true);
		_texEtaX = new Texture2D(n, n, TextureFormat.RGBA32, false, true);
		_texEtaY = new Texture2D(n, n , TextureFormat.RGBA32, false, true);
		_cPw = _texPw.GetPixels();
		_cEtaX = _texEtaX.GetPixels();
		_cEtaY = _texEtaY.GetPixels();
		displacementMat.SetFloat("_L", L);
		floatDecoderMat.SetFloat("_L", L);
		floatDecoderMat.SetTexture("_XTex", _texEtaX);
		floatDecoderMat.SetTexture("_YTex", _texEtaY);
		floatDecoderMat.SetTexture("_ZTex", _texPw);

		_texUvw = new RenderTexture(n, n, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
		displacementMat.mainTexture = _texUvw;

		_k = new K(n, L);
		_phillips = new Phillips(_k, wind.magnitude, wind.normalized);
		_h0 = new H0(_phillips);
		_w = new W(_k);
		_h = new H(_h0, _w);
		_eta = new Eta(n, _k, _h);

		_fft = new FFT(n);
		_pw = new float[2 * n * n];
		_etaX = new float[_pw.Length];
		_etaY = new float[_pw.Length];
	}

	void Update() {
		_h.Jump(Time.timeSinceLevelLoad);
		_eta.Jump(Time.timeSinceLevelLoad);
		float scaleEtaX, scaleEtaY, scalePw;
		_fft.Execute(_eta.Ex, ref _maxEtaX, out scaleEtaX, _etaX);
		_fft.Execute(_eta.Ey, ref _maxEtaY, out scaleEtaY, _etaY);
		_fft.Execute(_h.Current, ref _maxPw, out scalePw, _pw);

		var scaleEtaXInv = 1f / scaleEtaX;
		var scaleEtaYInv = 1f / scaleEtaY;
		var scalePwInv = 1f / scalePw;

		for (var y = 0; y < n; y++) {
			for (var x = 0; x < n; x++) {
				var i = x + y * n;
				var pEtaX = _etaX[i] * scaleEtaXInv + 0.5f;
				var pEtaY = _etaY[i] * scaleEtaYInv + 0.5f;
				var pw = _pw[i] * scalePwInv + 0.5f;
				_cEtaX[i] = ColorUtil.EncodeFloatRGBA2(pEtaX);
				_cEtaY[i] = ColorUtil.EncodeFloatRGBA2(pEtaY);
				_cPw[i] = ColorUtil.EncodeFloatRGBA2(pw);
			}
		}
		_texEtaX.SetPixels(_cEtaX);
		_texEtaY.SetPixels(_cEtaY);
		_texPw.SetPixels(_cPw);
		_texEtaX.Apply();
		_texEtaY.Apply();
		_texPw.Apply();

		floatDecoderMat.SetVector("_Scale", new Vector4(scaleEtaX, scaleEtaY, scalePw, 1.0f));
		Graphics.Blit(_texPw, _texUvw, floatDecoderMat);
	}

	public class ComplexArray : fftwf_complexarray {

		public ComplexArray(float[] data) : base(data) {}

		public void GetData(float[] data) {
			Marshal.Copy(Handle, data, 0, data.Length);
		}
	}
}
