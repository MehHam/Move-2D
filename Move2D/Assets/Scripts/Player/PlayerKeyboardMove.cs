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
			this.transform.Rotate (
				(Vector3.forward * Input.GetAxis ("Horizontal")
				+ Vector3.forward * Input.GetAxis ("Vertical"))
				* Time.deltaTime * speed
			);
		}
	}
}
