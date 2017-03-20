using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Move2D
{
	[RequireComponent (typeof(Text))]
	[RequireComponent (typeof(CanvasGroup))]
	public class ExitCountdownTextUI : MonoBehaviour
	{
		private Exit _exit;

		// Use this for initialization
		void Start ()
		{
			this._exit = GetComponentInParent<Exit> ();
		}
	
		// Update is called once per frame
		void OnGUI ()
		{
			this.GetComponent<CanvasGroup> ().alpha = this._exit.countdownActivated ? 1 : 0;
			this.GetComponent<Text> ().text = this._exit.timeLeft.ToString ();
		}
	}
}