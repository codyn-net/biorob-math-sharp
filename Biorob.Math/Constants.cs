/*
 *  Constants.cs - This file is part of Biorob.Math
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
	public class Constants
	{
		static Dictionary<string, object> s_context;
		
		public static double Epsilon = 1e-10;

		static Constants()
		{
			s_context = new Dictionary<string, object>();

			s_context["PI"] = System.Math.PI;
			s_context["pi"] = System.Math.PI;
			s_context["E"] = System.Math.E;
			s_context["e"] = System.Math.E;
		}

		public static Dictionary<string, object> Context
		{
			get
			{
				return s_context;
			}
		}
	}
}
