using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	public class WaitingForPlayersUI : MonoBehaviour
	{
		void Update ()
		{
			this.GetComponent<CanvasGroup> ().alpha =
			(GameManager.singleton != null && GameManager.singleton.isWaitingForPlayers) ?
			1 :
			0;
		}
	}
}
