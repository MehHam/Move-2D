using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Move2D
{
	/// <summary>
	/// Displays the GameOver text when the GameManager is at its GameOver state
	/// </summary>
	public class GameOverTextUI : MonoBehaviour
	{
		public Button restartButton;
		public Button giveUpButton;

		void Awake()
		{
			giveUpButton.onClick.AddListener(OnGiveUpClick);
			restartButton.onClick.AddListener (OnReadyToStartClick);
		}

		void OnReadyToStartClick()
		{
			Debug.Log ("OnReadyToStartClick");
			GameManager.singleton.OnStartClient ();
			restartButton.onClick.RemoveAllListeners ();
			restartButton.interactable = false;
			restartButton.GetComponentInChildren<Text> ().text = "Waiting for players...";
		}

		void OnGiveUpClick()
		{
			((CustomNetworkLobbyManager)(CustomNetworkLobbyManager.singleton)).errorMessage = NetworkErrorMessage.ClientLeft;
			CustomNetworkLobbyManager.s_Singleton.GoBackButton();
		}

		void Update ()
		{
			bool isGameOver = (GameManager.singleton != null && GameManager.singleton.isGameOver);
			this.GetComponent<CanvasGroup> ().alpha = isGameOver ? 1 : 0;
			this.GetComponent<CanvasGroup> ().interactable = isGameOver;
			this.GetComponent<CanvasGroup> ().blocksRaycasts = isGameOver;
			this.GetComponent<CanvasGroup> ().ignoreParentGroups = isGameOver;
		}
	}
}
