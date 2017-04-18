using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	public enum BuildType
	{
		None = 0,
		Any = 0xFFFF,
		WebGL = 1,
		Mac = 2,
		Editor = 4,
		Windows = 8,
		Linux = 16,
		IOS = 32,
		Android = 64,
	}

	public class HideIfBuildUI : MonoBehaviour
	{

		[BitMask (typeof(BuildType))] public BuildType someMask;

		#if UNITY_EDITOR
		const int build = 4;
		#elif UNITY_WEBGL
		const int build = 1;
		

#elif UNITY_STANDALONE_OSX
		const int build = 2;
		

#elif UNITY_STANDALONE_WIN
		const int build = 8;
		

#elif UNITY_STANDALONE_LINUX
		const int build = 16
		

#elif UNITY_IOS
		const int build = 32;
		

#elif UNITY_ANDROID
		const int build = 64;
		

#else
		const int build = 0;
#endif

		void Start ()
		{
		if ((int)someMask == ((int)someMask | (build << (int)someMask))) {
				var canvas = GetComponent<CanvasGroup> () == null ? this.gameObject.AddComponent<CanvasGroup> () : GetComponent<CanvasGroup> ();
				canvas.alpha = 0;
				canvas.interactable = false;
				canvas.blocksRaycasts = false;
			}
		}
	}
}
