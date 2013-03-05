using System;

namespace Biorob.Math.Geometry
{
	public class AxisAngle : Vector4
	{
		public AxisAngle(double x, double y, double z, double a) : base(x, y, z, a)
		{
		}

		public AxisAngle(double[] a) : this(a[0], a[1], a[2], a[3])
		{
		}

		public AxisAngle(AxisAngle a) : base(a)
		{
		}

		public AxisAngle(Vector4 a) : base(a)
		{
		}

		public AxisAngle(Vector3 axis, double angle) : base(axis.X, axis.Y, axis.Z, angle)
		{
		}

		public double Angle
		{
			get { return W; }
		}

		public Vector3 Axis
		{
			get { return new Vector3(d_values[0], d_values[1], d_values[2]); }
		}

		public static Vector3 operator*(AxisAngle me, Vector3 v)
		{
			var c = System.Math.Cos(me.Angle);
			var s = System.Math.Sin(me.Angle);
			var ax = me.Axis;

			return new Vector3(v * c + (ax.Cross(v) * s) + ax * (ax.Dot(v) * (1 - c)));
		}
	}
}

