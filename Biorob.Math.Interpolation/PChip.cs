using System;
using System.Collections.Generic;

namespace Biorob.Math.Interpolation
{
	public class PChip
	{
		public class Piece : Biorob.Math.Interpolation.Piece
		{
			public Point P0;
			public double M0;
			public Point P1;
			public double M1;

			public Piece(Point p0, Point p1, double m0, double m1)
			{
				P0 = p0;
				M0 = m0;
				P1 = p1;
				M1 = m1;
				
				Start = p0.X;
				End = p1.X;
				
				// O3: 2 * (c1.p - c2.p) + h * (c1.m + c2.m)
				// O2: 3 * (c2.p - c1.p) - h * (2 * c1.m + c2.m)
				// O1: h * c1.m
				// O0: c1.p
				Coefficients = new double[] {
					2 * (p0.Y - p1.Y) + m0 + m1,
					3 * (p1.Y - p0.Y) - 2 * m0 - m1,
					m0,
					p0.Y
				};
			}
		}

		public static List<Piece> Interpolate(IEnumerable<Point> unsorted)
		{
			List<Point> points = new List<Point>(unsorted);
			points.Sort();
			
			return InterpolateSorted(points);
		}
		
		public static List<Piece> InterpolateSorted(Point[] points)
		{
			return InterpolateSorted(new List<Point>(points));
		}
		
		public static List<Piece> InterpolateSorted(List<Point> points)
		{
			// Remove points that are very close together
			Point[] r = points.ToArray();
			
			for (int i = 1; i < r.Length; ++i)
			{
				if (System.Math.Abs(r[i - 1].X - r[i].X) < 10e-5)
				{
					points.Remove(r[i - 1]);
				}
			}
			
			int size = points.Count;
			List<Piece> ret = new List<Piece>();
			
			if (size < 2)
			{
				return ret;
			}

			double[] slopes = new double[size];
			double[] dpdt = new double[size];
			double[] dt = new double[size - 1];

			for (int i = 0; i < size - 1; ++i)
			{
				double dp = (points[i + 1].Y - points[i].Y);
				dt[i] = (points[i + 1].X - points[i].X);

				dpdt[i] = dt[i] == 0 ? 0 : (dp / dt[i]);
			}

			dpdt[size - 1] = 0;

			bool[] samesign = new bool[size - 1];

			for (int i = 0; i < size - 2; ++i)
			{
				samesign[i] = (System.Math.Sign(dpdt[i]) == System.Math.Sign(dpdt[i + 1]) && dpdt[i] != 0);
			}

			// Three point derivative
			for (int i = 0; i < size - 2; ++i)
			{
				double dpdt1 = dpdt[i];
				double dpdt2 = dpdt[i + 1];
		
				if (samesign[i])
				{
					double hs = dt[i] + dt[i + 1];
		
					double w1 = (dt[i] + hs) / (3 * hs);
					double w2 = (hs + dt[i + 1]) / (3 * hs);
		
					double mindpdt;
					double maxdpdt;
		
					if (dpdt1 > dpdt2)
					{
						maxdpdt = dpdt1;
						mindpdt = dpdt2;
					}
					else
					{
						maxdpdt = dpdt2;
						mindpdt = dpdt1;
					}
		
					slopes[i + 1] = mindpdt / (w1 * (dpdt[i] / maxdpdt) + w2 * (dpdt[i + 1] / maxdpdt));
				}
				else
				{
					slopes[i + 1] = 0;
				}
			}

			slopes[0] = ((2 * dt[0] + dt[1]) * dpdt[0] - dt[0] * dpdt[1]) / (dt[0] + dt[1]);

			if (System.Math.Sign(slopes[0]) != System.Math.Sign(dpdt[0]))
			{
				slopes[0] = 0;
			}
			else if (System.Math.Sign(dpdt[0]) != System.Math.Sign(dpdt[1]) &&
			         System.Math.Abs(slopes[0]) > System.Math.Abs(3 * dpdt[0]))
			{
				slopes[0] = 3 * dpdt[0];
			}

			slopes[size - 1] = ((2 * dt[size - 2] + dt[size - 3]) * dpdt[size - 2] -
			                    dt[size - 2] * dpdt[size - 3]) /
			                   (dt[size - 2] + dt[size - 3]);

			if (System.Math.Sign(slopes[size - 1]) != System.Math.Sign(dpdt[size - 2]))
			{
				slopes[size - 1] = 0;
			}
			else if (System.Math.Sign(dpdt[size - 2]) != System.Math.Sign(dpdt[size - 3]) &&
			         System.Math.Abs(slopes[size - 1]) > System.Math.Abs(3 * dpdt[size - 2]))
			{
				slopes[size - 1] = 3 * dpdt[size - 2];
			}
			
			for (int i = 0; i < size - 1; ++i)
			{
				double h = points[i + 1].X - points[i].X;
				
				ret.Add(new Piece(points[i], points[i + 1], h * slopes[i], h * slopes[i + 1]));
			}
			
			return ret;
		}
	}
}

