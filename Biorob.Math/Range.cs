using System;
using System.Collections.Generic;

namespace Biorob.Math
{
	public class Range : Changeable
	{
		private double d_min;
		private double d_max;
		private bool d_empty;
		
		public delegate void ObjectHandler(object source);
		
		public event ObjectHandler Resized = delegate {};
		public event ObjectHandler Shifted = delegate {};
		
		public Range(double min, double max)
		{
			d_min = min;
			d_max = max;
			
			d_empty = false;
		}
		
		public Range(Range other) : this(other.Min, other.Max)
		{
		}
		
		public Range() : this(0, 0)
		{
			d_empty = true;
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
				if (d_max != value || d_empty)
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
				if (d_min != value || d_empty)
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
			if (!d_empty && d_min == min && d_max == max)
			{
				return;
			}
			
			bool resized = System.Math.Abs((max - min) - Span) <= double.Epsilon;
			
			d_min = min;
			d_max = max;
			
			EmitChanged();
			
			if (resized)
			{
				Resized(this);
			}
			else
			{
				Shifted(this);
			}
		}
		
		public override void EmitChanged ()
		{
			d_empty = false;
			base.EmitChanged();
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
			
			return System.Math.Abs(d_min - other.d_min) <= Constants.Epsilon &&
			       System.Math.Abs(d_min - other.d_min) <= Constants.Epsilon;
		}
		
		public Range Widen(double factor)
		{
			if (Span <= Constants.Epsilon)
			{
				return new Range(d_min - factor, d_max + factor);
			}
			else
			{
				double dd = Span * factor;

				return new Range(d_min - dd, d_max + dd);
			}
		}
		
		public void ExpandMax(Range other)
		{
			Freeze();
			
			ExpandMax(other.Min);
			ExpandMax(other.Max);
			
			Thaw();
		}
		
		public double Normalize(double x)
		{
			return (x - d_min) / Span;
		}
		
		public void ExpandMax(double val)
		{
			Freeze();

			if (d_empty || val < d_min)
			{
				d_min = val;
				EmitChanged();
			}
			
			if (d_empty || val > d_max)
			{
				d_max = val;
				EmitChanged();
			}
			
			Thaw();
		}
		
		public void Expand(double size)
		{
			d_min -= size / 2;
			d_max += size / 2;
			
			EmitChanged();
			Resized(this);
		}
		
		public bool Contains(double x)
		{
			return x >= d_min && x <= d_max;
		}
		
		public void Shift(double amount)
		{
			d_min += amount;
			d_max += amount;
			
			EmitChanged();
			Shifted(this);
		}
	}
}

