using System;
using System.Collections.Generic;

namespace Biorob.Math
{
	public class Range : Changeable
	{
		private double d_min;
		private double d_max;
		
		public Range(double min, double max)
		{
			d_min = min;
			d_max = max;
		}
		
		public Range(Range other) : this(other.Min, other.Max)
		{
		}
		
		public Range() : this(0, 0)
		{
		}
		
		public Range Copy()
		{
			return new Range(d_min, d_max);
		}
		
		public double Max
		{
			get
			{
				return d_max;
			}
			set
			{
				if (d_max != value)
				{
					d_max = value;
					
					EmitChanged();
				}
			}
		}
		
		public double Min
		{
			get
			{
				return d_min;
			}
			set
			{
				if (d_min != value)
				{
					d_min = value;
					
					EmitChanged();
				}
			}
		}
		
		public void Update(Range other)
		{
			Update(other.Min, other.Max);
		}
		
		public void Update(double min, double max)
		{
			if (d_min == min && d_max == max)
			{
				return;
			}
			
			d_min = min;
			d_max = max;
			
			EmitChanged();
		}
		
		public override string ToString()
		{
			return String.Format("[{0}, {1}]", d_min, d_max);
		}
		
		public double Span
		{
			get { return d_max - d_min; }
		}
		
		public bool MarginallyEquals(Range other)
		{
			if (other == null)
			{
				return false;
			}
			
			return System.Math.Abs(d_min - other.d_min) <= double.Epsilon &&
			       System.Math.Abs(d_min - other.d_min) <= double.Epsilon;
		}
		
		public Range Widen(double factor)
		{
			if (Span <= double.Epsilon)
			{
				return new Range(d_min - factor, d_max + factor);
			}
			else
			{
				double dd = Span * factor;

				return new Range(d_min - dd, d_max + dd);
			}
		}
		
		public bool Contains(double x)
		{
			return x >= d_min && x <= d_max;
		}
	}
}

