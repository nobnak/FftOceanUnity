using UnityEngine;
using System.Collections;

public class H {
	public int N { get; private set; }
	public H0 H0 { get; private set; }
	public W W { get; private set; }

	private Vector2[,] _h;

	public H(H0 H0, W W) {
		this.H0 = H0;
		this.W = W;

		N = H0.N;
		this._h = new Vector2[N, N];
	}

	public void Jump(float t) {
		for (var y = 0; y < N; y++) {
			for (var x = 0; x < N; x++) {
				var theta = W[x, y] * t;
				var c = Mathf.Cos(theta);
				var s = Mathf.Sin(theta);
				var hp = H0[x, y];
				var hm = H0.ConjMinusK(x, y);

				_h[x, y] = new Vector2(
					(hp.x + hm.x) * c + (hm.y - hp.y) * s,
					(hp.y + hm.y) * c + (hp.x - hm.x) * s);
			}
		}
	}
}
