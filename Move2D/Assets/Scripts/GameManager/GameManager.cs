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

namespace Move2D
{
	/// <summary>
	/// Game state.
	/// </summary>
	public enum GameState
	{
		Inactive,
		WaitingForPlayers,
		ReadyToStartLevel,
		PreLevel,
		WaitingForAnimation,
		LevelStart,
		Playing,
		PreLevelEnd,
		LevelEnd,
		EndGame,
	}

	/// <summary>
	/// Class that manages the current state of the game
	/// </summary>
	public class GameManager : NetworkBehaviour
	{
		public delegate void GameManagerHandler ();

		public delegate void GameManagerScoreHandler (int value);

		public delegate void ClientHandler (NetworkConnection conn);

		public static event ClientHandler onClientSceneChanged;

		/// <summary>
		/// Event called whenever the level started
		/// </summary>
		public static event GameManagerHandler onLevelStarted;
		/// <summary>
		/// Occurs when the score is changed;
		/// </summary>
		public static event GameManagerScoreHandler onScoreChange;

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

			public NetworkPlayerInfo (PlayerInfo playerInfo, NetworkConnection networkConnection)
			{
				this.playerInfo = playerInfo;
				this.networkConnection = networkConnection;
				this.readyToBegin = false;
			}

			public bool readyToBegin;
		}

		public GameObject motionPointFollow;
		public GameObject sphereCDM;
		public GameObject bulletBuilder;

		int _playerReadyToStart = 0;
		bool _readyToStart = false;
		private int _nextLevelIndex = 0;
		float? _startingTime = null;
		Coroutine _timeCoroutine;

		public static GameManager singleton { get; private set; }

		public List<NetworkPlayerInfo> networkPlayersInfo = new List<NetworkPlayerInfo> ();

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
			this.GetCurrentLevel ().sphereVisibility == Level.SphereVisibility.FadeAfterStartLevel); } }

		public const float arenaRadius = 16.0f;

		/// <summary>
		/// After this timeout the server will start the game and kick all clients that didn't send their ready to begin message
		/// </summary>
		public float timeOut = 10.0f;

		public bool isPlaying { get { return this.gameState == GameState.Playing; } }

		/// <summary>
		/// Has the game started or not.
		/// </summary>
		public bool gameStarted { get { return this.gameState != GameState.Inactive; } }

		/// <summary>
		/// The difficulty of the game
		/// </summary>
		[Tooltip ("The difficulty of the game")]
		[SyncVar] public Difficulty difficulty = Difficulty.Beginner;
		/// <summary>
		/// The current game state
		/// </summary>
		[Tooltip ("The current game state")]
		[SyncVar] public GameState gameState = GameState.Inactive;
		/// <summary>
		/// The index of the level that is currently played
		/// </summary>
		[Tooltip ("Index of the level currently played")]
		[SyncVar] public int currentLevelIndex = 0;
		/// <summary>
		/// How much time is left in the current level
		/// </summary>
		[Tooltip ("Time left in the current level")]
		[SyncVar] public int time = 0;
		/// <summary>
		/// The current score of the players
		/// </summary>
		[Tooltip ("Current score of the player")]
		[SyncVar] public int score = 0;



		// ----------------- NetworkBehaviour Events ------------------



		void OnEnable ()
		{
			CustomNetworkLobbyManager.onServerSceneChanged += OnServerSceneChanged;
			CustomNetworkLobbyManager.onClientSceneChanged += OnClientSceneChanged;
			CustomNetworkLobbyManager.onServerDisconnect += OnServerDisconnect;
			CustomNetworkLobbyManager.onServerConnect += OnServerConnect;
			ReadySetGo.onAnimationFinished += OnAnimationFinished;
		}

		void OnDisable ()
		{
			CustomNetworkLobbyManager.onServerSceneChanged -= OnServerSceneChanged;
			CustomNetworkLobbyManager.onClientSceneChanged -= OnClientSceneChanged;
			CustomNetworkLobbyManager.onServerConnect -= OnServerConnect;
			CustomNetworkLobbyManager.onServerDisconnect -= OnServerDisconnect;
			ReadySetGo.onAnimationFinished -= OnAnimationFinished;
		}

		void Awake ()
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

		void Update ()
		{
			if (isServer)
				HandleStateMachine ();
		}

		void OnDestroy ()
		{
			StopAllCoroutines ();
		}

		void OnDisconnectedFromServer ()
		{
			Destroy (gameObject);
		}



		// ----------------- State Machine ------------------



		void Inactive ()
		{
			int count = 0;
			// Some connections can be null for some reason, so we have to do the count by ourself
			foreach (var connection in NetworkServer.connections) {
				if (connection != null)
					count++;
			}
			// If all clients are ready we can start the level
			if (count == this._playerReadyToStart)
				this.gameState = GameState.PreLevel;
		}

		/// <summary>
		/// Called when the game state is WaitingForPlayers.
		/// Waits for all players to be ready, kicks all the clients that aren't after the timeout.
		/// </summary>
		/// <param name="timeOut">The time out time.</param>
		void WaitingForPlayers ()
		{
			if (!_startingTime.HasValue)
				_startingTime = Time.time;
			if ((Time.time < _startingTime.Value + timeOut) && !AllPlayersReady ()) {
				return;
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
			_startingTime = null;
			this.gameState = GameState.PreLevel;
		}

		/// <summary>
		/// Called when the game state is at PreLevel. Do all the pre-level actions.
		/// </summary>
		[Server]
		void PreLevel ()
		{
			var readySetGo = GameObject.FindObjectOfType<ReadySetGo> ();
			if (GetCurrentLevel ().readyAnimation && readySetGo != null && readySetGo.GetComponent<Animator> () != null) {
				readySetGo.GetComponent<Animator> ().SetTrigger ("Activation");
				readySetGo.GetComponent<NetworkAnimator> ().SetTrigger ("Activation");
				this.gameState = GameState.WaitingForAnimation;

			} else
				this.gameState = GameState.LevelStart;
		}

		/// <summary>
		/// Waitings for pre level animation.
		/// </summary>
		[Server]
		void WaitingForAnimation ()
		{
		}

		/// <summary>
		/// Called when the game state is at LevelStart. Start the level
		/// </summary>
		[Server]
		void LevelStart ()
		{
			InitPlayerInfo ();
			StopTime ();
			time = this.GetCurrentLevels () [currentLevelIndex].time;
			StartTime ();
			SpawnNetworkPrefabs ();
			if (onLevelStarted != null)
				onLevelStarted ();
			RpcLevelStarted ();
			this.gameState = GameState.Playing;
		}

		/// <summary>
		/// Called when the game state is Playing.
		/// </summary>
		void Playing ()
		{
		}

		/// <summary>
		/// Called when the game state is PreLevelEnd. Do the pre level end stuff.
		/// </summary>
		[Server]
		void PreLevelEnd ()
		{
			if (this.time > 0) {
				this.time--;
				this.score++;
			} else {
				this._nextLevelIndex = this.currentLevelIndex + 1;
				this.gameState = GameState.LevelEnd;
			}
		}

		/// <summary>
		/// Called when the game state is LevelEnd. End the level and loads the next one.
		/// </summary>
		[Server]
		void LevelEnd ()
		{
			StopAllCoroutines ();
			if (this._nextLevelIndex < this.GetCurrentLevels ().Length) {
				this.currentLevelIndex = this._nextLevelIndex;
				LoadLevel ();
			}
		}

		/// <summary>
		/// Called when the game state is at EndGame.a
		/// </summary>
		[Server]
		void EndGame ()
		{
		}

		/// <summary>
		/// Handles the state machine.
		/// </summary>
		void HandleStateMachine ()
		{
			switch (this.gameState) {
			case GameState.Inactive:
				Inactive ();
				break;
			case GameState.WaitingForPlayers:
				WaitingForPlayers ();
				break;
			case GameState.PreLevel:
				PreLevel ();
				break;
			case GameState.WaitingForAnimation:
				WaitingForAnimation ();
				break;
			case GameState.LevelStart:
				LevelStart ();
				break;
			case GameState.Playing:
				Playing ();
				break;
			case GameState.PreLevelEnd:
				PreLevelEnd ();
				break;
			case GameState.LevelEnd:
				LevelEnd ();
				break;
			case GameState.EndGame:
				EndGame ();
				break;
			default:
				break;
			}
		}



		// ----------------- Handlers ------------------
		


		/// <summary>
		/// Event handler called on the server when a client disconnects
		/// </summary>
		/// <param name="conn">The network connection of the client.</param>
		[Server]
		void OnServerDisconnect (NetworkConnection conn)
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
		void OnServerConnect (NetworkConnection conn)
		{
			InitPlayerInfo ();
		}

		/// <summary>
		/// Event handler called when the scene is changed for the server
		/// </summary>
		[Server]
		void OnServerSceneChanged ()
		{
			if (this.gameState != GameState.Inactive) {
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
					_readyToStart = true;
				}
			}
		}

		/// <summary>
		/// Event handler called on the client when its scene changed
		/// </summary>
		/// <param name="conn">The network connection of the client</param>
		public void OnClientSceneChanged (NetworkConnection conn)
		{
			if (onClientSceneChanged != null)
				onClientSceneChanged (conn);
			if (gameStarted)
				conn.Send (MyBeginMsg, new EmptyMessage ());
		}

		/// <summary>
		/// Message handler called when a client is ready to begin the level
		/// </summary>
		/// <param name="netMsg">Network message.</param>
		[Server]
		void OnServerStartLevelMessage (NetworkMessage netMsg)
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
		void OnServerStartGameMessage (NetworkMessage netMsg)
		{
			this._playerReadyToStart++;
		}
		
		// Called when the ready set go animation is finished
		void OnAnimationFinished ()
		{
			this.gameState = GameState.LevelStart;
		}



		// ----------------- Public Methods ------------------



		/// <summary>
		/// Exits the level.
		/// </summary>
		[Server]
		public void ExitLevel ()
		{
			this._nextLevelIndex = this.currentLevelIndex + 1;
			this.gameState = GameState.PreLevelEnd;
		}

		/// <summary>
		/// Get the current level data
		/// </summary>
		/// <returns>The current level data</returns>
		public Level GetCurrentLevel ()
		{
			return this.GetCurrentLevels () [this.currentLevelIndex];
		}

		/// <summary>
		/// Get the levels for the current difficulty
		/// </summary>
		/// <returns>Get the levels for the current difficulty.</returns>
		public Level[] GetCurrentLevels ()
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

		[Server]
		/// <summary>
	/// Increase the score by a value.
	/// The more player there is, the more points you get
	/// </summary>
	/// <param name="value">The value by which the score will incremented. Can be negative</param>
	public void AddToScore (int value = 1)
		{
			int previousScore = this.score;
			if (value > 0)
				this.score += value * Math.Max (1, (networkPlayersInfo.Count - 1));
			if (value < 0)
				this.score = Mathf.Max (this.score + value, 0);
			if (previousScore != this.score)
				onScoreChange (this.score - previousScore);
		}

		[Server]
		/// <summary>
	/// Change the difficulty of the game. The first level of that difficulty setting is automatically started.
	/// </summary>
	/// <param name="difficulty">The new difficulty.</param>
	public void ChangeDifficulty (Difficulty difficulty)
		{
			StopAllCoroutines ();
			this.score = 0;
			this.difficulty = difficulty;
			this._nextLevelIndex = 0;
			this.gameState = GameState.LevelEnd;
		}



		// ----------------- Client ------------------



		public override void OnStartClient ()
		{
			CustomNetworkLobbyManager.singleton.client.Send (MyStartMsg, new EmptyMessage ());
		}

		/// <summary>
		/// Requests the client to start the level.
		/// </summary>
		[ClientRpc]
		void RpcLevelStarted ()
		{
			if (!isServer) {
				if (onLevelStarted != null)
					onLevelStarted ();
			}
		}



		// ---------------------------------------------------

		/// <summary>
		/// Spawn all networkPrefabs
		/// </summary>
		void SpawnNetworkPrefabs()
		{
			if (this.GetCurrentLevel ().spawnMotionPointFollow) {
				var motionPointFollow = GameObject.Instantiate (this.motionPointFollow, new Vector3 (0, 4, 0), Quaternion.identity);
				NetworkServer.Spawn (motionPointFollow);
			}
			if (this.GetCurrentLevel ().spawnBulletBuilder) {
				var bulletBuilder = GameObject.Instantiate (this.bulletBuilder, new Vector3 (0, 0, 0), Quaternion.identity);
				bulletBuilder.GetComponent<BulletBuilder> ().pattern = this.GetCurrentLevel ().bulletBuilderPattern;
				NetworkServer.Spawn (bulletBuilder);
			}
			var sphereCDM = GameObject.Instantiate (this.sphereCDM, Vector3.zero, Quaternion.identity);
			NetworkServer.Spawn (sphereCDM);
		}

		/// <summary>
		/// Returns true if the variable readyToBegin is true for all network player info
		/// </summary>
		/// <returns><c>true</c>, if players are ready, <c>false</c> otherwise.</returns>
		bool AllPlayersReady ()
		{
			foreach (var networkPlayerInfo in networkPlayersInfo) {
				if (!networkPlayerInfo.readyToBegin)
					return false;
			}
			return this._readyToStart;
		}

		/// <summary>
		/// Loads the level.
		/// </summary>
		[Server]
		void LoadLevel ()
		{
			var nextSceneName = this.GetCurrentLevels () [this.currentLevelIndex].sceneName;
			this.time = this.GetCurrentLevel ().time;
			CustomNetworkLobbyManager.singleton.ServerChangeScene (nextSceneName);
			this.gameState = GameState.WaitingForPlayers;
		}

		/// <summary>
		/// Starts the time countdown
		/// </summary>
		[Server]
		void StartTime ()
		{
			this._timeCoroutine = StartCoroutine (DecreaseTime ());
		}

		/// <summary>
		/// Stops the time countdown
		/// </summary>
		[Server]
		void StopTime ()
		{
			if (this._timeCoroutine != null)
				StopCoroutine (this._timeCoroutine);
		}

		// The time countdown
		[Server]
		IEnumerator DecreaseTime ()
		{
			while (time > 0) {
				yield return new WaitForSeconds (1.0f);
				time--;
			}
			this._nextLevelIndex = this.currentLevelIndex + 1;
			this.gameState = GameState.PreLevelEnd;
		}

		void InitPlayerInfo ()
		{
			networkPlayersInfo = new List<NetworkPlayerInfo> ();
			foreach (var player in GameObject.FindGameObjectsWithTag("Player")) {
				networkPlayersInfo.Add (
					new NetworkPlayerInfo (player.GetComponent<Player> ().playerInfo,
						player.GetComponent<Player> ().connectionToClient)
				);
			}
		}
	}
}
