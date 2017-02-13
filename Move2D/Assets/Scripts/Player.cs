using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerMoveManager))]
public class Player : NetworkBehaviour {
	public string playerName;
	public float mass = 1.0f;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this);
	}

	void OnPlayerDisconnected(NetworkPlayer player) {
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
}
