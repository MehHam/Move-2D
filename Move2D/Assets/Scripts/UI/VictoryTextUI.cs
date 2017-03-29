using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Move2D
{
	public class VictoryTextUI : MonoBehaviour
	{
		public Text score;
		public Text difficulty;
		public Button backButton;

		public string difficultyBaseText = "You finished all the ";
		public string scoreBaseText = "Your final score : ";

		void Awake()
		{
			backButton.onClick.AddListener (delegate {
				CustomNetworkLobbyManager.s_Singleton.GoBackButton();
			});
		}

		void Update()
		{
			if (GameManager.singleton != null)
			{
				bool isVictory = (GameManager.singleton != null && GameManager.singleton.isVictory);
				this.GetComponent<CanvasGroup> ().alpha = isVictory ? 1 : 0;
				score.text = scoreBaseText + GameManager.singleton.score;
				difficulty.text = difficultyBaseText + GameManager.singleton.difficulty.ToStringColor () + " levels.";
				this.GetComponent<CanvasGroup> ().interactable = isVictory;
				this.GetComponent<CanvasGroup> ().blocksRaycasts = isVictory;
				this.GetComponent<CanvasGroup> ().ignoreParentGroups = isVictory;
			}
		}
	}
}
