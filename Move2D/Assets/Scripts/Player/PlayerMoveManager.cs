using UnityEngine;
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
public class PlayerMoveManager : NetworkBehaviour
{
	public float positionUpdateRate = 0.2f;
	public float smoothRatio = 15.0f;
	public bool sendPosition = true;

	Vector3 _playerPosition;

	public override void OnStartLocalPlayer ()
	{
		StartCoroutine (UpdatePosition());
		base.OnStartLocalPlayer ();
	}

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
		//this.GetComponent<Rigidbody2D> ().isKinematic = (GameManager.singleton != null && GameManager.singleton.paused);
	}

	void Update()
	{
		LerpPosition ();
	}

	void LerpPosition() {
		if (isLocalPlayer)
			return;
		this.transform.position = Vector3.Lerp (this.transform.position, _playerPosition, Time.deltaTime * smoothRatio);
	}

	IEnumerator UpdatePosition() {
		while (enabled) {
			if (sendPosition)
				CmdSendPosition (this.transform.position);
			yield return new WaitForSeconds (positionUpdateRate);
		}
	}

	[Command]
	void CmdSendPosition(Vector3 position) {
		_playerPosition = position;
		RpcReceivePosition (position);
	}

	[ClientRpc]
	void RpcReceivePosition(Vector3 position) {
		_playerPosition = position;
	}
		
}
