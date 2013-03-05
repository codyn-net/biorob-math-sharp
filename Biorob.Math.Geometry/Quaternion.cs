using System;

namespace Biorob.Math.Geometry
{
	public class Quaternion : Vector4
	{
		public Quaternion() : base()
		{
		}

		public Quaternion(Quaternion other) : base(other)
		{
		}

		public Quaternion(double x, double y, double z, double w) : base(x, y, z, w)
		{
		}
		
		public static Quaternion FromRotationMatrix(Matrix3x3 m)
		{
			// Find even permutation uvw of xyz such that colU[u] is the largest trace element
			int u = 0, v = 1, w = 2;

			if (m[1, 1] > m[0, 0])
			{
				u = 1;
				v = 2;
				w = 0;
			}
			if (m[2, 2] > m[1, 1])
			{
				u = 2;
				v = 0;
				w = 1;
			}

			double r = System.Math.Sqrt(1 + m[u, u] - m[v, v] - m[w, w]);
			
			if (r == 0)
			{
				return new Quaternion(0, 0, 0, 1);
			}

			Quaternion ret = new Quaternion();
			
			ret.W = (m[w, v] - m[v, w]) / (2 * r);
			ret[u] = 0.5 * r;
			ret[v] = (m[u, v] + m[v, u]) / (2 * r);
			ret[w] = (m[w, u] + m[u, w]) / (2 * r);

			return ret;
		}

		public static Quaternion FromAxisAngle(AxisAngle aa)
		{
			double angle = aa.Angle * 0.5;
			double sinAngle = System.Math.Sin(angle);

			var xyz = aa.Axis * sinAngle;
			var w = System.Math.Cos(angle);
			
			return new Quaternion(xyz.X, xyz.Y, xyz.Z, w);
		}

		public AxisAngle doubleoAxis()
		{
			double angle = 2 * System.Math.Acos(W);
			double s = System.Math.Sqrt(1 - W * W);

			if (s < 0.00001)
			{
				return new AxisAngle(1, 0, 0, 0);
			}
			else
			{
				return new AxisAngle(X / s, Y / s, Z / s, angle);
			}
		}
		
		public Quaternion Inverse()
		{
			return new Quaternion(-X, -Y, -Z, W);
		}
		
		public Quaternion Invert()
		{
			X = -X;
			Y = -Y;
			Z = -Z;

			return this;
		}
		
		public Quaternion Normalized()
		{
			Quaternion ret = new Quaternion(this);
			return ret.Normalize();
		}
		
		public Quaternion Normalize()
		{
			double mag = W * W + X * X + Y * Y + Z * Z;
			
			if (mag > 1.0001 || mag < 0.9999)
			{
				mag = System.Math.Sqrt(mag);

				X /= mag;
				Y /= mag;
				Z /= mag;
				W /= mag;
			}
			
			return this;
		}
		
		public Vector3 Side
		{
			get
			{
				return new Vector3(1 - 2 * (Y * Y + Z * Z),
				                      2 * (X * Y - W * Z),
				                      2 * (X * Z + W * Y));
			}
		}
		
		public Vector3 Up
		{
			get
			{
				return new Vector3(2 * (X * Y + W * Z),
				                      1 - 2 * (X * X + Z * Z),
				                      2 * (Y * Z - W * X));
			}
		}
		
		public Vector3 Forward
		{
			get
			{
				return new Vector3(2 * (X * Z - W * Y),
				                      2 * (Y * Z + W * X),
				                      1 - 2 * (X * X + Y * Y));
			}
		}
		
		public Vector3 Rotate(Vector3 v)
		{
			double t2 = W * -X;
			double t3 = W * -Y;
			double t4 = W * -Z;
	
			double t5 = -X * X;
			double t6 = X * Y;
			double t7 = X * Z;
	
			double t8 = -Y * Y;
			double t9 = Y * Z;
			double t10 = -Z * Z;
	        
			return new Vector3(2 * (( t8 + t10) * v[0] + ( t6 +  t4) * v[1] + (-t3 + t7) * v[2]) + v[0],
                                  2 * ((-t4 +  t6) * v[0] + ( t5 + t10) * v[1] + ( t9 + t2) * v[2]) + v[1],
                                  2 * (( t7 +  t3) * v[0] + (-t2 +  t9) * v[1] + ( t5 + t8) * v[2]) + v[2]);
		}
		
		public static Quaternion operator*(Quaternion first, Quaternion second)
		{
			return new Quaternion(first.W * second.X + first.X * second.W + first.Y * second.Z - first.Z * second.Y,
			                      first.W * second.Y - first.X * second.Z + first.Y * second.W + first.Z * second.X,
			                      first.W * second.Z + first.X * second.Y - first.Y * second.X + first.Z * second.W,
			                      first.W * second.W - first.X * second.X - first.Y * second.Y - first.Z * second.Z);
		}
		
		public static Vector3 operator*(Quaternion first, Vector3 vec)
		{
			return first.Rotate(vec);
		}
	}
}
