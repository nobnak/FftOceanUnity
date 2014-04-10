using UnityEngine;


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
	private float[] _spectrum;
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
		this._spectrum = new float[2 * N * N];
		
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
	
	public float[] Spectrum(float t) {
		for (var m = 0; m < _N; m++) {
			for (var n = 0; n < _N; n++) {
				var index = m * _N + n;
				var s = Spectrum(n, m, t);
				_spectrum[2 * index] = s.x;
				_spectrum[2 * index + 1] = s.y;
			}
		}
		return _spectrum;
	}

	public float[] SpectrumShift(float t) {
		var N2 = _N / 2;
		for (var m = 0; m < _N; m++) {
			for (var n = 0; n < _N; n++) {
				var index = ((m + N2) % _N) * _N + (n + N2) % _N;
				var s = Spectrum(n, m, t);
				_spectrum[2 * index] = s.x;
				_spectrum[2 * index + 1] = s.y;
			}
		}
		return _spectrum;
	}
	
	public Vector2 Spectrum(int n, int m, float t) {
		var index = m * _N + n;
		var h = _heightSpectrum0[index];
		var hconj = _heightSpectrum0Conj[index];
		var theta = _omega[index] * t;
		var cos = Mathf.Cos(theta);
		var sin = Mathf.Sin(theta);
		
		var sx = (h.x * cos - h.y * sin) + (hconj.x * cos + hconj.y * sin);
		var sy = (h.x * sin + h.y * cos) + (-hconj.x * sin + hconj.y * cos);
		return new Vector2(sx, sy);
	}
	
	public float PhillipsSpectrum(int n, int m) {
		var k = K(n, m);
		var k2mag = k.sqrMagnitude;
		if (k2mag < 1e-12f)
			return 0f;

		var k4mag = k2mag * k2mag;
		var k6mag = k4mag * k2mag;
		var L = _windSpeed * _windSpeed / GRAVITY;
		var kDotW = Vector2.Dot(k, _windDirection);
		var kDotW2 = kDotW * kDotW * kDotW * kDotW * kDotW * kDotW / k6mag;

		float damping   = 0.001f;
		float l2        = L * L * damping * damping;

		return _amplitude * Mathf.Exp(-1f / (k2mag * L * L)) * kDotW2 / k4mag * Mathf.Exp(- k2mag * l2);
	}
	
	public float Dispertion(int n, int m) {
		var k = K(n, m);
		var w = Mathf.Sqrt(GRAVITY * k.magnitude);
		return Mathf.Floor(w / w0) * w0;
	}
	
	public Vector2 K(int n, int m) {
		return new Vector2(Mathf.PI * (2f * n - _N) / _length, Mathf.PI * (2f * m - _N) / _length);
	}
	
	public static void ShiftComplex(float[] dataIn, float[] dataOut, int N) {
		var N2 = N / 2;
		for (var m = 0; m < N; m++) {
			for (var n = 0; n < N; n++) {
				var indexIn = m * N + n;
				var indexOut = ((m + N2) % N) * N + (n + N2) % N;
				dataOut[2 * indexOut] = dataIn[2 * indexIn];
				dataOut[2 * indexOut + 1] = dataIn[2 * indexIn + 1];
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
}