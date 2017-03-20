using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Move2D
{
	/// <summary>
	/// A movement module manages the accelerometer type of movement for the player
	/// </summary>
	public class PlayerAccelerometerMove : NetworkBehaviour, IPlayerMotion
	{
		private Vector2 gyROT = new Vector2 (0, 0);
		public Gyroscope gyro;
		public int sliderValue = 0;

		public bool IsActivated (int sliderValue)
		{
			return (this.sliderValue == sliderValue);
		}

		public void Move ()
		{
			// Only the local player should call this method
			if (!isLocalPlayer)
				return;
			if (SystemInfo.supportsGyroscope) {

				// gyro = Input.gyro;
				//gyro.enabled = true;
				// this.GetComponent<Rigidbody2D>().AddForce (gyROT, ForceMode2D.Force);
			}
		}
	}
}