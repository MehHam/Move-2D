using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	/// <summary>
	/// Displays the GameOver text when the GameManager is at its GameOver state
	/// </summary>
	public class GameOverTextUI : MonoBehaviour
	{
		void Update ()
		{
			this.GetComponent<CanvasGroup> ().alpha = (GameManager.singleton != null && GameManager.singleton.isGameOver) ? 1 : 0;
		}
	}
}
