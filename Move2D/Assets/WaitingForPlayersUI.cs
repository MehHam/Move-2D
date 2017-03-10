using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForPlayersUI : MonoBehaviour {
	void Update()
	{
		this.GetComponent<CanvasGroup> ().alpha =
			(GameManager.singleton != null && GameManager.singleton.gameState == GameState.WaitingForPlayers) ?
			1 :
			0;
	}
}
