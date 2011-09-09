using System;
using Biorob.Math;
using System.Collections.Generic;

namespace Biorob.Math.Functions
{
	public class Bezier : IEnumerable<Bezier.Piece>
	{
		public class Piece
		{
			private Point d_begin;
			private Point d_end;
			private Point d_c1;
			private Point d_c2;

			public Piece(Point begin, Point c1, Point c2, Point end)
			{
				d_begin = begin;
				d_end = end;
				d_c1 = c1;
				d_c2 = c2;
			}
			
			public Piece(PiecewisePolynomial.Piece piece)
			{
				if (piece.Degree != 3)
				{
					throw new Exception(String.Format("Piece is not of 3rd degree (degree is in fact {0})", piece.Degree));
				}
				
				double dx = piece.End - piece.Begin;

				// Convert from polynomial form to bezier curve form			
				d_begin = new Point(piece.Begin,
				                    piece.Coefficients[3]);
				
				d_c1 = new Point(piece.Begin + dx / 3,
				                 piece.Coefficients[3] + piece.Coefficients[2] / 3);
				
				d_c2 = new Point(piece.End - dx / 3,
				                 piece.Coefficients[3] +
				                 (2 / 3.0) * piece.Coefficients[2] +
				                 (1 / 3.0) * piece.Coefficients[1]);
				
				d_end = new Point(piece.End,
				                  piece.Coefficients[0] +
				                  piece.Coefficients[1] +
				                  piece.Coefficients[2] +
				                  piece.Coefficients[3]);
			}
			
			public Point Begin
			{
				get { return d_begin; }
			}
			
			public Point End
			{
				get { return d_end; }
			}
			
			public Point C1
			{
				get { return d_c1; }
			}
			
			public Point C2
			{
				get { return d_c2; }
			}
		}
		
		private List<Piece> d_pieces;
		
		public Bezier() : this(new Piece[] {})
		{
		}
		
		public Bezier(IEnumerable<Piece> pieces)
		{
			d_pieces = new List<Piece>(pieces);
		}
		
		public Bezier(PiecewisePolynomial poly)
		{
			if (poly.Count == 0)
			{
				return;
			}
			
			d_pieces = new List<Piece>();
			
			foreach (PiecewisePolynomial.Piece piece in poly)
			{
				d_pieces.Add(new Piece(piece));
			}
		}
		
		public IEnumerable<Piece> Pieces
		{
			get { return d_pieces; }
		}
		
		public int Count
		{
			get { return d_pieces.Count; }
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return d_pieces.GetEnumerator();
		}
		
		public IEnumerator<Bezier.Piece> GetEnumerator()
		{
			return d_pieces.GetEnumerator();
		}
	}
}

