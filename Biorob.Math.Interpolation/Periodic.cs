using System;
using System.Collections.Generic;

namespace Biorob.Math.Interpolation
{
	public class Periodic
	{
		public static List<Point> Extend(Point[] points, double min, double max)
		{
			List<Point> ret = new List<Point>(points);
			Extend(ret, min, max);
			
			return ret;
		}

		public static void Extend(List<Point> points, double min, double max)
		{
			if (points.Count < 2)
			{
				return;
			}

			double period = max - min;
			int size = points.Count;
			
			Point first = points[0];
			Point second = points[1];
			
			Point last = points[size - 1];
			Point prelast = points[size - 2];
			
			points.Insert(0, new Point(last.X - period, last.Y));
			points.Insert(0, new Point(prelast.X - period, prelast.Y));
			
			points.Add(new Point(first.X + period, first.Y));
			points.Add(new Point(second.X + period, second.Y));
		}
	}
}

