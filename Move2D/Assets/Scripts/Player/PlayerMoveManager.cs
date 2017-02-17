using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

//Adding this allows us to access members of the UI namespace including Text.
using UnityEngine.UI;

public enum MotionMode
{
	Keyboard,
	Accelerometer,
	Gyroscope,
	Drag,
}
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
		if (GameManager.singleton != null) {
			foreach (var playerMotion in gameObject.GetComponents<IPlayerMotion>()) {
				playerMotion.Move ();
			}
		}
	}
}
