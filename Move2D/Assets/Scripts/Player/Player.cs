using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Move2D {
/// <summary>
/// All the informations about a Player
/// </summary>
[System.Serializable]
public class PlayerInfo
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
public class Player : NetworkBehaviour, IPlayerLineObject {
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

	public GameObject moveController;

	public float positionUpdateRate = 0.2f;
	public float smoothRatio = 15.0f;

	Vector3 _playerPosition;

	public delegate void PlayerDestroyHandler(Player player);
	public static event PlayerDestroyHandler onPlayerDestroy;

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

	[Command]
	public void CmdTryGuess()
	{
		GameObject.FindObjectOfType<CenterOfMassValidatorUI> ().ServerGuess ();
	}

	// Use this for initialization
	public override void OnStartLocalPlayer ()
	{
		this.GetComponent<Renderer> ().material.color = color;
		if (isLocalPlayer)
		{
			var diff = this.transform.position - Vector3.zero;
			var direction = diff / diff.magnitude;
			var moveController = GameObject.Instantiate (this.moveController,
				Vector3.zero,
				Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f)
			);
			this.transform.SetParent (moveController.transform);
			_playerPosition = this.transform.position;
			//CmdSpawnMoveController ();
			//StartCoroutine (UpdatePosition());
		}
		base.OnStartLocalPlayer ();
	}

	[Command]
	void CmdSpawnMoveController()
	{
		var moveController = GameObject.Instantiate (this.moveController, Vector3.zero, Quaternion.identity);
		this.transform.SetParent (moveController.transform);
		NetworkServer.SpawnWithClientAuthority (moveController, connectionToClient);
		RpcSetMoveControllerAsParent (moveController);
	}

	[ClientRpc]
	void RpcSetMoveControllerAsParent(GameObject moveController)
	{
		if (isLocalPlayer) {
			this.transform.SetParent (moveController.transform);
		}
	}

	void Update() {
		this.transform.localScale = new Vector3 (mass, mass, mass);
		this.GetComponent<Renderer> ().material.color = color;
		//LerpPosition ();
	}

	void OnClientDisconnect(NetworkConnection conn) {
		if (conn == this.connectionToClient)
			NetworkServer.Destroy (gameObject);
	}

	void LerpPosition() {
		if (isLocalPlayer)
			return;
		this.transform.localPosition = Vector3.Lerp (this.transform.position, _playerPosition, Time.deltaTime * smoothRatio);
	}

	IEnumerator UpdatePosition() {
		while (enabled) {
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

	void OnDestroy()
	{
		if (onPlayerDestroy != null)
			onPlayerDestroy (this);		
	}

		#region IPlayerLineObject implementation

		public Color GetColor ()
		{
			return this.playerInfo.color;
		}

		public float GetMass ()
		{
			return this.playerInfo.mass;
		}

		#endregion
}
}
