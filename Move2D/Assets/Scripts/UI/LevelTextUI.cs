using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTextUI : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		if (GameManager.singleton == null)
			this.GetComponent<Text> ().text = "Level: /";
		else
			this.GetComponent<Text> ().text = "Level: " + GameManager.singleton.levels [GameManager.singleton.currentLevelIndex].index.ToString();
	}
}
