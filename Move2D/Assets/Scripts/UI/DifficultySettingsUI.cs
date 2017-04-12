using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Move2D
{
	public class DifficultySettingsUI : MonoBehaviour
	{
		void OnEnable()
		{
			GameManager.onLevelStarted += OnLevelStarted;
		}

		void OnLevelStarted ()
		{
			if (GameManager.singleton.isServer)
			{
				this.GetComponent<Dropdown> ().value = (int)(GameManager.singleton.difficulty);
				this.GetComponent<CanvasGroup> ().alpha = 1;
				this.GetComponent<CanvasGroup> ().interactable = true;
				this.GetComponent<CanvasGroup> ().blocksRaycasts = true;
				this.GetComponent<Dropdown> ().onValueChanged.AddListener (delegate {
					OnValueChanged ();
				});
			}
			else
			{
				this.GetComponent<CanvasGroup>().alpha = 0;
				this.GetComponent<CanvasGroup>().interactable = false;
				this.GetComponent<CanvasGroup>().blocksRaycasts = false;
			}
		}

		void Start ()
		{
			this.GetComponent<CanvasGroup>().alpha = 0;
			this.GetComponent<CanvasGroup>().interactable = false;
			this.GetComponent<CanvasGroup>().blocksRaycasts = false;
		}

		void OnDisable()
		{
			GameManager.onLevelStarted -= OnLevelStarted;
		}

		void Update ()
		{
			this.GetComponent<Dropdown> ().interactable = GameManager.singleton.isPlaying;
		}

		public void OnValueChanged ()
		{
			GameManager.singleton.ChangeDifficulty ((GameManager.Difficulty)this.GetComponent<Dropdown> ().value);
		}
	}
}