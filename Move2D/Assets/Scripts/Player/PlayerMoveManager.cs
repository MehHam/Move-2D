using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Move2D
{
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

	public class PlayerMoveManager : MonoBehaviour
	{
		public float positionUpdateRate = 0.2f;
		public float smoothRatio = 15.0f;
		public bool sendPosition = true;

		Vector3 _playerPosition;

		void FixedUpdate ()
		{
			// Only the local player should call this method
			// The player can't move if the game is paused
			if (GameManager.singleton != null && GameManager.singleton.gameState == GameState.Playing) {
				foreach (var playerMotion in gameObject.GetComponents<IPlayerMotion>()) {
					playerMotion.Move ();
				}
			}
			//this.GetComponent<Rigidbody2D> ().isKinematic = (GameManager.singleton != null && GameManager.singleton.paused);
		}
		
	}
}
