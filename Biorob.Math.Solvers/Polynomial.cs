using System;

namespace Biorob.Math.Solvers
{
	public class Polynomial
	{
		private double[] d_coefficients;

		protected Polynomial(params double[] coefficients)
		{
			d_coefficients = coefficients;
		}
		
		public static Polynomial Create(params double[] coefficients)
		{
			switch (coefficients.Length)
			{
				case 2:
					return new Linear(coefficients[0], coefficients[1]);
				case 3:
					return new Quadratic(coefficients[0], coefficients[1], coefficients[2]);
				case 4:
					return new Cubic(coefficients[0], coefficients[1], coefficients[2], coefficients[3]);
				default:
					return new Polynomial(coefficients);
			}
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

