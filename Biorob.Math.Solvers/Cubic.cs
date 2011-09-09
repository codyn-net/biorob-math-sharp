using System;
using System.Collections.Generic;

namespace Biorob.Math.Solvers
{
	public class Cubic : Polynomial
	{
		private double[] d_roots;

		public Cubic(double a, double b, double c, double d) : base(a, b, c, d)
		{
			if (System.Math.Abs(a) < Constants.Epsilon)
			{
				// This is not really cubic, solve quadratic case
				Quadratic q = new Quadratic(b, c, d);
				
				if (q.Complex)
				{
					d_roots = new double[] {};
				}
				else
				{
					d_roots = q.Roots;
				}
				
				return;
			}

			double f = ((3 * c / a) - (b * b / (a * a))) / 3;
			double g = ((2 * b * b * b / (a * a * a)) - (9 * b * c / (a * a)) + (27 * d / a)) / 27;
			double h = (g * g / 4) + (f * f * f / 27);
			
			if (h > 0)
			{
				double r = -(g / 2) + System.Math.Sqrt(h);
				double s = System.Math.Sign(r) * System.Math.Pow(System.Math.Abs(r), 1.0 / 3.0);
				double t = -(g / 2) - System.Math.Sqrt(h);
				double u = System.Math.Sign(t) * System.Math.Pow(System.Math.Abs(t), 1.0 / 3.0);
				
				d_roots = new double[] {
					(s + u) - (b / (3 * a))
				};
			}
			else if (f == 0 && g == 0 && h == 0)
			{
				d_roots = new double[] {
					-System.Math.Pow(d / a, 1.0 / 3.0)
				};
			}
			else
			{
				double i = System.Math.Sqrt(g * g / 4 - h);
				double j = System.Math.Sign(i) * System.Math.Pow(System.Math.Abs(i), 1.0 / 3.0);
				double k = System.Math.Acos(-(g / (2 * i)));
				double l = -j;
				double m = System.Math.Cos(k / 3);
				double n = System.Math.Sqrt(3) * System.Math.Sin(k / 3);
				double p = -(b / (3 * a));

				d_roots = new double[] {
					2 * j * System.Math.Cos(k / 3) - (b / (3 * a)),
					l * (m + n) + p,
					l * (m - n) + p
				};
			}
		}
		
		public override double[] Roots
		{
			get { return d_roots; }
		}
	}
}

