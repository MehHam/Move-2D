using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that dynamically change a text file to display the time in the current gameManager
/// </summary>
[RequireComponent(typeof(Text))]
public class TimeTextUI : MonoBehaviour {
	/// <summary>
	/// The base text.
	/// </summary>
	[Tooltip("The base text to be displayed")]
	public string baseText = "Time:";
	// Update is called once per frame
	void Update () {
		if (GameManager.singleton == null)
			this.GetComponent<Text> ().text = baseText + " " + "/";
		else {
			string formattedTime = baseText + " " + (GameManager.singleton.time / 60).ToString ("00") + ":" + (GameManager.singleton.time % 60).ToString ("00");
			this.GetComponent<Text> ().text = formattedTime;
		}
	}
}
