using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Move2D
{
	public class VictoryTextUI : MonoBehaviour
	{
		/// <summary>
		/// The text component of the victory score
		/// </summary>
		public Text score;
		/// <summary>
		/// The text component of the victory difficulty
		/// </summary>
		public Text difficulty;
		/// <summary>
		/// The text component of the total time
		/// </summary>
		public Text totalTime;
		/// <summary>
		/// The back button component
		/// </summary>
		public Button backButton;

		/// <summary>
		/// Base text that will be displayed before the difficulty text.
		/// </summary>
		public string difficultyBaseText = "You finished all the ";
		/// <summary>
		/// Base text that will be displayed before the score text.
		/// </summary>
		public string scoreBaseText = "Your final score : ";
		/// <summary>
		/// Base text that will be displayed before the time text.
		/// </summary>
		public string totalTimeBaseText = "Time : ";

		void Awake()
		{
			backButton.onClick.AddListener (delegate {
				((CustomNetworkLobbyManager)(CustomNetworkLobbyManager.singleton)).errorMessage = NetworkErrorMessage.ClientLeft;
				CustomNetworkLobbyManager.s_Singleton.GoBackButton();
			});
		}

		void Update()
		{
			if (GameManager.singleton != null)
			{
				bool isVictory = (GameManager.singleton != null && GameManager.singleton.isVictory);
				this.GetComponent<CanvasGroup> ().alpha = isVictory ? 1 : 0;
				score.text = scoreBaseText
					+ GameManager.singleton.score;
				difficulty.text = difficultyBaseText
					+ GameManager.singleton.difficulty.ToStringColor ()
					+ " levels.";
				totalTime.text = totalTimeBaseText
					+ (GameManager.singleton.totalTime / 60).ToString ("00")
					+ ":"
					+ (GameManager.singleton.totalTime % 60).ToString ("00");
				this.GetComponent<CanvasGroup> ().interactable = isVictory;
				this.GetComponent<CanvasGroup> ().blocksRaycasts = isVictory;
				this.GetComponent<CanvasGroup> ().ignoreParentGroups = isVictory;
			}
		}
	}
}
