/*
 *  Operations.cs - This file is part of Biorob.Math
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
using System.Reflection;

namespace Biorob.Math
{
	public class Operations
	{
		public class Operation : Attribute
		{
			private int d_arity;
			private string d_name;

			public int Arity
			{
				get
				{
					return d_arity;
				}
				set
				{
					d_arity = value;
				}
			}

			public string Name
			{
				get
				{
					return d_name;
				}
				set
				{
					d_name = value;
				}
			}
		}
		
		public static int ExtractArity(MethodInfo info)
		{
			ParameterInfo[] pars = info.GetParameters();
			int arity = pars.Length;
				
			if (pars.Length > 0 && pars[0].ParameterType == typeof(Value[]))
			{
				arity = -1;
			}
			
			return arity;
		}

		public class Function
		{
			private MethodInfo d_operation;
			private int d_arity;

			public Function(MethodInfo operation)
			{
				d_operation = operation;
				
				d_arity = Operations.ExtractArity(operation);
			}

			public void Execute(Stack<Value> stack, int numargs)
			{
				object[] args;
				
				if (d_arity == -1)
				{
					Value[] arg = new Value[numargs];
					
					for (int i = 0; i < numargs; ++i)
					{
						arg[numargs - i - 1] = stack.Pop();
					}

					args = new object[] {arg};
				}
				else if (d_arity != numargs)
				{
					throw new Exception(String.Format("Number of arguments does not match (got {0}, expected {1})", numargs, d_arity));
				}
				else
				{
					args = new object[numargs];
					
					for (int i = 0; i < numargs; ++i)
					{
						args[numargs - i - 1] = stack.Pop();
					}
				}

				Value ret = (Value)d_operation.Invoke(null, args);
				stack.Push(ret);
			}
			
			public int Arity
			{
				get { return d_arity; }
			}
		}
		
		private delegate double BinaryFunction(double a, double b);
		
		// Normal functions
		private static Value Transform(Value a, Value b, BinaryFunction accum, BinaryFunction f)
		{
			Value ret = new Value(a.Size);
			
			if (ret.Size == 0)
			{
				return ret;
			}
				
			for (int i = 0; i < a.Size; ++i)
			{
				ret[i] = 0;
				
				for (int j = 0; j < b.Size; ++j)
				{
					if (j == 0)
					{
						ret[i] = f(a[i], b[j]);
					}
					else
					{
						ret[i] = accum(ret[i], f(a[i], b[j]));
					}
				}
			}
			
			return ret;
		}
		
		private static Value Transform(Value a, Value b, BinaryFunction f)
		{
			return Transform(a, b, (la, lb) => la + lb, f);
		}

		private delegate double UnaryFunction(double v);
		
		private static Value Transform(Value v, UnaryFunction f)
		{
			Value ret = new Value(v.Size);
			
			for (int i = 0; i < v.Size; ++i)
			{
				ret[i] = f(v[i]);
			}
			
			return ret;
		}
		
		public static Value Pow(Value a, Value b)
		{
			return Transform(a, b, System.Math.Pow);
		}
		
		private delegate void ForallValuesFunc(double v);
		
		private static void ForallValues(Value[] vals, ForallValuesFunc f)
		{
			foreach (Value v in vals)
			{
				for (int i = 0; i < v.Size; ++i)
				{
					f(v[i]);
				}
			}
		}
		
		private static double Accumulate(Value vals, BinaryFunction f)
		{
			return Accumulate(new Value[] {vals}, f);
		}
		
		private static double Accumulate(Value[] vals, BinaryFunction f)
		{
			bool first = true;
			double ret = 0;
			
			ForallValues(vals, (v) => {
				if (first)
				{
					ret = v;
				}
				else
				{
					ret = f(ret, v);
				}
				
				first = false;
			});
			
			return ret;
		}

		public static Value Min(Value[] vals)
		{
			return new Value(Accumulate(vals, System.Math.Min));
		}

		public static Value Max(Value[] vals)
		{
			return new Value(Accumulate(vals, System.Math.Max));
		}

		public static Value Sqrt(Value val)
		{
			return Transform(val, System.Math.Sqrt);
		}

		public static Value Exp(Value val)
		{
			return Transform(val, System.Math.Exp);
		}

		public static Value Ln(Value val)
		{
			return Transform(val, System.Math.Log);
		}
		
		public static Value Log10(Value val)
		{
			return Transform(val, System.Math.Log10);
		}

		public static Value Sin(Value val)
		{
			return Transform(val, System.Math.Sin);
		}

		public static Value Cos(Value val)
		{
			return Transform(val, System.Math.Cos);
		}

		public static Value Tan(Value val)
		{
			return Transform(val, System.Math.Tan);
		}
		
		public static Value Abs(Value val)
		{
			return Transform(val, System.Math.Abs);
		}
		
		public static Value Asin(Value val)
		{
			return Transform(val, System.Math.Asin);
		}
		
		public static Value Acos(Value val)
		{
			return Transform(val, System.Math.Acos);
		}
		
		public static Value Atan(Value val)
		{
			return Transform(val, System.Math.Atan);
		}
		
		public static Value Atan2(Value a, Value b)
		{
			return Transform(a, b, System.Math.Atan2);
		}

		public static Value Round(Value val)
		{
			return Transform(val, System.Math.Round);
		}
		
		public static Value Ceil(Value val)
		{
			return Transform(val, System.Math.Ceiling);
		}
		
		public static Value Floor(Value val)
		{
			return Transform(val, System.Math.Floor);
		}
		
		public static Value Sign(Value val)
		{
			return Transform(val, (a) => (double)System.Math.Sign(a));
		}
		
		public static Value Sum(Value[] vals)
		{
			return new Value(Accumulate(vals, (a, b) => a + b));
		}
		
		public static Value Product(Value[] vals)
		{
			return new Value(Accumulate(vals, (a, b) => a * b));
		}

		// Operators
		public static Value Plus(Value a, Value b)
		{
			if (a.Size >= b.Size)
			{
				return Transform(a, b, (la, lb) => la + lb);
			}
			else
			{
				return Transform(b, a, (la, lb) => la + lb);
			}
		}

		public static Value Minus(Value a, Value b)
		{
			return Transform(a, b, (la, lb) => la - lb);
		}

		public static Value Multiply(Value a, Value b)
		{
			if (a.Size >= b.Size)
			{
				return Transform(a, b, (la, lb) => la * lb);
			}
			else
			{
				return Transform(b, a, (la, lb) => la * lb);
			}
		}

		public static Value Divide(Value a, Value b)
		{
			return Transform(a, b, (la, lb) => la / lb);
		}

		public static Value Modulo(Value a, Value b)
		{
			return Transform(a, b, (la, lb) => la % lb);
		}

		public static Value Equal(Value a, Value b)
		{
			return Transform(a, b, (la, lb) => (la != 0 && lb != 0) ? 1 : 0, (la, lb) => la == lb ? 1 : 0);
		}

		public static Value Greater(Value a, Value b)
		{
			return Transform(a, b, (la, lb) => (la != 0 && lb != 0) ? 1 : 0, (la, lb) => la > lb ? 1 : 0);
		}

		public static Value Less(Value a, Value b)
		{
			return Transform(a, b, (la, lb) => (la != 0 && lb != 0) ? 1 : 0, (la, lb) => la < lb ? 1 : 0);
		}

		public static Value GreaterOrEqual(Value a, Value b)
		{
			return Transform(a, b, (la, lb) => (la != 0 && lb != 0) ? 1 : 0, (la, lb) => la >= lb ? 1 : 0);
		}

		public static Value LessOrEqual(Value a, Value b)
		{
			return Transform(a, b, (la, lb) => (la != 0 && lb != 0) ? 1 : 0, (la, lb) => la <= lb ? 1 : 0);
		}

		public static Value And(Value a, Value b)
		{
			return Transform(a, b, (la, lb) => (la != 0 && lb != 0) ? 1 : 0, (la, lb) => (la != 0) && (lb != 0) ? 1 : 0);
		}
		
		public static Value Or(Value a, Value b)
		{
			return Transform(a, b, (la, lb) => (la != 0 && lb != 0) ? 1 : 0, (la, lb) => (la != 0 || lb != 0) ? 1 : 0);
		}

		public static Value Negate(Value val)
		{
			return Transform(val, (v) => v == 0 ? 1 : 0);
		}

		public static Value UnaryPlus(Value val)
		{
			return new Value(val);
		}

		public static Value UnaryMinus(Value val)
		{
			return Transform(val, (v) => -v);
		}
		
		public static Value Complement(Value val)
		{
			return Transform(val, (v) => ~(int)v);
		}

		public static Value Ternary(Value condition, Value truepart, Value falsepart)
		{
			double cond = Accumulate(condition, (a, b) => (a != 0 && b != 0) ? 1 : 0);
			
			if (cond != 0)
			{
				return new Value(truepart);
			}
			else
			{
				return new Value(falsepart);
			}
		}
		
		public static Value Range(Value a, Value b)
		{
			if (a.Size == 0 || b.Size == 0)
			{
				return new Value();
			}
			
			int start = (int)a[0];
			int end = (int)b[0];
			int dir = System.Math.Sign(end - start);
			
			int num = dir * (end - start) + 1;
			
			Value ret = new Value(num);
			
			for (int i = 0; i < num; ++i)
			{
				ret[i] = start + dir * i;
			}
			
			return ret;
		}

		public static Function LookupOperator(TokenOperator.OperatorType type, int arity)
		{
			switch (type)
			{
				case TokenOperator.OperatorType.Plus:
					return LookupFunction("plus", arity);
				case TokenOperator.OperatorType.Minus:
					return LookupFunction("minus", arity);
				case TokenOperator.OperatorType.Multiply:
					return LookupFunction("multiply", arity);
				case TokenOperator.OperatorType.Divide:
					return LookupFunction("divide", arity);
				case TokenOperator.OperatorType.Modulo:
					return LookupFunction("modulo", arity);
				case TokenOperator.OperatorType.Less:
					return LookupFunction("less", arity);
				case TokenOperator.OperatorType.Greater:
					return LookupFunction("greater", arity);
				case TokenOperator.OperatorType.LessOrEqual:
					return LookupFunction("lessorequal", arity);
				case TokenOperator.OperatorType.GreaterOrEqual:
					return LookupFunction("greaterorequal", arity);
				case TokenOperator.OperatorType.Equal:
					return LookupFunction("equal", arity);
				case TokenOperator.OperatorType.Negate:
					return LookupFunction("negate", arity);
				case TokenOperator.OperatorType.And:
					return LookupFunction("and", arity);
				case TokenOperator.OperatorType.Or:
					return LookupFunction("or", arity);
				case TokenOperator.OperatorType.Power:
					return LookupFunction("pow", arity);
				case TokenOperator.OperatorType.Range:
					return LookupFunction("range", arity);
				case TokenOperator.OperatorType.UnaryPlus:
					return LookupFunction("unaryplus", arity);
				case TokenOperator.OperatorType.UnaryMinus:
					return LookupFunction("unaryminus", arity);
				case TokenOperator.OperatorType.Ternary:
					return LookupFunction("ternary", arity);
				case TokenOperator.OperatorType.Complement:
					return LookupFunction("complement", arity);
			}

			return null;
		}

		public static Function LookupFunction(string identifier, int arity)
		{
			// Iterate over all the functions, find the one with the right name
			MethodInfo[] methods = typeof(Operations).GetMethods(BindingFlags.Static | BindingFlags.Public);
			MethodInfo fallback = null;

			foreach (MethodInfo method in methods)
			{
				if (method.ReturnType != typeof(Value) || method.Name.ToLower() != identifier.ToLower())
				{
					continue;
				}
				
				int num = Operations.ExtractArity(method);
				
				if (num == -1)
				{
					fallback = method;
				}
				else
				{
					return new Function(method);
				}
			}
			
			if (fallback != null)
			{
				return new Function(fallback);
			}

			return null;
		}
	}
}
