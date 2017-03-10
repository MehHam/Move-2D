using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySettingsUI : MonoBehaviour {
	void Start()
	{
		this.GetComponent<Dropdown> ().value = (int)(GameManager.singleton.difficulty);
		this.GetComponent<Dropdown> ().onValueChanged.AddListener (delegate {
			OnValueChanged ();
		});
	}

	void Update()
	{
		this.GetComponent<Dropdown> ().interactable = GameManager.singleton.isPlaying;
	}

	public void OnValueChanged()
	{
		GameManager.singleton.ChangeDifficulty ((GameManager.Difficulty)this.GetComponent<Dropdown>().value);
	}
}
