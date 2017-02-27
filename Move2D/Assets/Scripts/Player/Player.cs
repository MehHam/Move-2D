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
	/// <summary>
	/// Name of the player
	/// </summary>
	[Tooltip("Name of the player")]
	public string name;
	/// <summary>
	/// Mass of the player, used to compute the sphere position
	/// </summary>
	[Tooltip("Mass of the player, used to compute the sphere position")]
	[SyncVar] public Color color;
	/// <summary>
	/// Color of the player
	/// </summary>
	[Tooltip("Color of the player")]
	[SyncVar] public float mass;

	/// <summary>
	/// The player controller identifier.
	/// </summary>
	[HideInInspector]
	public short playerControllerId;
}

/// <summary>
/// Class used to keep basic informations about the player and manage its behaviour when it's disconnected
/// </summary>
[RequireComponent(typeof(PlayerMoveManager))]
public class Player : NetworkBehaviour {
	/// <summary>
	/// All the informations about this player
	/// </summary>
	[SyncVar] public PlayerInfo playerInfo = new PlayerInfo();

	/// <summary>
	/// Name of the player
	/// </summary>
	public string playerName {
		get { return playerInfo.name; }
		set { playerInfo.name = value; }
	}
	/// <summary>
	/// Mass of the player, used to compute the sphere position
	/// </summary>
	public float mass {
		get { return playerInfo.mass; }
		set { playerInfo.mass = value; }
	}

	/// <summary>
	/// Color of the player
	/// </summary>
	public Color color {
		get { return playerInfo.color; }
		set { playerInfo.color = value; }
	}
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

	void Awake() {
		playerInfo.mass = 1.0f;
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

	void OnClientDisconnect(NetworkConnection conn) {
		if (conn == this.connectionToClient)
			NetworkServer.Destroy (gameObject);
	}
}
