using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

namespace Move2D
{
	/// <summary>
	/// A movement module manages the keyboard type of movement for the player
	/// </summary>
	class PlayerKeyboardMove : MonoBehaviour, IPlayerMotion
	{
		/// <summary>
		/// The movement speed of the player
		/// </summary>
		[Tooltip ("The movement speed of the player")]
		public float speed = 50.0f;

		public int sliderValue = 0;

		public bool IsActivated (int sliderValue)
		{
			return (this.sliderValue == sliderValue);
		}

		public void Move ()
		{
			float horizontal = 0.0f;
			float vertical = 0.0f;
			if (this.transform.rotation.eulerAngles.z >= 267.5f || this.transform.rotation.eulerAngles.z <= 92.5f) {
				horizontal -= Input.GetAxis ("Horizontal");
			}
			if (this.transform.rotation.eulerAngles.z <= 272.5f && this.transform.rotation.eulerAngles.z >= 87.5f) {
				horizontal += Input.GetAxis ("Horizontal");
			}
			if (this.transform.rotation.eulerAngles.z >= 177.5f || this.transform.rotation.eulerAngles.z <= 2.5f) {
				vertical += Input.GetAxis ("Vertical");
			}
			if (this.transform.rotation.eulerAngles.z <= 182.5f || this.transform.rotation.eulerAngles.z >= 357.5f) {
				vertical -= Input.GetAxis ("Vertical");
			}
			this.transform.Rotate (
				(Vector3.forward * horizontal
				+ Vector3.forward * vertical)
				* Time.deltaTime * speed
			);
		}
	}
}
