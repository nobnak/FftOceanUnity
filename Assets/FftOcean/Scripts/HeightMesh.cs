using UnityEngine;
using System.Collections;
using FFTWSharp;
using System.Runtime.InteropServices;

public class HeightMesh : MonoBehaviour {
	public int N;
	public float length;
	public Vector2 wind;
	public float amplitude;

	private HeightSpectrum _hspec;
	private Mesh _mesh;
	private float[] _height;
	private System.IntPtr _fftBufIn;
	private System.IntPtr _fftBufOut;
	private System.IntPtr _fftPlan;

	void Start () {
		_hspec = new HeightSpectrum(N, length, wind, amplitude);
		_height = new float[2 * N * N];
		_fftBufIn = fftwf.malloc(8 * N * N);
		_fftBufOut = fftwf.malloc(8 * N * N);
		_fftPlan = fftwf.dft_2d(N, N, _fftBufIn, _fftBufOut, fftw_direction.Backward, fftw_flags.Estimate);

		var mfilter = GetComponent<MeshFilter>();
		mfilter.sharedMesh = _mesh = MakeMesh(N, length);
	}

	void OnDestroy() {
		fftwf.destroy_plan(_fftPlan);
		fftwf.free(_fftBufIn);
		fftwf.free(_fftBufOut);
	}
	
	void Update () {
		var spectrum = _hspec.SpectrumShift(Time.timeSinceLevelLoad);
		Marshal.Copy(spectrum, 0, _fftBufIn, spectrum.Length);
		fftwf.execute(_fftPlan);
		Marshal.Copy(_fftBufOut, _height, 0, _height.Length);

		_mesh.vertices = UpdateVerties(_mesh.vertices, _height, N);
		_mesh.RecalculateNormals();
	}

	static Mesh MakeMesh(int N, float length) {
		var m = new Mesh ();
		m.MarkDynamic ();
		var nPlus1 = N + 1;
		var dx = length / N;
		var vertices = new Vector3[nPlus1 * nPlus1];
		for (var y = 0; y < nPlus1; y++) {
			for (var x = 0; x < nPlus1; x++) {
				var index = y * nPlus1 + x;
				vertices [index] = new Vector3 (x * dx, 0f, y * dx);
			}
		}
		m.vertices = vertices;
		var triangles = new int[6 * N * N];
		var counter = 0;
		for (var y = 0; y < N; y++) {
			for (var x = 0; x < N; x++) {
				var vindex = y * nPlus1 + x;
				triangles [counter++] = vindex;
				triangles [counter++] = vindex + nPlus1 + 1;
				triangles [counter++] = vindex + 1;
				triangles [counter++] = vindex;
				triangles [counter++] = vindex + nPlus1;
				triangles [counter++] = vindex + nPlus1 + 1;
			}
		}
		m.triangles = triangles;
		m.RecalculateNormals();
		return m;
	}

	static Vector3[] UpdateVerties(Vector3[] vertices, float[] _height, int N) {
		var nPlus1 = N + 1;
		var amp = 1f / Mathf.Sqrt(N * N);
		for (var y = 0; y < N; y++) {
			for (var x = 0; x < N; x++) {
				var hindex = y * N + x;
				var vindex = y * nPlus1 + x;
				
				var h = amp * _height[2 * hindex];
				var v = vertices[vindex];
				v.y = h;
				vertices[vindex] = v;
			}
			{
				var hindex = y * N;
				var vindex = y * nPlus1 + N;
				var v = vertices[vindex];
				var h = amp * _height[2 * hindex];
				v.y = h;
				vertices[vindex] = v;
			}
		}
		for (var x = 0; x < N; x++) {
			var vindex = N * nPlus1 + x;
			var v = vertices[vindex];
			v.y = amp * _height[2 * x];
			vertices[vindex] = v;
		}
		{
			var v = vertices[nPlus1 * nPlus1 - 1];
			var h = amp * _height[2 * N];
			v.y = h;
			vertices[nPlus1 * nPlus1 - 1] = v;
		}
		return vertices;
	}
}
