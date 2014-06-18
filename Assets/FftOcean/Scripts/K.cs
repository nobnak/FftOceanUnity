using UnityEngine;
using System.Collections;

public class K {
	public int N { get; private set; }
	public float L { get; private set; }

	private int _halfN;
	private float _twopiOverL;

	public K(int N, float L) {
		this.N = N;
		this.L = L;

		this._halfN = N >> 1;
		this._twopiOverL = 2f * Mathf.PI / L;
	}

	public Vector2 this[int n, int m]{
		get {
			n = (n + _halfN) % N - _halfN;
			m = (m + _halfN) % N - _halfN;
			return new Vector2(n * _twopiOverL, m * _twopiOverL);
		}
	}
}
