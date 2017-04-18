using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	public class BitMaskAttribute : PropertyAttribute
	{
		public System.Type propType;

		public BitMaskAttribute (System.Type aType)
		{
			propType = aType;
		}
	}
}
