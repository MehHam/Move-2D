using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadySetGo : MonoBehaviour {
	public delegate void ReadySetGoEvent();
	public static event ReadySetGoEvent onAnimationFinished;

	void ReadySetGoFinished()
	{
		if (onAnimationFinished != null)
			onAnimationFinished ();
	}
}
