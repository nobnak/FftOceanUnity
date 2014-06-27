using UnityEngine;
using System.Collections;

public static class ColorUtil {
	public static readonly Vector4 kEncodeMul = new Vector4(1.0f, 255.0f, 65025.0f, 16581375.0f);
	public const float kEncodeBit = 1.0f / 255.0f;

	public static Color EncodeFloatRGBA(float v) {
		Vector4 enc = kEncodeMul * v;
		enc = new Vector4(enc.x - Mathf.Floor(enc.x), enc.y - Mathf.Floor(enc.y), enc.z - Mathf.Floor(enc.z), enc.w - Mathf.Floor(enc.w));
		enc -= new Vector4(enc.y, enc.z, enc.w, enc.w) * kEncodeBit;
		return enc;
	}
}
