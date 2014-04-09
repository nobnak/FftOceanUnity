using UnityEngine;
using System.Collections;

public class SpectrumView : MonoBehaviour {
}

public class HeightSpectrum {
	public const float TWO_PI = 2f * Mathf.PI;
	public const float GRAVITY = 9.81f;
	public const float w0 = TWO_PI / 200.0f;

	private int _N;
	private float _length;
	private float _windSpeed;
	private Vector2 _windDirection;
	private float _amplitude;

	private Vector2[] _heightSpectrum0;
	private Vector2[] _heightSpectrum0Conj;
	private float[] _omega;

	public HeightSpectrum(int N, float length, Vector2 wind, float amplitude) {
		this._N = N;
		this._length = length;
		this._windSpeed = wind.magnitude;
		this._windDirection = wind.normalized;
		this._amplitude = amplitude;

		this._heightSpectrum0 = new Vector2[N * N];
		this._heightSpectrum0Conj = new Vector2[N * N];
		this._omega = new float[N * N];

		for (var m = 0; m < N; m++) {
			for (var n = 0; n < N; n++) {
				var index = m * N + n;
				_heightSpectrum0[index] = StandardNormalDistribution() * Mathf.Sqrt(PhillipsSpectrum(n, m) * 0.5f);
				_heightSpectrum0Conj[index] = StandardNormalDistribution() * Mathf.Sqrt(PhillipsSpectrum(N - n, N - m) * 0.5f);
				_heightSpectrum0Conj[index].y *= -1f;
				_omega[index] = Dispertion(n, m);
			}
		}
	}

	public static Vector2 StandardNormalDistribution() {
		var u1 = Random.value;
		var u2 = Random.value;
		var sqrt = Mathf.Sqrt(-2f * Mathf.Log(u1));
		var theta = TWO_PI * u2;
		var z0 = sqrt * Mathf.Cos(theta);
		var z1 = sqrt * Mathf.Sin(theta);
		return new Vector2(z0, z1);
	}

	public float PhillipsSpectrum(int n, int m) {
		var k = K(n, m);
		var k2mag = k.sqrMagnitude;
		var k6mag = k2mag * k2mag * k2mag;
		var L = _windSpeed * _windSpeed / GRAVITY;
		var kDotW = Vector2.Dot(k, _windDirection);
		return _amplitude * Mathf.Exp(-1f / (k2mag * L * L)) * (kDotW * kDotW) / k6mag;
	}

	public float Dispertion(int n, int m) {
		var k = K(n, m);
		var w = Mathf.Sqrt(GRAVITY * k.magnitude);
		return Mathf.Floor(w / w0) * w0;
	}

	public Vector2 K(int n, int m) {
		return new Vector2(Mathf.PI * (2f * n - _N) / _length, Mathf.PI * (2f * m - _N) / _length);
	}
}