using UnityEngine;
using System.Collections;

public class SpectrumView : MonoBehaviour {
}

public class HeightSpectrum {
	private int _N;
	private float _length;

	private Vector2[] _heightSpectrum0;
	private Vector2[] _heightSpectrum0Conj;

	public HeightSpectrum(int N, float length) {
		this._N = N;
		this._length = length;

		this._heightSpectrum0 = new Vector2[N * N];
		this._heightSpectrum0Conj = new Vector2[N * N];

		for (var m = 0; m < N; m++) {
			for (var n = 0; n < N; n++) {
				var index = m * N + n;
				_heightSpectrum0[index] = StandardNormalDistribution() * Mathf.Sqrt(PhillipsSpectrum(n, m) * 0.5f);
				_heightSpectrum0Conj[index] = StandardNormalDistribution() * Mathf.Sqrt(PhillipsSpectrum(N - n, N - m) * 0.5f);
				_heightSpectrum0Conj[index].y *= -1f;
			}
		}
	}

	public static Vector2 StandardNormalDistribution() {
		var u1 = Random.value;
		var u2 = Random.value;
		var sqrt = Mathf.Sqrt(-2f * Mathf.Log(u1));
		var theta = 2f * Mathf.PI * u2;
		var z0 = sqrt * Mathf.Cos(theta);
		var z1 = sqrt * Mathf.Sin(theta);
		return new Vector2(z0, z1);
	}

	public float PhillipsSpectrum(int n, int m) {
	}
}