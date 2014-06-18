using UnityEngine;
using System.Collections;

public class OceanUtil {
	public const float TWO_PI = 2f * Mathf.PI;
	public const float GRAVITY = 9.81f;

	public static float PhillipsSpectrum(Vector2 k, float windSpeed, Vector2 windDirection) {
		var k2mag = k.sqrMagnitude;
		if (k2mag < 1e-12f)
			return 0f;
		
		var k4mag = k2mag * k2mag;
		var k6mag = k4mag * k2mag;
		var L = windSpeed * windSpeed / GRAVITY;
		var kDotW = Vector2.Dot(k, windDirection);
		var kDotW2 = kDotW * kDotW * kDotW * kDotW * kDotW * kDotW / k6mag;
		
		float damping   = 0.001f;
		float l2        = L * L * damping * damping;
		
		return Mathf.Exp(-1f / (k2mag * L * L)) * kDotW2 / k4mag * Mathf.Exp(- k2mag * l2);
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
