using System;
using System.Collections.Generic;

namespace Biorob.Math
{
	public interface IContextItem
	{
		Value Value
		{
			get;
		}

		Dictionary<string, object> Members
		{
			get;
		}
	}
}

