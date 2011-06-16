/*
 *  Instruction.cs - This file is part of Biorob.Math
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
	public abstract class Instruction
	{
		public abstract void Execute(Stack<double> stack, Dictionary<string, object> context);
	}

	public class InstructionNumber : Instruction
	{
		public double Value;

		public InstructionNumber(double val)
		{
			Value = val;
		}

		public override void Execute(Stack<double> stack, Dictionary<string, object> context)
		{
			stack.Push(Value);
		}
		
		public override string ToString()
		{
			return String.Format("Num({0})", Value);
		}
	}

	public class InstructionIdentifier : Instruction
	{
		public string[] Identifier;

		public InstructionIdentifier(params string[] identifier)
		{
			Identifier = identifier;
		}
		
		public bool Find(Dictionary<string, object> context, out object obj)
		{
			string ident;
			obj = null;

			for (int i = 0; i < Identifier.Length - 1; ++i)
			{
				ident = Identifier[i];

				if (!context.ContainsKey(ident))
				{
					return false;
				}
				
				IContextItem item = context[ident] as IContextItem;
				
				if (item != null)
				{
					context = item.Members;
				}
				else
				{
					context = null;
				}
				
				if (context == null)
				{
					return false;
				}
			}
			
			ident = Identifier[Identifier.Length - 1];
			
			if (!context.ContainsKey(ident))
			{
				return false;
			}
			
			obj = context[ident];
			return true;
		}

		public override void Execute(Stack<double> stack, Dictionary<string, object> context)
		{
			object val;
			
			if (!Find(context, out val))
			{			
				stack.Push(0);
				return;
			}

			Expression expr = val as Expression;

			if (expr != null)
			{
				stack.Push(expr.Evaluate(context));
			}
			else
			{
				IContextItem item = val as IContextItem;
				
				if (item != null)
				{
					stack.Push(item.Value);
				}
				else
				{
					try
					{
						stack.Push(Convert.ToDouble(val));
					}
					catch (InvalidCastException)
					{
						Console.Error.WriteLine("Failed to convert `{0}' to double!", val);
						stack.Push(0);
					}
				}
			}
		}
		
		public override string ToString()
		{
			return String.Format("Idt({0})", String.Join(".", Identifier));
		}
	}

	public class InstructionFunction : Instruction
	{
		public string Name;
		public Operations.Function Function;

		public InstructionFunction(string name, Operations.Function function)
		{
			Name = name;
			Function = function;
		}

		public override void Execute(Stack<double> stack, Dictionary<string, object> context)
		{
			Function.Execute(stack);
		}
		
		public override string ToString()
		{
			return String.Format("Fun({0})", Name);
		}
	}
}
