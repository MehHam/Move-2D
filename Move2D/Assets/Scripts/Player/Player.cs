using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Class used to keep basic informations about the player and manage its behaviour when it's disconnected
/// </summary>
[RequireComponent(typeof(PlayerMoveManager))]
public class Player : NetworkBehaviour {
	/// <summary>
	/// Name of the player
	/// </summary>
	[Tooltip("Name of the player")]
	[SyncVar] public string playerName;
	/// <summary>
	/// Mass of the player, used to compute the sphere position
	/// </summary>
	[Tooltip("Mass of the player, used to compute the sphere position")]
	[SyncVar] public float mass = 1.0f;
	/// <summary>
	/// Color of the player
	/// </summary>
	[Tooltip("Color of the player")]
	[SyncVar] public Color color;
	/// <summary>
	/// Is this player the second player ?
	/// </summary>
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
