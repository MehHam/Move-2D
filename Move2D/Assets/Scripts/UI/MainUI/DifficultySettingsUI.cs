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
			GameManager.onClientDisconnect += OnClientDisconnect;
		}
			
		void OnDisable()
		{
			GameManager.onLevelStarted -= OnLevelStarted;
			GameManager.onClientDisconnect -= OnClientDisconnect;
		}

		void OnLevelStarted ()
		{
			bool isMainPlayer = false;
			foreach (var player in GameObject.FindObjectsOfType<Player>()) {
				if (player.isLocalPlayer && player.playerInfo.isMainPlayer)
					isMainPlayer = true;
			}
			if (isMainPlayer || GameManager.singleton.isServer)
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
			
		void OnClientDisconnect (NetworkConnection conn)
		{
			// In case the main player changed
			OnLevelStarted ();
		}

		void Start ()
		{
			this.GetComponent<CanvasGroup>().alpha = 0;
			this.GetComponent<CanvasGroup>().interactable = false;
			this.GetComponent<CanvasGroup>().blocksRaycasts = false;
		}

		void Update ()
		{
			this.GetComponent<Dropdown> ().interactable = GameManager.singleton.isPlaying;
		}

		public void OnValueChanged ()
		{
			var difficulty = (GameManager.Difficulty)this.GetComponent<Dropdown> ().value;
			if (GameManager.singleton.isServer)
				GameManager.singleton.ChangeDifficulty (difficulty);
			else {
				foreach (var player in GameObject.FindObjectsOfType<Player>()) {
					if (player.isLocalPlayer) {
						player.CmdChangeDifficulty (difficulty);
						break;
					}
				}
			}
		}
	}
}