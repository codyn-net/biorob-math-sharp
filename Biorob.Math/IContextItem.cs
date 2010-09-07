using System;
using System.Collections.Generic;

namespace Biorob.Math
{
	public interface IContextItem
	{
		double Value
		{
			get;
		}

		Dictionary<string, object> Members
		{
			get;
		}
	}
}

