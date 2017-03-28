using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Move2D
{
	/// <summary>
	/// Class that dynamically change a text file to display the time in the current gameManager
	/// </summary>
	[RequireComponent (typeof(Text))]
	public class TimeTextUI : MonoBehaviour
	{
		/// <summary>
		/// The base text.
		/// </summary>
		[Tooltip ("The base text to be displayed")]
		public string baseText = "Time: ";
		// Update is called once per frame
		void Update ()
		{
			if (GameManager.singleton == null)
				this.GetComponent<Text> ().text = baseText + "/";
			else {
				int time = GameManager.singleton.time;
				string formattedTime = (time / 60).ToString ("00") + ":" + (time % 60).ToString ("00");
				string coloredTime = (GameManager.singleton.isPlaying && time <= 10) ? "<color=red>" + formattedTime + "</color>" : formattedTime;
				this.GetComponent<Text> ().text = baseText + coloredTime;
			}
		}
	}
}
