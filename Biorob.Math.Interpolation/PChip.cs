using System;
using System.Collections.Generic;

namespace Biorob.Math.Interpolation
{
	public class PChip
	{
		public static List<Piece> Interpolate(List<Point> points, double min, double max)
		{
			points.Sort();
			
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
				if (size == 1)
				{
					ret.Add(new Piece(0, new double[] {0, 0, 0, points[0].Y}));
					ret.Add(new Piece(1, new double[] {0, 0, 0, points[0].Y}));
				}
				
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
		
				// O3: 2 * (c1.p - c2.p) + h * (c1.m + c2.m)
				// O2: 3 * (c2.p - c1.p) - h * (2 * c1.m + c2.m)
				// O1: h * c1.m
				// O0: c1.p
				ret.Add(new Piece(points[i].X, new double[] {
					2 * (points[i].Y - points[i + 1].Y) + h * (slopes[i] + slopes[i + 1]),
					3 * (points[i + 1].Y - points[i].Y) - h * (2 * slopes[i] + slopes[i + 1]),
					h * slopes[i],
					points[i].Y
				}));
			}
			
			ret.Add(new Piece(points[size - 1].X, new double[] {0, 0, 0, points[size - 1].Y}));
			return ret;
		}
	}
}

