using System;

namespace Biorob.Math.Solvers
{
	public class Linear : Polynomial
	{
		public Linear(double a, double b) : base(a, b)
		{
		}
		
		public override double[] Roots
		{
			get
			{
				return new double[] {-Coefficients[1] / Coefficients[0]};
			}
		}
	}
}

