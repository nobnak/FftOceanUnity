using UnityEngine;
using System.Collections;
using FFTWSharp;
using System.Runtime.InteropServices;

public class FFT {
	public int N { get; private set; }

	private ComplexArray _fftIn, _fftOut;
	private fftwf_plan _fftPlan;
	private float[] _spaceDomain;

	public FFT(int n) {
		this.N = n;

		var size = n * n;
		_fftIn = new ComplexArray(size);
		_fftOut = new ComplexArray(size);
		_fftPlan = fftwf_plan.dft_2d(n, n, _fftIn, _fftOut, fftw_direction.Backward, fftw_flags.Estimate);
		_spaceDomain = new float[2 * size];
	}

	public void Execute(float[] freqDomain, ref float maxHeight, out float scale, float[] spaceDomain) {
		_fftIn.SetData(freqDomain);
		_fftPlan.Execute();
		_fftOut.GetData(_spaceDomain);
		
		var fftScale = 1f / Mathf.Sqrt(N);
		for (var y = 0; y < N; y++) {
			for (var x = 0; x < N; x++) {
				var i = x + y * N;
				var hx = fftScale * _spaceDomain[2 * i];
				spaceDomain[i] = hx;
				if (hx > maxHeight)
					maxHeight = hx;
				else if (-hx > maxHeight)
					maxHeight = -hx;
			}
		}
		scale = 2.02f * maxHeight;
	}
	
	public class ComplexArray : fftwf_complexarray {
		
		public ComplexArray(int size) : base(size) {}
		
		public void GetData(float[] data) {
			Marshal.Copy(Handle, data, 0, data.Length);
		}
	}
}
