using System;

namespace Biorob.Math
{
	public class Complex
	{
		private double d_real;
		private double d_imaginary;

		public Complex(double realPart, double imaginaryPart)
		{
			d_real = realPart;
			d_imaginary = imaginaryPart;
		}
		
		public Complex() : this(0, 0)
		{
		}
		
		public Complex(Complex other) : this(other.RealPart, other.ImaginaryPart)
		{
		}
		
		public static Complex FromPolar(double length, double angle)
		{
			return new Complex(length * System.Math.Cos(angle), length * System.Math.Sin(angle));
		}
		
		public double RealPart
		{
			get
			{
				return d_real;
			}
			set
			{
				d_real = value;
			}
		}
		
		public double ImaginaryPart
		{
			get
			{
				return d_imaginary;
			}
			set
			{
				d_imaginary = value;
			}
		}
		
		public static Complex operator-(Complex a)
		{
			return new Complex(a.RealPart, -a.ImaginaryPart);
		}
		
		public static Complex operator+(Complex a, Complex b)
		{
			return new Complex(a.RealPart + b.RealPart, a.ImaginaryPart + b.ImaginaryPart);
		}
		
		public static Complex operator-(Complex a, Complex b)
		{
			return new Complex(a.RealPart - b.RealPart, a.ImaginaryPart - b.ImaginaryPart);
		}
		
		public static Complex operator*(Complex a, Complex b)
		{
			return new Complex(a.RealPart * b.RealPart - a.ImaginaryPart * b.ImaginaryPart,
			                   a.ImaginaryPart * b.RealPart + a.RealPart * b.ImaginaryPart);
		}
		
		public static Complex operator/(Complex a, Complex b)
		{
			double div = b.RealPart * b.RealPart + b.ImaginaryPart * b.ImaginaryPart;
			return new Complex((a.RealPart * b.RealPart + a.ImaginaryPart * b.ImaginaryPart) / div,
			                   (a.ImaginaryPart * b.RealPart - a.RealPart * b.ImaginaryPart) / div);
		}
		
		public Complex Sqrt()
		{
			double r = System.Math.Sqrt(d_real * d_real + d_imaginary * d_imaginary);
			return new Complex(System.Math.Sqrt((r + d_real) / 2), System.Math.Sign(d_imaginary) * System.Math.Sqrt((r - d_real) / 2));
		}
		
		public static Complex Sqrt(double x)
		{
			if (x < 0)
			{
				return new Complex(0, System.Math.Sqrt(-x));
			}
			else
			{
				return new Complex(System.Math.Sqrt(x), 0);
			}
		}
		
		public double Length
		{
			get
			{
				return System.Math.Sqrt(d_real * d_real + d_imaginary * d_imaginary);
			}			
		}
		
		public double Angle
		{
			get
			{
				return System.Math.Atan2(d_imaginary, d_real);
			}
		}
		
		public Complex[] Cbrt()
		{
			double r = Length;
			double phi = Angle;
			
			double nr = System.Math.Pow(r, 1.0 / 3.0);
			
			return new Complex[] {
				Complex.FromPolar(nr, 1.0 / 3.0 * phi),
				Complex.FromPolar(nr, 1.0 / 3.0 * phi + 2.0 / 3.0 * System.Math.PI),
				Complex.FromPolar(nr, 1.0 / 3.0 * phi - 2.0 / 3.0 * System.Math.PI)
			};
		}
		
		public Complex Power(int num)
		{
			if (num == 0)
			{
				return new Complex(1, 0);
			}
			
			Complex ret = new Complex(d_real, d_imaginary);
			
			for (int i = 1; i < num; ++i)
			{
				ret *= ret;
			}
			
			return ret;
		}
		
		public override string ToString()
		{
			 return String.Format("[{0} + i{1}]", d_real, d_imaginary);
		}
	}
}

