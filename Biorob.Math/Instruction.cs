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
		public abstract void Execute(Stack<Value> stack, Dictionary<string, object> context);
	}

	public class InstructionValue : Instruction
	{
		public Value Value;
		
		public InstructionValue(double val)
		{
			Value = new Value(val);
		}

		public InstructionValue(Value val)
		{
			Value = val;
		}

		public override void Execute(Stack<Value> stack, Dictionary<string, object> context)
		{
			stack.Push(Value);
		}
		
		public override string ToString()
		{
			return String.Format("Value({0})", Value);
		}
	}
	
	public class InstructionVector : Instruction
	{
		public int NumArgs;
		
		public InstructionVector(int numargs)
		{
			NumArgs = numargs;
		}

		public override void Execute(Stack<Value> stack, Dictionary<string, object> context)
		{
			Stack<Value> vals = new Stack<Value>();
			
			int size = 0;
			
			for (int i = 0; i < NumArgs; ++i)
			{
				Value v = stack.Pop();
				vals.Push(v);
				
				size += v.Size;
			}
			
			Value ret = new Value(size);
			int idx = 0;
			
			while (vals.Count > 0)
			{
				Value v = vals.Pop();

				for (int j = 0; j < v.Size; ++j)
				{
					ret[idx++] = v[j];
				}
			}
			
			stack.Push(ret);
		}
		
		public override string ToString()
		{
			return String.Format("Vector({0})", NumArgs);
		}
	}
	
	public class InstructionIndexRange : InstructionIndex
	{
		public InstructionIndexRange() : base(1)
		{
		}
		
		public override void Execute(Stack<Value> stack, Dictionary<string, object> context)
		{
			Value end = stack.Pop();
			Value start = stack.Pop();
			Value v = stack.Peek();
			
			if (end.Size == 0 || start.Size == 0)
			{
				stack.Push(new Value());
				return;
			}
			
			int s = (int)start[0];
			int e = (int)end[0];
			
			if (s < 0)
			{
				s = v.Size + s;
			}
			
			if (e < 0)
			{
				e = v.Size + e;
			}
			
			stack.Push(Operations.Range(new Value((double)s), new Value((double)e)));
			base.Execute(stack, context);
		}
	}
	
	public class InstructionIndex : InstructionVector
	{
		public InstructionIndex(int numargs) : base(numargs)
		{
		}

		public override void Execute(Stack<Value> stack, Dictionary<string, object> context)
		{
			base.Execute(stack, context);
			
			Value indexer = stack.Pop();
			Value indexed = stack.Pop();
			
			List<double> ret = new List<double>();
			
			for (int i = 0; i < indexer.Size; ++i)
			{
				int idx = (int)indexer[i];
				
				if (idx >= 0 && idx < indexed.Size)
				{
					ret.Add(indexed[idx]);
				}
			}
			
			stack.Push(new Value(ret.ToArray()));
		}
		
		public override string ToString()
		{
			return String.Format("Index({0})", NumArgs);
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

		public override void Execute(Stack<Value> stack, Dictionary<string, object> context)
		{
			object val;
			
			if (!Find(context, out val))
			{			
				stack.Push(new Value(0));
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
				else if (val.GetType() == typeof(double[]))
				{
					stack.Push(new Value((double[])val));
				}
				else if (val is Value)
				{
					stack.Push((Value)val);
				}
				else
				{
					try
					{
						stack.Push(new Value(Convert.ToDouble(val)));
					}
					catch (InvalidCastException)
					{
						Console.Error.WriteLine("Failed to convert `{0}' to double!", val);
						stack.Push(new Value(0));
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
		public int NumArgs;

		public InstructionFunction(string name, Operations.Function function, int numargs)
		{
			Name = name;
			Function = function;
			NumArgs = numargs;
		}

		public override void Execute(Stack<Value> stack, Dictionary<string, object> context)
		{
			Function.Execute(stack, NumArgs);
		}
		
		public override string ToString()
		{
			return String.Format("Fun({0})", Name);
		}
	}
}
