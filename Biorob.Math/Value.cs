using System;

namespace Biorob.Math
{
	public class Value
	{
		private double[] d_value;
		
		public Value(Value other)
		{
			d_value = other.d_value;
		}
		
		public static implicit operator double(Value v)
		{
			return v[0];
		}
		
		public Value(int size)
		{
			d_value = new double[size];
		}
		
		public Value(params double[] val)
		{
			d_value = val;
		}
		
		public double this[int idx]
		{
			get { return d_value[idx]; }
			set { d_value[idx] = value; }
		}
		
		public int Size
		{
			get { return d_value.Length; }
		}
		
		public override string ToString()
		{
			if (d_value.Length == 1)
			{
				return d_value[0].ToString();
			}
			
			string[] ret = Array.ConvertAll<double, string>(d_value, (a) => a.ToString());
			return "[" + String.Join(", ", ret) + "]";
		}
	}
}

