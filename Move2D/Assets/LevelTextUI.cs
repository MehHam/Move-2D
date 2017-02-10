using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTextUI : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		this.GetComponent<Text> ().text = "Level: " + GameManager.singleton.levels [GameManager.singleton.currentLevelIndex].index.ToString();
	}
}
