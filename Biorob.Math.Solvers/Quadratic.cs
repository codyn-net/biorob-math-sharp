using System;

namespace Biorob.Math.Solvers
{
	public class Quadratic : Polynomial
	{
		private double d_discriminant;
		private double[] d_roots;
		
		public Quadratic(double a, double b, double c) : base(a, b, c)
		{
			d_discriminant = b * b - 4 * a * c;
			d_roots = null;
		}
		
		public double Discriminant
		{
			get
			{
					return d_discriminant;
			}
		}
		
		public bool Complex
		{
			get
			{
				return d_discriminant < 0;
			}
		}
		
		public override double[] Roots
		{
			get
			{
				if (d_roots == null)
				{
					d_roots = CalculateRoots();
				}
				
				return d_roots;
			}
		}
		
		private double[] CalculateRoots()
		{
			double[] c = Coefficients;
			
			if (System.Math.Abs(c[0]) < Constants.Epsilon)
			{
				if (System.Math.Abs(c[1]) < Constants.Epsilon)
				{
					return new double[] {};
				}
				else
				{
					return new double[] {-c[2] / c[1]};
				}
			}
			
			if (System.Math.Abs(d_discriminant) < Constants.Epsilon)
			{
				return new double[] {-c[1] / (2 * c[0])};
			}
			else if (d_discriminant > 0)
			{
				double s = System.Math.Sqrt(c[1] * c[1] - 4 * c[0] * c[2]);
				return new double[] {(-c[1] + s) / (2 * c[0]), (-c[1] - s) / (2 * c[0])};
			}
			else
			{
				return new double[] {};
			}
		}
		
		public Complex[] ComplexRoots
		{
			get
			{
				if (!Complex)
				{
					return new Complex[] {};
				}
				
				double[] c = Coefficients;

				double s = System.Math.Sqrt(4 * c[0] * c[2] - c[1] * c[1]);
				double a2 = 2 * c[0];

				double realPart = -c[1] / a2;
				double imaginaryPart = s / a2;
				
				return new Complex[] {new Complex(realPart, imaginaryPart), new Complex(realPart, -imaginaryPart)};
			}
		}
	}
}

