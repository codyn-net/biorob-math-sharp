using System;

namespace Biorob.Math.Geometry
{
	public class Matrix
	{
		protected double[,] d_values;

		public Matrix(int rows, int columns)
		{
			d_values = new double[rows, columns];
		}

		protected void CopyFrom(double[,] values, int rows, int columns)
		{
			for (int r = 0; r < rows; ++r)
			{
				for (int c = 0; c < columns; ++c)
				{
					d_values[r, c] = values[r, c];
				}
			}
		}

		public double this[int row, int column]
		{
			get { return d_values[row, column]; }
		}

		public Matrix(double[,] values)
		{
			int rows = values.GetLength(0);
			int columns = values.GetLength(1);

			d_values = new double[rows, columns];
			CopyFrom(values, rows, columns);
		}
	}

	public class Matrix3x3 : Matrix
	{
		public Matrix3x3() : base(3, 3)
		{
		}

		public Matrix3x3(double[,] values) : base(3, 3)
		{
			if (values.GetLength(0) != 3 || values.GetLength(1) != 3)
			{
				throw new Exception("Only 3x3");
			}

			CopyFrom(values, 3, 3);
		}
	}
}

