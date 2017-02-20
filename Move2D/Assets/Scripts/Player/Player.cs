using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Class used to keep basic informations about the player and manage its behaviour when it's disconnected
/// </summary>
[RequireComponent(typeof(PlayerMoveManager))]
public class Player : NetworkBehaviour {
	[SyncVar] public string playerName;
	[SyncVar] public float mass = 1.0f;
	[SyncVar] public Color color;
	public bool player2;

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
