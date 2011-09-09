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
			return Evaluate(d_coefficients, x);
		}
		
		public static double Evaluate(double[] coefficients, double x)
		{
			double ret = 0;
			double mult = 1;
			
			for (int i = coefficients.Length - 1; i >= 0; --i)
			{
				ret += mult * coefficients[i];
				mult *= x;
			}
			
			return ret;
		}
	}
}

