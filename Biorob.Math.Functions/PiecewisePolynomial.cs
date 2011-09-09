using System;
using System.Collections.Generic;

namespace Biorob.Math.Functions
{
	public class PiecewisePolynomial : IEnumerable<PiecewisePolynomial.Piece>
	{
		public class Piece
		{
			private Range d_range;
			private double[] d_coefficients;
			
			public Piece() : this(new Range(0, 1), new double[] {0, 0, 0, 0})
			{
			}
			
			public Piece(Range range, double[] coefficients)
			{
				d_range = range;
				d_coefficients = coefficients;
			}
			
			public Range Range
			{
				get { return d_range; }
				set { d_range = value; }
			}
			
			public double Begin
			{
				get { return d_range.Min; }
				set { d_range.Min = value; }
			}
			
			public double End
			{
				get { return d_range.Max; }
				set { d_range.Max = value; }
			}
			
			public double[] Coefficients
			{
				get { return d_coefficients; }
				set { d_coefficients = value; }
			}
			
			public static implicit operator double[] (Piece piece)
			{
				return piece.d_coefficients;
			}
			
			public int Degree
			{
				get
				{
					if (d_coefficients == null || d_coefficients.Length == 0)
					{
						return 0;
					}
					else
					{
						return d_coefficients.Length - 1;
					}
				}
			}
			
			public double Evaluate(double x)
			{
				return Solvers.Polynomial.Evaluate(d_coefficients, d_range.Normalize(x));
			}
		}
		
		private List<Piece> d_pieces;
		private Range d_yrange;
		
		public PiecewisePolynomial() : this(new Piece[] {})
		{
		}

		public PiecewisePolynomial(IEnumerable<Piece> pieces)
		{
			d_pieces = new List<Piece>(pieces);
		}
		
		public int Count
		{
			get { return d_pieces.Count; }
		}
		
		public IEnumerable<Piece> Pieces
		{
			get { return d_pieces; }
		}
		
		public Piece this[int idx]
		{
			get { return d_pieces[idx]; }
		}
		
		public Piece PieceAt(double x)
		{
			int idx = 0;
			
			if (NextPieceAt(ref idx, x))
			{
				return d_pieces[idx];
			}
			
			return null;
		}
		
		public double Evaluate(double x)
		{
			// Find piece
			Piece piece = PieceAt(x);
			
			if (piece != null)
			{
				return piece.Evaluate(x);
			}
			else
			{
				return double.NaN;
			}
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return d_pieces.GetEnumerator();
		}
		
		public IEnumerator<PiecewisePolynomial.Piece> GetEnumerator()
		{
			return d_pieces.GetEnumerator();
		}
		
		private bool NextPieceAt(ref int pidx, double x)
		{
			for (int i = pidx; i < d_pieces.Count; ++i)
			{
				if (d_pieces[i].Range.Contains(x))
				{
					pidx = i;
					return true;
				}
			}
			
			return false;
		}
		
		public Range XRange
		{
			get
			{
				if (d_pieces == null || d_pieces.Count == 0)
				{
					return new Range();
				}

				return new Range(d_pieces[0].Begin, d_pieces[d_pieces.Count - 1].End);
			}
		}
		
		public Range YRange
		{
			get
			{
				if (d_yrange == null)
				{
					d_yrange = new Range();
					
					Range r = new Range(0, 1);

					foreach (Piece piece in d_pieces)
					{
						Solvers.Polynomial poly = Solvers.Polynomial.Create(piece.Coefficients);
						d_yrange.ExpandMax(poly.Range(r));
					}
				}
				
				return d_yrange;
			}
		}

		public List<Point> Evaluate(Range xrange, int samples)
		{
			List<Point> ret = new List<Point>();
			
			if (samples == 1)
			{
				ret.Add(new Point(xrange.Min, Evaluate(xrange.Min)));
			}
			
			if (samples <= 1)
			{
				return ret;
			}
			
			// Find the first piece matching xrange.Min
			int pidx = 0;
			
			while (pidx < d_pieces.Count && !d_pieces[pidx].Range.Contains(xrange.Min))
			{
				++pidx;
			}
			
			if (pidx == d_pieces.Count)
			{
				// Outside of the range
				return ret;
			}
			
			double spend = xrange.Min;

			for (int i = 0; i < samples; ++i)
			{
				if (!NextPieceAt(ref pidx, spend))
				{
					break;
				}
				
				ret.Add(new Point(spend, d_pieces[pidx].Evaluate(spend)));

				int num = samples - i - 1;
				
				if (num > 0)
				{
					spend += (xrange.Max - spend) / num;
				}
			}
			
			return ret;
		}
	}
}

