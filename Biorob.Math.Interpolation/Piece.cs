using System;

namespace Biorob.Math.Interpolation
{
	public class Piece
	{
		public double Start;
		public double[] Coefficients;
		
		public Piece(double start, double[] coefficients)
		{
			Start = start;
			Coefficients = coefficients;
		}
	}
}

