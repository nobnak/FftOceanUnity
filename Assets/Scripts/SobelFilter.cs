using UnityEngine;
using System.Collections;

public class SobelFilter {
	public const int STRIDE = 2;

	public static Vector3[] Normal(Vector3[] normals, float[] heights, int N, float length) {
		var dx = length / N;
		var invDx = 1f / (8f * dx);
		var nPlus1 = N + 1;

		for (var y = 0; y < nPlus1; y++) {
			for (var x = 0; x < nPlus1; x++) {
				var nindex = y * nPlus1 + x;
				var s = SobelModulo(x, y, N, heights);
				var n = normals[nindex];
				n.x = -s.x * invDx;
				n.y = 1f;
				n.z = -s.y * invDx;
				n.Normalize();
				normals[nindex] = n;
			}
		}
		return normals;
	}

	public static Vector2 Sobel(int x, int y, int N, float[] heights) {
		var h20 = heights[STRIDE * ((y+1)*N+(x-1))]; var h21 = heights[STRIDE * ((y+1)*N+x)]; var h22 = heights[STRIDE * ((y+1)*N+(x+1))];
		var h10 = heights[STRIDE * (    y*N+(x-1))];                                          var h12 = heights[STRIDE * (    y*N+(x+1))];
		var h00 = heights[STRIDE * ((y-1)*N+(x-1))]; var h01 = heights[STRIDE * ((y-1)*N+x)]; var h02 = heights[STRIDE * ((y-1)*N+(x+1))];
		var gx = h22 + 2 * h12 + h02 - (h20 + 2 * h10 + h00);
		var gy = h20 + 2 * h21 + h22 - (h00 + 2 * h01 + h02);
		return new Vector2(gx, gy);
	}

	public static Vector2 SobelModulo(int x, int y, int N, float[] heights) {
		var h20 = heights[STRIDE * (  ((y+1)%N)*N + (x+N-1)%N)]; 
		var h21 = heights[STRIDE * (  ((y+1)%N)*N +       x%N)];     
		var h22 = heights[STRIDE * (  ((y+1)%N)*N +   (x+1)%N)];
		var h10 = heights[STRIDE * (      (y%N)*N + (x+N-1)%N)];                                                          
		var h12 = heights[STRIDE * (      (y%N)*N +   (x+1)%N)];
		var h00 = heights[STRIDE * (((y+N-1)%N)*N + (x+N-1)%N)]; 
		var h01 = heights[STRIDE * (((y+N-1)%N)*N +       x%N)]; 
		var h02 = heights[STRIDE * (((y+N-1)%N)*N +   (x+1)%N)];
		var gx = h22 + 2 * h12 + h02 - (h20 + 2 * h10 + h00);
		var gy = h20 + 2 * h21 + h22 - (h00 + 2 * h01 + h02);
		return new Vector2(gx, gy);
	}
}
