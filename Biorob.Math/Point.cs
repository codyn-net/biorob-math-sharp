using System;
using System.Collections.Generic;

namespace Biorob.Math
{
	public class Point : Changeable, IComparable<Point>
	{
		private double d_x;
		private double d_y;
		
		public Point(double x, double y)
		{
			d_x = x;
			d_y = y;
		}
		
		public Point(Point other) : this(other.X, other.Y)
		{
		}
		
		public Point Copy()
		{
			return new Point(d_x, d_y);
		}
		
		public Point() : this(0, 0)
		{
		}
		
		public int CompareTo(Point other)
		{
			return d_x.CompareTo(other.d_x);
		}
		
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			
			Point other = obj as Point;
			
			if (other == null)
			{
				return false;
			}
			
			return d_x == other.d_x && d_y == other.d_y;
		}
		
		public bool MarginallyEquals(Point other)
		{
			if (other == null)
			{
				return false;
			}
			
			return System.Math.Abs(d_x - other.d_x) <= Constants.Epsilon &&
			       System.Math.Abs(d_y - other.d_y) <= Constants.Epsilon;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public double X
		{
			get
			{
				return d_x;
			}
			set
			{
				if (d_x != value)
				{
					d_x = value;
					EmitChanged();
				}
			}
		}
		
		public double Y
		{
			get
			{
				return d_y;
			}
			set
			{
				if (d_y != value)
				{
					d_y = value;
					
					EmitChanged();
				}
			}
		}
		
		public void Shift(Point other)
		{
			d_x += other.d_x;
			d_y += other.d_y;
			
			EmitChanged();
		}
		
		public void Shift(double dx, double dy)
		{
			d_x += dx;
			d_y += dy;
			
			EmitChanged();
		}
		
		public void Update(Point point)
		{
			Update(point.X, point.Y);
		}
		
		public void Update(double x, double y)
		{
			if (d_x == x && d_y == y)
			{
				return;
			}

			d_x = x;
			d_y = y;
			
			EmitChanged();
		}
		
		public void Floor()
		{
			Freeze();

			X = System.Math.Floor(d_x);
			Y = System.Math.Floor(d_y);
			
			Thaw();
		}
		
		public static Point operator-(Point a, Point b)
		{
			return new Point(a.X - b.X, a.Y - b.Y);
		}
		
		public static Point operator+(Point a, Point b)
		{
			return new Point(a.X + b.X, a.Y + b.Y);
		}
		
		public double this[int idx]
		{
			get
			{
				if (idx == 0)
				{
					return d_x;
				}
				else
				{
					return d_y;
				}
			}
			set
			{
				if (idx == 0)
				{
					X = value;
				}
				else
				{
					Y = value;
				}
			}
		}
		
		public override string ToString()
		{
			return String.Format("<{0}, {1}>", d_x, d_y);
		}
	}
}

