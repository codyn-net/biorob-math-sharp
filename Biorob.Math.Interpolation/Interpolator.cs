using System;
using System.Collections.Generic;
using Biorob.Math.Functions;
using Biorob.Math;

namespace Biorob.Math.Interpolation
{
	public abstract class Interpolator
	{
		public PiecewisePolynomial Interpolate(IEnumerable<Point> unsorted)
		{
			List<Point> points = new List<Point>(unsorted);
			points.Sort();
			
			return InterpolateSorted(points);
		}
		
		public PiecewisePolynomial InterpolateSorted(Point[] points)
		{
			return InterpolateSorted(new List<Point>(points));
		}
		
		public abstract PiecewisePolynomial InterpolateSorted(List<Point> points);
	}
}

