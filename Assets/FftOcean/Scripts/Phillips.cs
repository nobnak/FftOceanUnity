using UnityEngine;
using System.Collections;

public class Phillips {
	public K K { get; private set; }

	public float WindSpeed { get; private set; }
	public Vector2 WindDirection { get; private set; }

	public Phillips(K k, float windSpeed, Vector2 windDirection) {
		this.K = k;
		this.WindSpeed = windSpeed;
		this.WindDirection = windDirection;
	}

	public float this[int n, int m] {
		get {
			var k = K[n, m];
			return OceanUtil.PhillipsSpectrum(k, WindSpeed, WindDirection);
		}
	}
}
