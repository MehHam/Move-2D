using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// All the informations about a Player
/// </summary>
[System.Serializable]
public struct PlayerInfo
{
	[SyncVar] public string name;
	[SyncVar] public Color color;
	[SyncVar] public float mass;
}

/// <summary>
/// Class used to keep basic informations about the player and manage its behaviour when it's disconnected
/// </summary>
[RequireComponent(typeof(PlayerMoveManager))]
public class Player : NetworkBehaviour {
	
	[SyncVar]
	/// <summary>
	/// All the informations about this player
	/// </summary>
	public PlayerInfo playerInfo;
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
		GameManager.onClientSceneChanged += OnClientSceneChanged;
	}

	void OnDisable()
	{
		CustomNetworkLobbyManager.onClientDisconnect -= OnClientDisconnect;
		GameManager.onClientSceneChanged -= OnClientSceneChanged;
	}
		
	void OnClientSceneChanged (NetworkConnection conn)
	{
	}

	[ClientRpc]
	public void RpcSetPosition(Vector2 position)
	{
		Debug.Log ("RpcSetPosition called : " + position);
		this.GetComponent<Rigidbody2D> ().position = position;
	}

	/// <summary>
	/// Call the server to change the mass of the player
	/// </summary>
	/// <param name="value">Value.</param>
	[Command]
	public void CmdSetMass(float value)
	{
		mass = value;
	}

	[Command]
	/// <summary>
	/// Call the server to respawn the player
	/// </summary>
	public void CmdRespawn(){
		var transform = NetworkManager.singleton.GetStartPosition ();
		var spawnPoint = transform == null ?
			new Vector3 (DynamicStartPositions.spawnRadius, DynamicStartPositions.spawnRadius) :
			transform.position;
		var player = (GameObject)Instantiate (CustomNetworkLobbyManager.singleton.playerPrefab, spawnPoint, Quaternion.identity);
		player.GetComponent<Player> ().playerName = this.playerName;
		player.GetComponent<Player> ().mass = this.mass;
		player.GetComponent<Player> ().color = this.color;
		NetworkServer.Destroy (this.gameObject);
		NetworkServer.ReplacePlayerForConnection (this.connectionToClient, player, this.playerControllerId);
	}

	// Use this for initialization
	void Start () {
		this.GetComponent<Renderer> ().material.color = color;
		//DontDestroyOnLoad (this.gameObject);
	}

	void Update() {
		this.transform.localScale = new Vector3 (mass, mass, mass);
		this.GetComponent<Renderer> ().material.color = color;
	}

	void OnClientDisconnect(NetworkMessage msg) {
		NetworkServer.Destroy (gameObject);
	}
}
