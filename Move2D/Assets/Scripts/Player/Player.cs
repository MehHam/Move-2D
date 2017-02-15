using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerMoveManager))]
public class Player : NetworkBehaviour {
	[SyncVar] public string playerName;
	[SyncVar] public float mass = 1.0f;
	[SyncVar] public Color color;

	void OnEnable()
	{
		CustomNetworkLobbyManager.onClientDisconnect += OnClientDisconnect;
	}

	void OnDisable()
	{
		CustomNetworkLobbyManager.onClientDisconnect -= OnClientDisconnect;
	}

	// Use this for initialization
	void Start () {
		this.GetComponent<Renderer> ().material.color = color;
		DontDestroyOnLoad (this);
	}

	void OnClientDisconnect(NetworkMessage msg) {
		NetworkServer.Destroy (gameObject);
	}
}
