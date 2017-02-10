using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeTextUI : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		string formattedTime = GameManager.singleton.time / 60 + ":" + GameManager.singleton.time % 60;
		this.GetComponent<Text> ().text = "Time: " + formattedTime;
	}
}
