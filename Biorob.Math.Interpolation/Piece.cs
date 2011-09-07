using System;

namespace Biorob.Math.Interpolation
{
	public class Piece
	{
		public double Start;
		public double End;

		public double[] Coefficients;
		
		public Piece() : this(0, 1, new double[] {0, 0, 0, 0})
		{
		}
		
		public Piece(double start, double end, double[] coefficients)
		{
			Start = start;
			End = end;

			Coefficients = coefficients;
		}
		
		public double Evaluate(double x)
		{
			double ret = 0;
			double mult = 1;
			
			x = (x - Start) / (End - Start);

			for (int i = Coefficients.Length - 1; i >= 0; --i)
			{
				ret += mult * Coefficients[i];
				mult *= x;
			}
			
			return ret;
		}
	}
}

