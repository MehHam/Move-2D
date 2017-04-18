using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	public enum BuildType
	{
		None = 0x00,
		Any = 0xFF,
		WebGL = 0x01,
		Mac = 0x02,
		Editor = 0x04,
		Windows = 0x08,
		Linux = 0x10,
		IOS = 0x20,
		Android = 0xFF,
	}

	public class HideIfBuildUI : MonoBehaviour
	{

		[BitMask (typeof(BuildType))] public BuildType someMask;
#if UNITY_EDITOR
		const int build = 0x04;

#elif UNITY_WEBGL
		const int build = 0x01;
		

#elif UNITY_STANDALONE_OSX
		const int build = 0x02;
		

#elif UNITY_STANDALONE_WIN
		const int build = 0x04;
		

#elif UNITY_STANDALONE_LINUX
		const int build = 0x08;
		

#elif UNITY_IOS
		const int build = 0x10;
		

#elif UNITY_ANDROID
		const int build = 0x20;
		

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
