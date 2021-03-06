/*
 *  Tokenizer.cs - This file is part of Biorob.Math
 *
 *  Copyright (C) 2010 - Jesse van den Kieboom
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by the
 * Free Software Foundation; either version 2.1 of the License, or (at your
 * option) any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this library; if not, write to the Free Software Foundation,
 * Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
 */

using System;
using System.IO;

namespace Biorob.Math
{
	public class Token
	{
		public enum TokenType
		{
			None,
			Number,
			Identifier,
			Operator
		}

		public TokenType Type;
		public string Text;

		public Token(TokenType type, string text)
		{
			Type = type;
			Text = text;
		}
	}

	public class TokenNumber : Token
	{
		public double Value;

		public TokenNumber(string text) : base(Token.TokenType.Number, text)
		{
			Value = Double.Parse(text);
		}

		public override string ToString()
		{
			return string.Format("[TokenNumber: {0} ({1})]", Value, Text);
		}
	}

	public class TokenIdentifier : Token
	{
		public TokenIdentifier(string identifier) : base(Token.TokenType.Identifier, identifier)
		{
		}

		public override string ToString()
		{
			return string.Format("[TokenIdentifier: {0}]", Text);
		}
	}

	public class TokenOperator : Token
	{
		public struct OpSet
		{
			public int Priority;
			public bool LeftAssoc;

			public OpSet(int priority, bool leftAssoc)
			{
				Priority = priority;
				LeftAssoc = leftAssoc;
			}
		}

		public enum OperatorType
		{
			None,

			Arithmetic,
			Multiply,
			Divide,
			Modulo,
			Plus,
			Minus,
			Power,
			Range,

			Logical,
			Negate,
			Greater,
			Less,
			GreaterOrEqual,
			LessOrEqual,
			Equal,
			Or,
			And,
			Complement,

			Ternary,
			TernaryTrue,
			TernaryFalse,

			Group,
			GroupStart,
			GroupEnd,
			
			Vector,
			VectorStart,
			VectorEnd,

			Comma,

			Unary,
			UnaryPlus,
			UnaryMinus
		}

		public static OpSet[] OperatorProperties = new OpSet[] {
				new OpSet(0, false), // None,

				// arithmetic operators
				new OpSet(0, false), // Arithmetic
				new OpSet(7, true),  // Multiply
				new OpSet(7, true),  // Divide
				new OpSet(7, true),  // Modulo
				new OpSet(6, true),  // Plus
				new OpSet(6, true),  // Minus
				new OpSet(11, false), // Power
				new OpSet(11, false), // Range

				// logical operators
				new OpSet(0, false), // Logical
				new OpSet(8, false), // Negate
				new OpSet(5, true),  // Greater
				new OpSet(5, true),  // Less
				new OpSet(5, true),  // GreaterOrEqual
				new OpSet(5, true),  // LessOrEqual
				new OpSet(4, true),  // Equal
				new OpSet(2, true),  // Or
				new OpSet(3, true),  // And
				new OpSet(8, false), // Complement

				// ternary operator
				new OpSet(0, false), // Ternary
				new OpSet(1, false), // TernaryTrue
				new OpSet(1, false), // TernaryFalse

				// group 'operator'
				new OpSet(0, false), // Group
				new OpSet(11, true), // GroupStart
				new OpSet(11, true), // GroupEnd
				
				// vector 'operator'
				new OpSet(0, false), // Vector
				new OpSet(9, true), // VectorStart
				new OpSet(9, true), // VectorEnd

				new OpSet(11, true), // Comma

				// unary
				new OpSet(0, false), // Unary
				new OpSet(10, false), // UnaryPlus
				new OpSet(10, false)  // UnaryMinus
		};

		public OperatorType OpType;
		public OpSet Properties;

		public TokenOperator(OperatorType type, string text) : base(Token.TokenType.Operator, text)
		{
			OpType = type;
			Properties = OperatorProperties[(int)type];
		}

		public override string ToString()
		{
			return string.Format("[TokenOperator: {0}]", Text);
		}
	}

	public class Tokenizer
	{
		private string d_text;
		private int d_pos;

		static string Number = "0123456789";
		static string Alpha = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

		public Tokenizer(string text)
		{
			d_text = text;
			d_pos = 0;
		}

		public string Text
		{
			get
			{
				return d_text;
			}
		}

		private void SkipWhitespace()
		{
			string ws = "\t \n\r";
			while (d_pos < d_text.Length && ws.IndexOf(d_text[d_pos]) >= 0)
			{
				++d_pos;
			}
		}

		private bool IsNumber()
		{
			if (d_text.StartsWith(".."))
			{
				return false;
			}

			string number = Number + ".";
			return number.IndexOf(d_text[d_pos]) >= 0;
		}

		private bool IsIdentifier()
		{
			string alpha = Alpha + "_" + "{";
			return alpha.IndexOf(d_text[d_pos]) >= 0;
		}

		public bool IsOperator()
		{
			switch (d_text[d_pos])
			{
				case '*':
				case '-':
				case '+':
				case '.':
				case '/':
				case '%':
				case '!':
				case '<':
				case '>':
				case '^':
				case '~':
				case '=':
				case '?':
				case ':':
				case '&':
				case '|':
				case '(':
				case ')':
				case ',':
				case '[':
				case ']':
					return true;
			}

			return false;
		}

		private TokenNumber ParseNumber()
		{
			string valid = Number + ".";
			int start = d_pos;

			do
			{
				++d_pos;
				
				if (d_pos + 1 < d_text.Length && d_text[d_pos] == '.' && d_text[d_pos + 1] == '.')
				{
					break;
				}
			}
			while (d_pos < d_text.Length && valid.IndexOf(d_text[d_pos]) >= 0);

			string text = d_text.Substring(start, d_pos - start);
			return new TokenNumber(text);
		}

		private TokenOperator ParseOperator()
		{
			int start = d_pos;
			char c = d_text[d_pos];
			TokenOperator.OperatorType type = TokenOperator.OperatorType.None;

			if (d_pos + 1 < d_text.Length)
			{
				char n = d_text[d_pos + 1];

				if (c == '*' && n == '*')
				{
					type = TokenOperator.OperatorType.Power;
				}
				else if (c == '=' && n == '=')
				{
					type = TokenOperator.OperatorType.Equal;
				}
				else if (c == '>' && n == '=')
				{
					type = TokenOperator.OperatorType.GreaterOrEqual;
				}
				else if (c == '<' && n == '=')
				{
					type = TokenOperator.OperatorType.LessOrEqual;
				}
				else if (c == '|' && n == '|')
				{
					type = TokenOperator.OperatorType.Or;
				}
				else if (c == '&' && n == '&')
				{
					type = TokenOperator.OperatorType.And;
				}
				else if (c == '.' && n == '.')
				{
					type = TokenOperator.OperatorType.Range;
				}

				if (type != TokenOperator.OperatorType.None)
				{
					d_pos += 2;
					return new TokenOperator(type, d_text.Substring(start, 2));
				}
			}

			++d_pos;

			switch (c)
			{
				case '*':
					type = TokenOperator.OperatorType.Multiply;
				break;
				case '/':
					type = TokenOperator.OperatorType.Divide;
				break;
				case '%':
					type = TokenOperator.OperatorType.Modulo;
				break;
				case '-':
					type = TokenOperator.OperatorType.Minus;
				break;
				case '+':
					type = TokenOperator.OperatorType.Plus;
				break;
				case '!':
					type = TokenOperator.OperatorType.Negate;
				break;
				case '>':
					type = TokenOperator.OperatorType.Greater;
				break;
				case '<':
					type = TokenOperator.OperatorType.Less;
				break;
				case '^':
					type = TokenOperator.OperatorType.Power;
				break;
				case '~':
					type = TokenOperator.OperatorType.Complement;
				break;
				case '?':
					type = TokenOperator.OperatorType.TernaryTrue;
				break;
				case ':':
					type = TokenOperator.OperatorType.TernaryFalse;
				break;
				case '(':
					type = TokenOperator.OperatorType.GroupStart;
				break;
				case ')':
					type = TokenOperator.OperatorType.GroupEnd;
				break;
				case '[':
					type = TokenOperator.OperatorType.VectorStart;
				break;
				case ']':
					type = TokenOperator.OperatorType.VectorEnd;
				break;
				case ',':
					type = TokenOperator.OperatorType.Comma;
				break;
			}

			if (type == TokenOperator.OperatorType.None)
			{
				return null;
			}

			return new TokenOperator(type, d_text.Substring(start, 1));
		}

		private TokenIdentifier ParseIdentifier()
		{
			string valid = Alpha + "_" + Number + ".";
			int start = d_pos;
			
			if (d_text[d_pos] == '{')
			{
				++start;
				++d_pos;
				
				while (d_pos < d_text.Length && d_text[d_pos] != '}')
				{
					++d_pos;
				}
			}
			else
			{
				while (d_pos < d_text.Length && valid.IndexOf(d_text[d_pos]) >= 0)
				{
					++d_pos;
				}
			}

			return new TokenIdentifier(d_text.Substring(start, d_pos - start));
		}

		private Token GetToken()
		{
			if (d_text == null)
			{
				return null;
			}

			d_pos = 0;

			SkipWhitespace();

			if (d_pos >= d_text.Length)
			{
				return null;
			}

			if (IsNumber())
			{
				return ParseNumber();
			}
			else if (IsIdentifier())
			{
				return ParseIdentifier();
			}
			else if (IsOperator())
			{
				return ParseOperator();
			}
			else
			{
				return null;
			}
		}

		public Token Next()
		{
			Token ret = GetToken();
			d_text = d_text.Substring(d_pos);
			d_pos = 0;

			return ret;
		}

		public Token Peek()
		{
			Token ret = GetToken();
			d_pos = 0;

			return ret;
		}

		public bool End()
		{
			return String.IsNullOrEmpty(d_text);
		}
	}
}
