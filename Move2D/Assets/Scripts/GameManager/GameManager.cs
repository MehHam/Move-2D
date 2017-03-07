using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using Prototype.NetworkLobby;
using System;

/// <summary>
/// Class that manages the current state of the game
/// </summary>
public class GameManager : NetworkBehaviour {
	public delegate void GameManagerEvent();
	public delegate void ClientEvent(NetworkConnection conn);

	public static event ClientEvent onClientSceneChanged;

	/// <summary>
	/// Event called whenever the level started
	/// </summary>
	public static event GameManagerEvent onLevelStarted;

	public enum Difficulty
	{
		Beginner,
		Intermediate,
		Expert,
	}

	/// <summary>
	/// Player information relative to a networkConnection
	/// </summary>
	public class NetworkPlayerInfo
	{
		/// <summary>
		/// Information about a player
		/// </summary>
		public PlayerInfo playerInfo;
		/// <summary>
		/// A network connection
		/// </summary>
		public NetworkConnection networkConnection;

		public NetworkPlayerInfo(PlayerInfo playerInfo, NetworkConnection networkConnection)
		{
			this.playerInfo = playerInfo;
			this.networkConnection = networkConnection;
			this.readyToBegin = false;
		}

		public bool readyToBegin;
	}

	public GameObject motionPointFollow;
	public GameObject sphereCDM;

	bool _isReadyToBegin;
	int _playerReadyToStart = 0;
	Coroutine _timeCoroutine;

	public static GameManager singleton { get; private set; }

	public List<NetworkPlayerInfo> networkPlayersInfo = new List<NetworkPlayerInfo>();

	const short MyBeginMsg = MsgType.Highest + 1;
	const short MyStartMsg = MsgType.Highest + 2;

	public LevelList levelList;

	/// <summary>
	/// Array displaying the data of all the beginner levels that will be played
	/// </summary>
	public Level[] beginnerLevels {
		get {
			return levelList.beginnerLevels;
		}
	}

	/// <summary>
	/// Array displaying the data of all the intermediate levels that will be played
	/// </summary>
	public Level[] intermediateLevels {
		get {
			return levelList.intermediateLevels;
		}
	}

	/// <summary>
	/// Array displaying the data of all the expert levels that will be played
	/// </summary>
	public Level[] expertLevels {
		get {
			return levelList.expertLevels;
		}
	}

	/// <summary>
	/// Gets a value indicating whether this <see cref="GameManager"/> has an invisible sphere.
	/// </summary>
	/// <value><c>true</c> if invisible sphere; otherwise, <c>false</c>.</value>
	public bool invisibleSphere { get { return (this.GetCurrentLevel ().sphereVisibility == Level.SphereVisibility.Invisible ||
		this.GetCurrentLevel ().sphereVisibility == Level.SphereVisibility.FadeAfterStartLevel);
		} }
	
	public const float arenaRadius = 16.0f;

	/// <summary>
	/// After this timeout the server will start the game and kick all clients that didn't send their ready to begin message
	/// </summary>
	public float timeOut = 10.0f;

	/// <summary>
	/// Has the game started or not. Becomes true the first time the time on a level is started.
	/// </summary>
	[Tooltip("Has the game started ?")]
	[SyncVar] public bool gameStarted = false;
	/// <summary>
	/// The difficulty of the game
	/// </summary>
	[Tooltip("The difficulty of the game")]
	[SyncVar] public Difficulty difficulty = Difficulty.Beginner;
	/// <summary>
	/// Whether the game is paused or not.
	/// </summary>
	[Tooltip("Is the game paused ?")]
	[SyncVar] public bool paused = true;
	/// <summary>
	/// The index of the level that is currently played
	/// </summary>
	[Tooltip("Index of the level currently played")]
	[SyncVar] public int currentLevelIndex = 0;
	/// <summary>
	/// How much time is left in the current level
	/// </summary>
	[Tooltip("Time left in the current level")]
	[SyncVar] public uint time = 0;
	/// <summary>
	/// The current score of the players
	/// </summary>
	[Tooltip("Current score of the player")]
	[SyncVar] public int score = 0;

	void OnEnable()
	{
		//NetworkSceneManager.OnClientLevelLoaded += OnLevelFinishedLoading;
		//NetworkSceneManager.OnServerLevelLoaded += OnLevelFinishedLoading;
		CustomNetworkLobbyManager.onServerDisconnect += OnServerDisconnect;
		CustomNetworkLobbyManager.onServerConnect += OnServerConnect;
		//CustomNetworkLobbyManager.onClientSceneLoaded += OnClientSceneLoaded;
		ReadySetGo.onAnimationFinished += OnAnimationFinished;
	}

	void OnDisable()
	{
		//NetworkSceneManager.OnClientLevelLoaded += OnLevelFinishedLoading;
		//NetworkSceneManager.OnServerLevelLoaded += OnLevelFinishedLoading;
		CustomNetworkLobbyManager.onServerConnect -= OnServerConnect;
		CustomNetworkLobbyManager.onServerDisconnect -= OnServerDisconnect;
		//CustomNetworkLobbyManager.onClientSceneLoaded -= OnClientSceneLoaded;
		ReadySetGo.onAnimationFinished -= OnAnimationFinished;
	}

	void Awake()
	{
		if (singleton != null && singleton != this) {
			NetworkServer.Destroy (this.gameObject);
		} else {
			GameManager.singleton = this;
			DontDestroyOnLoad (this.gameObject);
			NetworkServer.RegisterHandler (MyBeginMsg, OnServerStartLevelMessage);
			NetworkServer.RegisterHandler (MyStartMsg, OnServerStartGameMessage);
		}
	}

	void InitPlayerInfo() {
		networkPlayersInfo = new List<NetworkPlayerInfo> ();
		foreach (var player in GameObject.FindGameObjectsWithTag("Player")) {
			networkPlayersInfo.Add (
				new NetworkPlayerInfo(player.GetComponent<Player>().playerInfo,
				player.GetComponent<Player>().connectionToClient)
			);
		}
	}

	void DeactivateWaitingForPlayersUI()
	{
		var waitingForPlayers = GameObject.FindObjectOfType<EllipsisTextUI> ();
		if (waitingForPlayers != null)
			waitingForPlayers.Deactivate ();
	}

	/// <summary>
	/// Returns true if the variable readyToBegin is true for all network player info
	/// </summary>
	/// <returns><c>true</c>, if players are ready, <c>false</c> otherwise.</returns>
	bool AllPlayersReady()
	{
		foreach (var networkPlayerInfo in networkPlayersInfo)
		{
			if (!networkPlayerInfo.readyToBegin)
				return false;
		}
		return _isReadyToBegin;
	}

	/// <summary>
	/// Get the current level data
	/// </summary>
	/// <returns>The current level data</returns>
	public Level GetCurrentLevel()
	{
		return this.GetCurrentLevels () [this.currentLevelIndex];
	}

	/// <summary>
	/// Get the levels for the current difficulty
	/// </summary>
	/// <returns>Get the levels for the current difficulty.</returns>
	public Level[] GetCurrentLevels()
	{
		switch (difficulty) {
		case Difficulty.Beginner:
			return beginnerLevels;
		case Difficulty.Intermediate:
			return intermediateLevels;
		case Difficulty.Expert:
			return expertLevels;
		}
		return null;
	}
		
	void OnDisconnectedFromServer()
	{
		Destroy (gameObject);
	}

	// Called when the ready set go animation is finished
	void OnAnimationFinished()
	{
		ImmediateStartLevel ();
	}

	void OnDestroy()
	{
		StopAllCoroutines ();
	}



	// ----------------- IEnumerator ------------------


	/// <summary>
	/// Waits for all players to be ready, kicks all the clients that aren't after the timeout.
	/// </summary>
	/// <param name="timeOut">The time out time.</param>
	IEnumerator WaitForAllPlayersReady(float timeOut)
	{
		var time = Time.time;
		// Wait until timeout OR both the players and the server are ready
		while ((Time.time < time + timeOut) && !AllPlayersReady()) {
			yield return null;
		}
		var players = FindObjectsOfType<Player> ();
		// Kick all players that aren't ready if timeout
		for (int i = networkPlayersInfo.Count - 1; i >= 0; i--) {
			if (!networkPlayersInfo [i].readyToBegin) {
				foreach (var player in players) {
					if (player.playerControllerId == networkPlayersInfo [i].playerInfo.playerControllerId)
						player.connectionToClient.Disconnect ();
				}
				networkPlayersInfo.RemoveAt (i);
			} else {
				// Then we set the player as not ready to begin for the next time a level is loaded
				networkPlayersInfo [i].readyToBegin = false;
			}
		}
		StartLevel ();
	}
		
	/// <summary>
	/// Converts the time to a score.
	/// </summary>
	IEnumerator ConvertTimeToScore()
	{
		while (this.time > 0) {
			this.time--;
			this.score++;
			yield return null;
		}
		LoadNextLevel ();
	}

	// The time countdown
	[Server]
	IEnumerator DecreaseTime()
	{
		while (time > 0)
		{
			yield return new WaitForSeconds (1.0f);
			time--;
		}
		// If there's a level next, loads the next level when the time is finished
		if (this.currentLevelIndex + 1 < this.GetCurrentLevels().Length) {
			LoadNextLevel ();
		}
	}


	// ----------------- Server ------------------

	// Starts immediately the level, ignore the ready set go animation
	[Server]
	void ImmediateStartLevel ()
	{
		InitPlayerInfo ();
		StopTime ();
		time = this.GetCurrentLevels() [currentLevelIndex].time;
		StartTime ();
		if (this.GetCurrentLevel ().gameMode == Level.GameMode.MotionPointFollow) {
			var motionPointFollow = GameObject.Instantiate (this.motionPointFollow, new Vector3 (0, 4, 0), Quaternion.identity);
			NetworkServer.Spawn (motionPointFollow);
		}
		var sphereCDM = GameObject.Instantiate (this.sphereCDM, Vector3.zero, Quaternion.identity);
		NetworkServer.Spawn (sphereCDM);
		if (onLevelStarted != null)
			onLevelStarted ();
		RpcLevelStarted ();
	}

	// Starts the level
	[Server]
	void StartLevel ()
	{
		var readySetGo = GameObject.FindObjectOfType<ReadySetGo> ();
		DeactivateWaitingForPlayersUI ();
		RpcReadyToBegin ();
		if (GetCurrentLevel ().readyAnimation && readySetGo != null && readySetGo.GetComponent<Animator>() != null) {
			paused = true;
			readySetGo.GetComponent<Animator> ().SetTrigger ("Activation");
			readySetGo.GetComponent<NetworkAnimator>().SetTrigger ("Activation");
		}
		else
			ImmediateStartLevel ();
	}

	/// <summary>
	/// Increase the score
	/// </summary>
	[Server]
	public void IncreaseScore()
	{
		IncreaseScore (1);
	}

	/// <summary>
	/// Increase the score by a value.
	/// The more player there is, the more points you get
	/// </summary>
	/// <param name="value">The value by which the score will be increased.</param>
	[Server]
	public void IncreaseScore(int value)
	{
		this.score += value * Math.Max(1, (networkPlayersInfo.Count - 1));
	}

	/// <summary>
	/// Decrease the score
	/// </summary>
	[Server]
	public void DecreaseScore()
	{
		DecreaseScore (1);
	}

	/// <summary>
	/// Decrease the score by a value
	/// </summary>
	/// <param name="value">The value by which the score will be decreased.</param>
	[Server]
	public void DecreaseScore(int value)
	{
		if (paused)
			return;
		this.score -= value;
		this.score = Mathf.Max (this.score, 0);
	}


	/// <summary>
	/// Loads the next level.
	/// </summary>
	[Server]
	void LoadNextLevel ()
	{
		this.currentLevelIndex++;
		LoadLevel ();
	}

	/// <summary>
	/// Loads the level.
	/// </summary>
	[Server]
	void LoadLevel ()
	{
		var nextSceneName = this.GetCurrentLevels() [this.currentLevelIndex].sceneName;
		StopTime ();
		this.time = this.GetCurrentLevel ().time;
		CustomNetworkLobbyManager.singleton.ServerChangeScene(nextSceneName);
		StartCoroutine (WaitForAllPlayersReady (this.timeOut));
	}

	[Server]
	/// <summary>
	/// Change the difficulty of the game. The first level of that difficulty setting is automatically started.
	/// </summary>
	/// <param name="difficulty">The new difficulty.</param>
	public void ChangeDifficulty(Difficulty difficulty)
	{
		StopAllCoroutines ();
		this.score = 0;
		this.difficulty = difficulty;
		this.currentLevelIndex = 0;
		LoadLevel ();
	}

	/// <summary>
	/// Event handler called on the server when a client disconnects
	/// </summary>
	/// <param name="conn">The network connection of the client.</param>
	[Server]
	void OnServerDisconnect(NetworkConnection conn)
	{
		for (int i = networkPlayersInfo.Count - 1; i >= 0; i--) {
			if (networkPlayersInfo [i].networkConnection == conn)
				networkPlayersInfo.RemoveAt (i);
		}
	}

	/// <summary>
	/// Event handler called on the server when a client connects
	/// </summary>
	/// <param name="conn">The network conneciton of the client.</param>
	[Server]
	void OnServerConnect(NetworkConnection conn)
	{
		InitPlayerInfo ();
	}

	/// <summary>
	/// Event handler called when the scene is changed for the server
	/// </summary>
	[Server]
	public void OnServerSceneChanged()
	{
		// Call this only if this isn't the first time the scene changed
		if (gameStarted) {
			foreach (var networkPlayerInfo in networkPlayersInfo) {
				var startPos = NetworkManager.singleton.GetStartPosition ();
				GameObject gamePlayer;
				if (startPos != null)
					gamePlayer = (GameObject)Instantiate (((LobbyManager)LobbyManager.singleton).gamePlayerPrefab,
						startPos.position,
						startPos.rotation);
				else
					gamePlayer = (GameObject)Instantiate (((LobbyManager)LobbyManager.singleton).gamePlayerPrefab,
						Vector3.zero,
						Quaternion.identity);
				NetworkServer.ReplacePlayerForConnection (networkPlayerInfo.networkConnection,
					gamePlayer,
					networkPlayerInfo.playerInfo.playerControllerId);
				gamePlayer.GetComponent<Player> ().playerInfo = networkPlayerInfo.playerInfo;
				_isReadyToBegin = true;
			}
		}
	}

	/// <summary>
	/// Message handler called when a client is ready to begin the level
	/// </summary>
	/// <param name="netMsg">Network message.</param>
	[Server]
	public void OnServerStartLevelMessage(NetworkMessage netMsg)
	{
		for (int i = 0; i < networkPlayersInfo.Count; i++) {
			if (networkPlayersInfo [i].networkConnection == netMsg.conn)
				networkPlayersInfo [i].readyToBegin = true;
		}
	}

	/// <summary>
	/// Message handler called on the server when a client is ready to start the game
	/// </summary>
	/// <param name="netMsg">Net message.</param>
	[Server]
	public void OnServerStartGameMessage(NetworkMessage netMsg)
	{
		this._playerReadyToStart++;
		int count = 0;
		// Some connections can be null for some reason, so we have to do the count by ourself
		foreach (var connection in NetworkServer.connections) {
			if (connection != null)
				count++;
		}
		// If all clients are ready we can start the level
		if (count == this._playerReadyToStart)
			StartLevel ();
	}

	[Server]
	public void ExitLevel()
	{
		StopTime ();
		StartCoroutine (ConvertTimeToScore());
	}

	/// <summary>
	/// Starts the time countdown
	/// </summary>
	[Server]
	public void StartTime()
	{
		this._timeCoroutine = StartCoroutine(DecreaseTime());
		this.paused = false;
		// If we start the time, it means that the game was started at least once
		gameStarted = true;
	}

	/// <summary>
	/// Stops the time countdown
	/// </summary>
	[Server]
	public void StopTime()
	{
		if (this._timeCoroutine != null)
			StopCoroutine (this._timeCoroutine);
		this.paused = true;
	}



	// ----------------- Client ------------------


	[Client]
	void Start()
	{
		CustomNetworkLobbyManager.singleton.client.Send (MyStartMsg, new EmptyMessage ());
	}

	/// <summary>
	/// Event handler called on the client when its scene changed
	/// </summary>
	/// <param name="conn">The network connection of the client</param>
	public void OnClientSceneChanged(NetworkConnection conn)
	{
		if (onClientSceneChanged != null)
			onClientSceneChanged (conn);
		if (gameStarted)
			conn.Send (MyBeginMsg, new EmptyMessage ());
	}

	/// <summary>
	/// Requests the client to start the level.
	/// </summary>
	[ClientRpc]
	void RpcLevelStarted()
	{
		if (!isServer) {
			if (onLevelStarted != null)
				onLevelStarted ();
		}
	}

	/// <summary>
	/// Request on the client when the server is ready to begin.
	/// </summary> 
	[ClientRpc]
	void RpcReadyToBegin()
	{
		DeactivateWaitingForPlayersUI ();
	}
}
