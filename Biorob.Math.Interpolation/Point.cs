using System;
using System.Collections.Generic;

namespace Biorob.Math.Interpolation
{
	public class Point : IComparable<Point>
	{
		public double X;
		public double Y;
		public bool ZeroMean;

		public Point(double x, double y, bool zeroMean)
		{
			X = x;
			Y = y;
			ZeroMean = zeroMean;
		}
		
		public Point(double x, double y) : this(x, y, false)
		{
		}
		
		public int CompareTo(Point other)
		{
			return X.CompareTo(other.X);
		}
	}
}

