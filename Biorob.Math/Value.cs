using System;

namespace Biorob.Math
{
	public class Value : IConvertible
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
		
		public bool ToBoolean (IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
		
		public char ToChar (IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
		
		public DateTime ToDateTime (IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
		
		public decimal ToDecimal (IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
		
		public byte ToByte (IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
		
		public double ToDouble (IFormatProvider provider)
		{
			return d_value[0];
		}
		
		public short ToInt16 (IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
		
		public int ToInt32 (IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
		
		public long ToInt64 (IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
		
		public sbyte ToSByte (IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
		
		public float ToSingle (IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
		
		public string ToString (IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
		
		public object ToType (Type conversionType, IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
		
		public ushort ToUInt16 (IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
		
		public uint ToUInt32 (IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
		
		public ulong ToUInt64 (IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
		
		public TypeCode GetTypeCode()
		{
			return TypeCode.Double;
		}
	}
}

