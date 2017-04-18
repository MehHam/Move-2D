using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	/// <summary>
	/// A group of function to be called by the ReadSetGo animator
	/// </summary>
	public class ReadySetGo : MonoBehaviour
	{
		public delegate void ReadySetGoEvent ();

		public static event ReadySetGoEvent onAnimationFinished;

		void ReadySetGoFinished ()
		{
			if (onAnimationFinished != null)
				onAnimationFinished ();
		}
	}
}
