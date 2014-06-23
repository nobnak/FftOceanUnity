using UnityEngine;
using System.Collections;

public class W {
	public const float T = 100f;
	public const float W0 = OceanUtil.TWO_PI / T;

	public int N { get; private set; }
	public K K { get; private set; }

	private float[,] _w;

	public W(K K) {
		this.K = K;
		init();
	}

	public float this[int n, int m] {
		get { return _w[n, m]; }
	}

	public float wDeepwater(Vector2 k) {
		return Mathf.Sqrt(OceanUtil.GRAVITY * k.magnitude);
	}

	private void init() {
		N = K.N;
		_w = new float[N, N];
		for (var y = 0; y < N; y++)
			for (var x = 0; x < N; x++)
				_w[x, y] = calc(x, y);
	}

	private float calc(int n, int m) {
		var k = K[n, m];
		var w = wDeepwater(k);
		return (int)(w / W0) * W0;
	}
}
