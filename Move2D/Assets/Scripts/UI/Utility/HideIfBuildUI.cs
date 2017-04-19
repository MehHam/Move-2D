using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	public enum BuildType
	{
		None = 0x00,
		Any = 0xFF,
		WebGL = 1 << 0,
		Mac = 1 << 1,
		Editor = 1 << 2,
		Windows = 1 << 3,
		Linux = 1 << 4,
		IOS = 1 << 5,
		Android = 1 << 6,
	}

	public class HideIfBuildUI : MonoBehaviour
	{

		[BitMask (typeof(BuildType))] public BuildType someMask;
#if UNITY_EDITOR
		const int build = 1 << 2;

#elif UNITY_WEBGL
		const int build = 1 << 0;
		

#elif UNITY_STANDALONE_OSX
		const int build = 1 << 1;
		

#elif UNITY_STANDALONE_WIN
		const int build = 1 << 3;
		

#elif UNITY_STANDALONE_LINUX
		const int build = 1 << 4;
		

#elif UNITY_IOS
		const int build = 1 << 5;
		

#elif UNITY_ANDROID
		const int build = 1 << 6;
		

#else
		const int build = 0x00;
#endif

		void Start ()
		{
			Debug.Log (build);
		if ((int)someMask == ((int)someMask | build)) {
				var canvas = GetComponent<CanvasGroup> () == null ? this.gameObject.AddComponent<CanvasGroup> () : GetComponent<CanvasGroup> ();
				canvas.alpha = 0;
				canvas.interactable = false;
				canvas.blocksRaycasts = false;
			}
		}
	}
}
