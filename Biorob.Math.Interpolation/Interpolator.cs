using System;
using System.Collections.Generic;

namespace Biorob.Math.Interpolation
{
	public abstract class Interpolator
	{
		private List<Piece> d_pieces;
		
		public Interpolator()
		{
			d_pieces = new List<Piece>();
		}
		
		public IEnumerable<Piece> Pieces
		{
			get
			{
				return d_pieces;
			}
		}
		
		public int Count
		{
			get
			{
				return d_pieces.Count;
			}
		}

		public List<Piece> Interpolate(IEnumerable<Point> unsorted)
		{
			List<Point> points = new List<Point>(unsorted);
			points.Sort();
			
			return InterpolateSorted(points);
		}
		
		public List<Piece> InterpolateSorted(Point[] points)
		{
			return InterpolateSorted(new List<Point>(points));
		}
		
		public abstract List<Piece> InterpolateSorted(List<Point> points);
	}
}

