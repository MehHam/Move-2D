using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DifficultyTextUI : MonoBehaviour {
	/// <summary>
	/// The base text to be displayed
	/// </summary>
	[Tooltip("The base text to be displayed")]
	public string baseText = "Difficulty Level:";
	// Update is called once per frame
	void Update () {
		this.GetComponent<Text> ().text = baseText + " " + ((GameManager.singleton == null) ? "/" : GameManager.singleton.difficulty.ToString());
	}
}
