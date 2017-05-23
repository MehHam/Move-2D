using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Move2D
{
	public class DifficultySettingsUI : MonoBehaviour
	{
		public Player[] _players;

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
			_players = GameObject.FindObjectsOfType<Player> ();
			foreach (var player in _players) {
				if (player != null && player.isLocalMainPlayer)
					isMainPlayer = true;
			}
			this.GetComponent<Dropdown> ().value = (int)(GameManager.singleton.difficulty);
			this.GetComponent<CanvasGroup> ().alpha = 1;
			this.GetComponent<CanvasGroup> ().interactable = true;
			this.GetComponent<CanvasGroup> ().blocksRaycasts = true;
			if (isMainPlayer || GameManager.singleton.isServer)
			{
				this.GetComponent<Dropdown> ().onValueChanged.AddListener (delegate {
					OnValueChanged ();
				});
			}
			else
			{
				this.GetComponent<Dropdown> ().interactable = false;
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
			bool isMainPlayer = false;
			foreach (var player in _players) {
				if (player != null && player.isLocalMainPlayer)
					isMainPlayer = true;
			}
			this.GetComponent<Dropdown> ().interactable = (isMainPlayer || (GameManager.singleton.isServer)) && GameManager.singleton.isPlaying;
		}

		public void OnValueChanged ()
		{
			var difficulty = (GameManager.Difficulty)this.GetComponent<Dropdown> ().value;
			if (GameManager.singleton.isServer)
				GameManager.singleton.ChangeDifficulty (difficulty);
			else {
				foreach (var player in _players) {
					if (player.isLocalPlayer) {
						player.CmdChangeDifficulty (difficulty);
						break;
					}
				}
			}
		}
	}
}