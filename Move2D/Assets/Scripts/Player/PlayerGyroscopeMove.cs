using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// A movement module manages the gyroscopic type of movement for the player
/// </summary>
[RequireComponent (typeof (Rigidbody2D))]
class PlayerGyroscopeMove : NetworkBehaviour, IPlayerMotion
{
	public Gyroscope gyro;
	public int sliderValue = 0;

	public bool IsActivated (int sliderValue)
	{
		return (this.sliderValue == sliderValue);
	}

	public void Move ()
	{
		return;
		// Only the local player should call this method
		if (!isLocalPlayer)
			return;
		// You can only move if the gyroscope is supporter
		if (SystemInfo.supportsGyroscope) {
			Debug.LogError ("Supports Gyroscope");
			float radiusR = 15.7f;
			float alpha = 0f;
			gyro = Input.gyro;
			float theta = Mathf.Atan (gyro.gravity.x / gyro.gravity.y);
			gyro.enabled = true;

			if (theta < Mathf.PI / 2 && theta > -Mathf.PI / 2) {
				alpha = 2 * theta;
			}
			Vector2 playerPos = new Vector2 (radiusR * Mathf.Cos (alpha), radiusR * Mathf.Sin (alpha));
			if (playerPos.magnitude > radiusR * 0.95f && playerPos.magnitude < radiusR * 1.05f) {
				this.GetComponent<Rigidbody2D> ().transform.position = Vector2.Lerp (this.GetComponent<Rigidbody2D> ().position, playerPos, 0.25f);
			}
		}
	}
}

