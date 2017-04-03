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
		bool _isHorizontalAxisInUse = false;
		bool _isHorizontalClockWise = false;
		bool _isVerticalAxisInUse = false;
		bool _isVerticalClockWise = false;
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

		public bool Move ()
		{
			float horizontal = Input.GetAxis ("Horizontal") * (this._isHorizontalClockWise ? 1 : -1);
			float vertical = Input.GetAxis ("Vertical") * (this._isVerticalClockWise ? 1 : -1);
			this.transform.Rotate (
				(Vector3.forward * horizontal
				+ Vector3.forward * vertical)
				* Time.deltaTime * speed
			);
			return horizontal != 0 || vertical != 0;
		}

		void SetAxisDown()
		{
			if (Input.GetAxisRaw ("Horizontal") != 0) {
				if (!this._isHorizontalAxisInUse) {
					this._isHorizontalClockWise = !(this.transform.rotation.eulerAngles.z >= 270.0f
						|| this.transform.rotation.eulerAngles.z <= 90.0f);
					this._isHorizontalAxisInUse = true;
				}
			} else
				this._isHorizontalAxisInUse = false;
			if (Input.GetAxisRaw ("Vertical") != 0) {
				if (!this._isVerticalAxisInUse) {
					this._isVerticalClockWise = (this.transform.rotation.eulerAngles.z >= 180.0f);
					this._isVerticalAxisInUse = true;
				}
			} else
				this._isVerticalAxisInUse = false;
		}

		void Update()
		{
			SetAxisDown ();
		}
	}
}
