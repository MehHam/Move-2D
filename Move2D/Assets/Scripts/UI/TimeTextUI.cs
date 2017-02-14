using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeTextUI : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		if (GameManager.singleton == null)
			this.GetComponent<Text> ().text = "/";
		else {
			string formattedTime = (GameManager.singleton.time / 60).ToString ("00") + ":" + (GameManager.singleton.time % 60).ToString ("00");
			this.GetComponent<Text> ().text = formattedTime;
		}
	}
}
