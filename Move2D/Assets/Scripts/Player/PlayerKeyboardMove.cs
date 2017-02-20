using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// A movement module manages the keyboard type of movement for the player
/// </summary>
[RequireComponent (typeof (Rigidbody2D))]
class PlayerKeyboardMove : NetworkBehaviour, IPlayerMotion
{
	public float speed = 0.0f;
	public int sliderValue = 0;

	public bool IsActivated (int sliderValue)
	{
		return (this.sliderValue == sliderValue);
	}

	public void Move() {
		// Only the local player should call this method
		if (!isLocalPlayer)
			return;
		//Store the current horizontal input in the float moveHorizontal.
		float moveHorizontal = Input.GetAxis ("Horizontal");

		//Store the current vertical input in the float moveVertical.
		float moveVertical = Input.GetAxis ("Vertical");

		//Use the two store floats to create a new Vector2 variable movement.
		Vector2 movement = new Vector2 (moveHorizontal, moveVertical);
		//Vector3 movement = new Vector3 (moveHorizontal, moveVertical,0);
		//Debug.Log(movement);
		//Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
		this.GetComponent<Rigidbody2D>().AddForce (movement * speed);
	}
}

