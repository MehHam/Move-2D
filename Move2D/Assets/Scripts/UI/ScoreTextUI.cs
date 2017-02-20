using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that dynamically change a text file to display the score in the current gameManager
/// </summary>
[RequireComponent(typeof(Text))]
public class ScoreTextUI : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		this.GetComponent<Text> ().text = (GameManager.singleton == null) ? "/" : GameManager.singleton.score.ToString();
	}
}
