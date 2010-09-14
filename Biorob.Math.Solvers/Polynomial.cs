using System;

namespace Biorob.Math.Solvers
{
	public class Polynomial
	{
		private double[] d_coefficients;

		public Polynomial(double[] coefficients)
		{
			d_coefficients = coefficients;
		}
		
		public double[] Coefficients
		{
			get
			{
				return d_coefficients;
			}
		}
		
		public double Evaluate(double x)
		{
			double power = 1;
			double ret = 0;
			
			for (int i = d_coefficients.Length - 1; i >= 0; ++i)
			{
				ret += d_coefficients[i] * power;
				power *= x;
			}
			
			return ret;
		}
	}
}

