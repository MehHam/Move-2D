﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Class that manages all the possible type of movement for the player and call each of them.
/// </summary>
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
		// Only the local player should call this method
		if (!isLocalPlayer)
			return;
		// The player can't move if the game is paused
		if (GameManager.singleton != null && !GameManager.singleton.paused) {
			foreach (var playerMotion in gameObject.GetComponents<IPlayerMotion>()) {
				playerMotion.Move ();
			}
		}
	}
}
