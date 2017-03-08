using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// A movement module manages the keyboard type of movement for the player
/// </summary>
class PlayerKeyboardMove : NetworkBehaviour, IPlayerMotion
{
	/// <summary>
	/// The movement speed of the player
	/// </summary>
	[Tooltip("The movement speed of the player")]
	public float speed = 0.0f;

	float _moveHorizontal = 0.0f;
	float moveVertical = 270.0f;
	public int sliderValue = 0;

	public bool IsActivated (int sliderValue)
	{
		return (this.sliderValue == sliderValue);
	}

	public override void OnStartLocalPlayer() {
		var dir = Vector2.zero - (Vector2)this.transform.position;
		float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
		_moveHorizontal = angle;
		base.OnStartLocalPlayer ();
	}

	public void Move() {
		// Only the local player should call this method
		if (!isLocalPlayer)
			return;
		//Store the current horizontal input in the float moveHorizontal.
		_moveHorizontal -= Input.GetAxis ("Horizontal") * speed * 0.02f;

		//Store the current vertical input in the float moveVertical.
		// moveVertical += Input.GetAxis ("Vertical") * speed * 0.02f;

		//moveVertical = ClampAngle(moveVertical, -723.0f, 877.0f);

		//Use the two store floats to create a new Vector2 variable movement.
		//Vector3 movement = new Vector3 (moveHorizontal, moveVertical,0);
		//Debug.Log(movement);
		//Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.

		Quaternion rotation = Quaternion.Euler(_moveHorizontal, moveVertical, 0.0f);
		Vector3 position = rotation * new Vector3 (0.0f, 0.0f, 16.0f);

		this.transform.position = position;
	}
}

