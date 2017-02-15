using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

//Adding this allows us to access members of the UI namespace including Text.
using UnityEngine.UI;

[RequireComponent(typeof(PlayerKeyboardMove))]
[RequireComponent(typeof(PlayerDragMove))]
[RequireComponent(typeof(PlayerAccelerometerMove))]
[RequireComponent(typeof(PlayerGyroscopeMove))]
public class PlayerMoveManager : NetworkBehaviour
{

	void FixedUpdate ()
	{
		if (!isLocalPlayer || (GameManager.singleton != null && GameManager.singleton.paused))
			return;
		if (GameManager.singleton != null && GameManager.singleton.slider != null) {
			var slider = GameManager.singleton.slider;
			foreach (var playerMotion in gameObject.GetComponents<IPlayerMotion>()) {
				if (playerMotion.IsActivated ((int)slider.value))
					playerMotion.Move ();
			}
		}
	}
}
