using System;

namespace Biorob.Math.Geometry
{
	public class Vector
	{
		protected double[] d_values;

		public Vector(Vector other) : this(other.d_values)
		{
		}

		public Vector(int size)
		{
			d_values = new double[size];
		}

		public Vector(int size, double defval) : this(size)
		{
			for (int i = 0; i < size; ++i)
			{
				d_values[i] = defval;
			}
		}

		public Vector(params double[] vals) : this(vals.Length)
		{
			vals.CopyTo(d_values, 0);
		}

		public double this[int i]
		{
			get { return d_values[i]; }
			set { d_values[i] = value; }
		}

		public int Size
		{
			get { return d_values.Length; }
		}

		public Vector3 Cross(Vector b)
		{
			if (Size != 3 || b.Size != 3)
			{
				throw new Exception("Cross product is only allowed on vectors of size 3");
			}

			return new Vector3(d_values[1] * b.d_values[2] - d_values[2] * b.d_values[1],
			                   d_values[2] * b.d_values[0] - d_values[0] * b.d_values[2],
			                   d_values[0] * b.d_values[1] - d_values[1] * b.d_values[0]);
		}

		public static Vector operator*(Vector me, double v)
		{
			Vector ret = new Vector(me.Size);

			for (int i = 0; i < me.Size; ++i)
			{
				ret.d_values[i] = me.d_values[i] * v;
			}

			return ret;
		}

		public double[] ToArray()
		{
			double[] ret = new double[d_values.Length];

			d_values.CopyTo(ret, 0);
			return ret;
		}

		public double Dot(Vector other)
		{
			if (Size != other.Size)
			{
				throw new Exception("Dot product vectors not of same size");
			}

			double ret = 0;

			for (int i = 0; i < Size; ++i)
			{
				ret += d_values[i] * other.d_values[i];
			}

			return ret;
		}

		public static Vector operator+(Vector a, Vector b)
		{
			if (a.Size != b.Size)
			{
				throw new Exception("Dot product vectors not of same size");
			}

			Vector ret = new Vector(a.Size);

			for (int i = 0; i < a.Size; ++i)
			{
				ret.d_values[i] = a.d_values[i] + b.d_values[i];
			}

			return ret;
		}
	}

	public class Vector3 : Vector
	{
		public Vector3() : base(3)
		{
		}

		public Vector3(Vector other) : base(other)
		{
		}

		public Vector3(double x, double y, double z) : base(x, y, z)
		{
		}

		public Vector3(double[] xyz) : base(xyz[0], xyz[1], xyz[2])
		{
		}

		protected Vector3(int size) : base(size)
		{
		}

		public double X
		{
			get { return d_values[0]; }
			set { d_values[0] = value; }
		}

		public double Y
		{
			get { return d_values[1]; }
			set { d_values[1] = value; }
		}

		public double Z
		{
			get { return d_values[2]; }
			set { d_values[2] = value; }
		}

		public static Vector3 operator*(Vector3 me, double v)
		{
			return new Vector3(me.X * v, me.Y * v, me.Z * v);
		}
	}

	public class Vector4 : Vector3
	{
		public Vector4() : base(4)
		{
		}

		public Vector4(Vector4 other) : base(other.d_values)
		{
		}

		public Vector4(double x, double y, double z, double w) : base(4)
		{
			d_values[0] = x;
			d_values[1] = y;
			d_values[2] = z;
			d_values[3] = w;
		}

		public Vector4(double[] xyz) : base(xyz[0], xyz[1], xyz[2])
		{
		}

		public double W
		{
			get { return d_values[3]; }
			set { d_values[3] = value; }
		}
	}
}

