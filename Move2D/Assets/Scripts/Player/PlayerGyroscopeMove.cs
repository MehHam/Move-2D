using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

namespace Move2D
{
	/// <summary>
	/// A movement module manages the gyroscopic type of movement for the player
	/// </summary>
	class PlayerGyroscopeMove : MonoBehaviour, IPlayerMotion
	{
		public Gyroscope gyro;
		public float speed = 5.0f;
		public int sliderValue = 0;

		public bool IsActivated (int sliderValue)
		{
			return (this.sliderValue == sliderValue);
		}

		public bool Move ()
		{
			// You can only move if the gyroscope is supporter
			if (SystemInfo.supportsGyroscope) {
				//float radiusR = 15.7f;
				float alpha = 0f;
				gyro = Input.gyro;
				float theta = Mathf.Atan (gyro.gravity.x / gyro.gravity.y);
				gyro.enabled = true;

				if (theta < Mathf.PI / 2 && theta > -Mathf.PI / 2) {
					alpha = 2 * theta;
				/*
					}
					Vector2 playerPos = new Vector2 (radiusR * Mathf.Cos (alpha), radiusR * Mathf.Sin (alpha));
				if (playerPos.magnitude > radiusR * 0.95f && playerPos.magnitude < radiusR * 1.05f) {
					/* this.transform.Rotate (
						Vector3.forward * -Input.gyro.attitude.z
						* Time.deltaTime * speed
					);*/
					var angle = this.transform.rotation.eulerAngles;
					angle.z += (theta / (Mathf.PI / 2.0f)) * speed;
					this.transform.eulerAngles = angle;
					return true;
				}
			}
			return false;
		}
	}
}