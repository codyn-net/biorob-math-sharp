/*
 *  Expression.cs - This file is part of Biorob.Math
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
using System.Collections.Generic;

namespace Biorob.Math
{
	public class Expression
	{
		public class ContextException : Exception
		{
			public ContextException(string fmt, params string[] args) : base(String.Format(fmt, args))
			{
			}
		}
		
		public class ParseException : Exception
		{
			private Expression d_expression;

			public ParseException(Expression expr) : base(expr.ErrorMessage)
			{
				d_expression = expr;
			}
			
			public Expression Expression
			{
				get
				{
					return d_expression;
				}
			}
		}

		string d_text;
		string d_error;
		bool d_checkVariables;

		List<Instruction> d_instructions;

		public Expression()
		{
			d_instructions = new List<Instruction>();
			d_checkVariables = false;
		}
		
		public Expression(string expr) : this()
		{
			if (!Parse(expr))
			{
				Expression cp = new Expression();
				
				cp.d_error = d_error;
				cp.d_instructions = d_instructions;
				cp.d_text = d_text;

				throw new ParseException(cp);
			}
		}
		
		public static void Create(string expression, out Expression expr)
		{
			expr = new Expression();
			
			if (!expr.Parse(expression))
			{
				ParseException exc = new ParseException(expr);
				expr = null;
				
				throw exc;
			}
		}

		public bool CheckVariables
		{
			get
			{
				return d_checkVariables;
			}
			set
			{
				d_checkVariables = value;
			}
		}

		private bool Error(string text, Tokenizer tokenizer)
		{
			d_error = text;

			if (tokenizer != null)
			{
				d_error += " [" + (tokenizer.Text != null ? tokenizer.Text : "") + "]";
			}

			return false;
		}

		public static implicit operator bool(Expression expr)
		{
			return expr.d_instructions.Count != 0;
		}

		public string Text
		{
			get
			{
				return d_text;
			}
		}

		private bool ParseNumber(Tokenizer tokenizer, TokenNumber token)
		{
			d_instructions.Add(new InstructionValue(token.Value));
			return true;
		}

		private bool ParseFunction(Tokenizer tokenizer, TokenIdentifier identifier)
		{
			Token next = tokenizer.Peek();
			bool loopit = true;
			int numargs = 0;

			if (next != null && (next is TokenOperator) && ((TokenOperator)next).OpType == TokenOperator.OperatorType.GroupEnd)
			{
				// Consume group end
				tokenizer.Next();
				loopit = false;
			}

			while (loopit)
			{
				if (!ParseExpression(tokenizer, -1, false))
				{
					return false;
				}

				++numargs;

				// See what is next
				next = tokenizer.Peek();

				if (next == null || !(next is TokenOperator))
				{
					return Error("Expected `operator', but got " + (next != null ? next.Text : "(nothing)"), tokenizer);
				}

				TokenOperator nextop = next as TokenOperator;

				if (nextop.OpType == TokenOperator.OperatorType.GroupEnd)
				{
					// Consume it
					tokenizer.Next();
					break;
				}
				else if (nextop.OpType != TokenOperator.OperatorType.Comma)
				{
					return Error("Expected `,' but got " + next.Text, tokenizer);
				}

				// Consume token
				tokenizer.Next();
			}
			
			Operations.Function func = Operations.LookupFunction(identifier.Text, numargs);
			
			if (func == null)
			{
				return Error(String.Format("Invalid function `{0}' for provided arguments", identifier.Text), tokenizer);
			}

			d_instructions.Add(new InstructionFunction(identifier.Text, func, numargs));
			return true;
		}

		private bool ParseIdentifier(Tokenizer tokenizer, TokenIdentifier identifier)
		{
			bool ret = false;

			// Consume token and peek the next to see if the identifier is a function call
			tokenizer.Next();
			Token next = tokenizer.Peek();
			TokenOperator nextop = next != null ? next as TokenOperator : null;

			if (nextop != null && nextop.OpType == TokenOperator.OperatorType.GroupStart)
			{
				// Consume peeked group start
				tokenizer.Next();
				ret = ParseFunction(tokenizer, identifier);
			}
			else
			{
				d_instructions.Add(new InstructionIdentifier(identifier.Text.Split(new char[] {'.'})));
				ret = true;
			}

			return ret;
		}
		
		private bool ParseVector(Tokenizer tokenizer, bool indexer)
		{
			Token next = tokenizer.Peek();
			bool loopit = true;
			int numargs = 0;

			if (next != null && (next is TokenOperator) && ((TokenOperator)next).OpType == TokenOperator.OperatorType.VectorEnd)
			{
				// Consume vector end
				tokenizer.Next();
				loopit = false;
			}

			while (loopit)
			{
				if (!ParseExpression(tokenizer, -1, false))
				{
					return false;
				}

				++numargs;

				// See what is next
				next = tokenizer.Peek();

				if (next == null || !(next is TokenOperator))
				{
					return Error("Expected `operator', but got " + (next != null ? next.Text : "(nothing)"), tokenizer);
				}

				TokenOperator nextop = next as TokenOperator;

				if (nextop.OpType == TokenOperator.OperatorType.VectorEnd)
				{
					// Consume it
					tokenizer.Next();
					break;
				}
				else if (nextop.OpType != TokenOperator.OperatorType.Comma)
				{
					return Error("Expected `,' but got " + next.Text, tokenizer);
				}

				// Consume token
				tokenizer.Next();
			}
			
			if (indexer)
			{
				InstructionFunction f = d_instructions[d_instructions.Count - 1] as InstructionFunction;
				
				if (numargs == 1 && f != null && f.Name == "..")
				{
					d_instructions.Remove(f);
					d_instructions.Add(new InstructionIndexRange());
				}
				else
				{
					d_instructions.Add(new InstructionIndex(numargs));
				}
			}
			else
			{			
				d_instructions.Add(new InstructionVector(numargs));
			}

			return true;
		}

		private bool ParseGroup(Tokenizer tokenizer)
		{
			if (!ParseExpression(tokenizer, -1, false))
			{
				return false;
			}

			Token next = tokenizer.Peek();
			bool groupend = (next != null && next is TokenOperator && ((TokenOperator)next).OpType == TokenOperator.OperatorType.GroupEnd);

			if (!groupend)
			{
				return Error("Expected `)' but got " + (next != null ? next.Text : "(nothing)"), tokenizer);
			}

			tokenizer.Next();
			return true;
		}

		private bool ParseUnaryOperator(Tokenizer tokenizer, TokenOperator token)
		{
			// Handle group
			if (token.OpType == TokenOperator.OperatorType.GroupStart)
			{
				// Consume token
				tokenizer.Next();
				return ParseGroup(tokenizer);
			}
			else if (token.OpType == TokenOperator.OperatorType.VectorStart)
			{
				// Consume token
				tokenizer.Next();
				return ParseVector(tokenizer, false);
			}

			bool ret = true;

			switch (token.OpType)
			{
				case TokenOperator.OperatorType.Minus:
					token = new TokenOperator(TokenOperator.OperatorType.UnaryMinus, "-");
				break;
				case TokenOperator.OperatorType.Plus:
					token = new TokenOperator(TokenOperator.OperatorType.UnaryPlus, "+");
				break;
				case TokenOperator.OperatorType.Negate:
				break;
				case TokenOperator.OperatorType.Complement:
				break;
				default:
					Error("Expected unary operator (-, +, !) but got `" + token.Text + "'", tokenizer);
					ret = false;
				break;
			}
			
			if (ret)
			{
				tokenizer.Next();
				ret = ParseExpression(tokenizer, token.Properties.Priority, token.Properties.LeftAssoc);
			}

			if (ret)
			{
				Instruction inst = new InstructionFunction(token.Text, Operations.LookupOperator(token.OpType, 1), 1);
				
				if (inst != null)
				{
					d_instructions.Add(inst);
				}
			}

			return ret;
		}

		private bool ParseTernaryOperator(Tokenizer tokenizer, TokenOperator token)
		{
			if (!ParseExpression(tokenizer, token.Properties.Priority, token.Properties.LeftAssoc))
			{
				return false;
			}

			// Next should be :
			Token next = tokenizer.Peek();

			if (next == null)
			{
				return Error("Expected `:' but got (nothing)", tokenizer);
			}

			TokenOperator nextop = next as TokenOperator;
			bool istern = nextop != null && nextop.OpType == TokenOperator.OperatorType.TernaryFalse;

			if (!istern)
			{
				return Error("Expected `:' but got " + next.Text, tokenizer);
			}

			tokenizer.Next();

			if (!ParseExpression(tokenizer, nextop.Properties.Priority, nextop.Properties.LeftAssoc))
			{
				return false;
			}

			d_instructions.Add(new InstructionFunction("?:", Operations.LookupOperator(TokenOperator.OperatorType.Ternary, 3), 3));
			return true;
		}

		private bool ParseOperator(Tokenizer tokenizer, TokenOperator token)
		{
			// Handle ternary
			if (token.OpType == TokenOperator.OperatorType.TernaryTrue)
			{
				tokenizer.Next();
				return ParseTernaryOperator(tokenizer, token);
			}
			else if (token.OpType == TokenOperator.OperatorType.VectorStart)
			{
				tokenizer.Next();
				return ParseVector(tokenizer, true);
			}

			Operations.Function func = Operations.LookupOperator(token.OpType, 2);

			if (func == null)
			{
				return Error("Unknown operator `" + token.Text + "'", tokenizer);
			}

			tokenizer.Next();
			
			if (!ParseExpression(tokenizer, token.Properties.Priority, token.Properties.LeftAssoc))
			{
				return false;
			}

			d_instructions.Add(new InstructionFunction(token.Text, func, 2));
			return true;
		}

		private bool ParseExpression(Tokenizer tokenizer, int priority, bool leftAssoc)
		{
			Token token;
			bool ret = false;
			int num = 0;

			while ((token = tokenizer.Peek()) != null)
			{
				switch (token.Type)
				{
					case Token.TokenType.Number:
						ret = ParseNumber(tokenizer, token as TokenNumber);
					break;
					case Token.TokenType.Identifier:
						ret = ParseIdentifier(tokenizer, token as TokenIdentifier);
						token = null;
					break;
					case Token.TokenType.Operator:
						TokenOperator op = token as TokenOperator;

						if (op.OpType == TokenOperator.OperatorType.GroupEnd ||
						    op.OpType == TokenOperator.OperatorType.Comma ||
						    op.OpType == TokenOperator.OperatorType.TernaryFalse ||
						    op.OpType == TokenOperator.OperatorType.VectorEnd)
						{
							// End of group
							return true;
						}

						if (num == 0)
						{
							ret = ParseUnaryOperator(tokenizer, op);

							if (ret)
							{
								token = null;
							}
						}
						else if (op.Properties.Priority < priority ||
						         (op.Properties.Priority == priority && leftAssoc))
						{
							// Do not handle operator here yet
							return true;
						}
						else
						{
							ret = ParseOperator(tokenizer, op);

							if (ret)
							{
								token = null;
							}
						}
					break;
					default:
						ret = Error("Unknown token: " + token.Text, tokenizer);
					break;
				}

				if (token != null)
				{
					// Consume token
					if (ret)
					{
						tokenizer.Next();
					}
				}

				++num;

				if (!ret)
				{
					break;
				}
			}

			if (!ret)
			{
				if (String.IsNullOrEmpty(d_error))
				{
					Error("Expected expression but got (nothing)", tokenizer);
				}
			}

			return ret;
		}

		public string ErrorMessage
		{
			get
			{
				return d_error;
			}
		}

		public bool Parse(string text)
		{
			Tokenizer tokenizer = new Tokenizer(text);

			d_instructions.Clear();
			d_error = null;
			
			if (text == null || text.Trim() == String.Empty)
			{
				d_instructions.Add(new InstructionValue(0));
				d_text = "";
				return true;
			}

			if (!ParseExpression(tokenizer, -1, false))
			{
				d_instructions.Clear();
				return false;
			}

			d_text = text;
			return true;
		}
		
		public string[] ResolveUnknowns(params Dictionary<string, object>[] context)
		{
			List<string> ret = new List<string>();
			ResolveUnknowns(ret, context);
			
			return ret.ToArray();
		}
		
		private void ResolveUnknowns(List<string> ret, params Dictionary<string, object>[] context)
		{
			foreach (Instruction instruction in d_instructions)
			{
				InstructionIdentifier id = instruction as InstructionIdentifier;

				if (id == null)
				{
					continue;
				}
				
				if (ret.Contains(id.Identifier[0]))
				{
					continue;
				}
				
				bool found = false;
				
				foreach (Dictionary<string, object> ctx in context)
				{
					object obj;

					if (id.Find(ctx, out obj))
					{
						Expression other = obj as Expression;

						if (other != null)
						{
							other.ResolveUnknowns(ret, context);
						}
						
						found = true;
						break;
					}
				}
				
				if (!found)
				{
					ret.Add(id.Identifier[0]);
				}
			}
		}

		public bool ValidateVariables(params Dictionary<string, object>[] context)
		{
			return ResolveUnknowns(context).Length == 0;
		}

		public Value Evaluate(params Dictionary<string, object>[] context)
		{
			if (d_instructions.Count == 0)
			{
				return new Value(0.0);
			}

			if (d_checkVariables)
			{
				string[] unknowns = ResolveUnknowns(context);

				if (unknowns.Length != 0)
				{
					throw new ContextException("The variables `{0}' are unknown", string.Join(", ", unknowns));
				}
			}

			Stack<Value> stack = new Stack<Value>();
			Dictionary<string, object> all = new Dictionary<string, object>();

			foreach (Dictionary<string, object> dic in context)
			{
				foreach (KeyValuePair<string, object> pair in dic)
				{
					all[pair.Key] = pair.Value;
				}
			}

			foreach (Instruction inst in d_instructions)
			{
				inst.Execute(stack, all);
			}

			if (stack.Count != 1)
			{
				Console.Error.WriteLine("Invalid stack size: " + stack.Count);
			}

			return stack.Pop();
		}

		public double Evaluate()
		{
			return Evaluate(new Dictionary<string, object>());
		}
		
		public Instruction[] Instructions
		{
			get
			{
				return d_instructions.ToArray();
			}
		}
	}
}
