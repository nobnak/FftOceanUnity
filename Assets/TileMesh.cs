using UnityEngine;
using System.Collections;
using FFTWSharp;
using System.Runtime.InteropServices;

public class TileMesh : MonoBehaviour {
	public int nxTile;
	public int nyTile;
	public Material tileMat;

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
	private GameObject[] _tiles;

	void Start () {
		_hspec = new HeightSpectrum(N, length, wind, amplitude);
		_height = new float[2 * N * N];
		_fftBufIn = fftwf.malloc(8 * N * N);
		_fftBufOut = fftwf.malloc(8 * N * N);
		_fftPlan = fftwf.dft_2d(N, N, _fftBufIn, _fftBufOut, fftw_direction.Backward, fftw_flags.Estimate);

		var fresnelTex = MakeFresnelLookUp();
		tileMat.SetTexture("_FresnelLookUp", fresnelTex);

		_mesh = MakeMesh(N, length);
		_tiles = new GameObject[nxTile * nyTile];
		var centerOffset = new Vector3(-0.5f * nxTile * length, 0f, -0.5f * nyTile * length);
		for (var y = 0; y < nyTile; y++) {
			for (var x = 0; x < nxTile; x++) {
				var go = _tiles[y * nxTile + x] = new GameObject();
				go.AddComponent<MeshFilter>().sharedMesh = _mesh;
				go.AddComponent<MeshRenderer>().sharedMaterial = tileMat;
				go.transform.parent = transform;
				go.transform.localPosition = new Vector3(x * length, 0f, y * length) + centerOffset;
			}
		}
	}

	void OnDestroy() {
		fftwf.destroy_plan(_fftPlan);
		fftwf.free(_fftBufIn);
		fftwf.free(_fftBufOut);
		for (var i = 0; i < _tiles.Length; i++)
			Destroy(_tiles[i]);
	}
	
	void Update () {
		var spectrum = _hspec.SpectrumShift(Time.timeSinceLevelLoad);
		Marshal.Copy(spectrum, 0, _fftBufIn, spectrum.Length);
		fftwf.execute(_fftPlan);
		Marshal.Copy(_fftBufOut, _height, 0, _height.Length);

		_mesh.vertices = UpdateVerties(_mesh.vertices, _height, N);
		_mesh.RecalculateNormals();
	}

	Texture2D MakeFresnelLookUp()
	{
		float nSnell = 1.34f; //Refractive index of water
		
		var fresnelTex = new Texture2D(512, 1, TextureFormat.Alpha8, false);
		fresnelTex.filterMode = FilterMode.Bilinear;
		fresnelTex.wrapMode = TextureWrapMode.Clamp;
		fresnelTex.anisoLevel = 0;
		
		for(int x = 0; x < 512; x++)
		{
			float fresnel = 0.0f;
			float costhetai = (float)x/511.0f;
			float thetai = Mathf.Acos(costhetai);
			float sinthetat = Mathf.Sin(thetai)/nSnell;
			float thetat = Mathf.Asin(sinthetat);
			
			if(thetai == 0.0f)
			{
				fresnel = (nSnell - 1.0f)/(nSnell + 1.0f);
				fresnel = fresnel * fresnel;
			}
			else
			{
				float fs = Mathf.Sin(thetat - thetai) / Mathf.Sin(thetat + thetai);
				float ts = Mathf.Tan(thetat - thetai) / Mathf.Tan(thetat + thetai);
				fresnel = 0.5f * ( fs*fs + ts*ts );
			}
			
			fresnelTex.SetPixel(x, 0, new Color(fresnel,fresnel,fresnel,fresnel));
		}
		
		fresnelTex.Apply();
		return fresnelTex;
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
