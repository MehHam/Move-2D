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
		Respawn,
		GameOver,
		Victory,
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
		/// Event called whenever the sphere respawned;
		/// </summary>
		public static event GameManagerHandler onRespawn;
		/// <summary>
		/// Occurs when the score is changed;
		/// </summary>
		public static event GameManagerScoreHandler onScoreChange;
		/// <summary>
		/// Occurs when the sphere takes its first damage
		/// </summary>
		public static event GameManagerHandler onFirstDamage;
		/// <summary>
		/// Occurs when the players get a time warning
		/// </summary>
		public static event GameManagerHandler onTimeWarning;

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
		public bool gyroscope = false;

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
		public const int maxLife = 5;

		/// <summary>
		/// After this timeout the server will start the game and kick all clients that didn't send their ready to begin message
		/// </summary>
		public float timeOut = 10.0f;
		/// <summary>
		/// The restart time out.
		/// </summary>
		public float restartTimeOut = 30.0f;

		public bool isPlaying { get { return this._gameState == GameState.Playing; } }
		public bool isWaitingForPlayers { get { return this._gameState == GameState.WaitingForPlayers; } }
		public bool isGameOver { get { return this._gameState == GameState.GameOver; } }
		public bool isVictory { get { return this._gameState == GameState.Victory; } }

		/// <summary>
		/// Has the game started or not.
		/// </summary>
		public bool gameStarted { get { return this._gameState != GameState.Inactive; } }

		/// <summary>
		/// The difficulty of the game
		/// </summary>
		[Tooltip ("The difficulty of the game")]
		[SyncVar] public Difficulty difficulty = Difficulty.Beginner;
		/// <summary>
		/// The current game state
		/// </summary>
		[Tooltip ("The current game state")]
		[SerializeField]
		[SyncVar] protected GameState _gameState = GameState.Inactive;
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
		/// How much life the sphere has left
		/// </summary>
		[Tooltip ("How much life the sphere has left")]
		[SyncVar] public int life = 0;
		/// <summary>
		/// The current score of the players
		/// </summary>
		[Tooltip ("Current score of the player")]
		[SyncVar] public int score = 0;
		/// <summary>
		/// Time since the players started the game.
		/// </summary>
		[Tooltip ("Time since the players started the game.")]
		[SyncVar] public int totalTime = 0;
		/// <summary>
		/// Whether the player have taken a damage yet
		/// </summary>
		[Tooltip ("Whether the players have taken a damage yet.")]
		public bool firstDamage = false;
		/// <summary>
		/// Whether the player got a warning about the timeLeft
		/// </summary>
		public bool timeWarning = false;



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
			if (isServer) {
				int count = 0;
				// Some connections can be null for some reason, so we have to do the count by ourself
				foreach (var connection in NetworkServer.connections) {
					if (connection != null)
						count++;
				}
				// If all clients are ready we can start the level
				if (count == this._playerReadyToStart) {
					this._gameState = GameState.PreLevel;
					this._playerReadyToStart = 0;
				}
			}
		}

		/// <summary>
		/// Called when the game state is WaitingForPlayers.
		/// Waits for all players to be ready, kicks all the clients that aren't after the timeout.
		/// </summary>
		void WaitingForPlayers ()
		{
			if (isServer) {
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
				this._gameState = GameState.PreLevel;
			}
		}

		/// <summary>
		/// Called when the game state is at PreLevel. Do all the pre-level actions.
		/// </summary>
		void PreLevel ()
		{
			if (isServer) {
				var readySetGo = GameObject.FindObjectOfType<ReadySetGo> ();
				if (GetCurrentLevel ().readyAnimation && readySetGo != null && readySetGo.GetComponent<Animator> () != null) {
					readySetGo.GetComponent<Animator> ().SetTrigger ("Activation");
					RpcReadySetGo ();
					this._gameState = GameState.WaitingForAnimation;

				} else
					this._gameState = GameState.LevelStart;
			}
		}

		/// <summary>
		/// Waitings for pre level animation.
		/// </summary>
		void WaitingForAnimation ()
		{
		}

		/// <summary>
		/// Called when the game state is at LevelStart. Start the level
		/// </summary>
		void LevelStart ()
		{
			if (isServer) {
				InitPlayerInfo ();
				StopTime ();
				time = this.GetCurrentLevels () [currentLevelIndex].time;
				life = maxLife;
				StartTime ();
				SpawnNetworkPrefabs ();
				if (onLevelStarted != null)
					onLevelStarted ();
				RpcLevelStarted ();
				this._gameState = GameState.Playing;
			}
		}

		/// <summary>
		/// Called when the game state is Playing.
		/// </summary>
		void Playing ()
		{
			if (life <= 0 || time <= 0)
				this._gameState = GameState.GameOver;
		}

		/// <summary>
		/// Called when the game state is PreLevelEnd. Do the pre level end stuff.
		/// </summary>
		[Server]
		void PreLevelEnd ()
		{
			if (this.time > 0) {
				this.time = Mathf.Max (0, this.time - 5);
				this.score++;
			} else {
				this._nextLevelIndex = this.currentLevelIndex + 1;
				this._gameState = GameState.LevelEnd;
			}
		}

		/// <summary>
		/// Called when the game state is LevelEnd. End the level and loads the next one.
		/// </summary>
		[Server]
		void LevelEnd ()
		{
			StopAllCoroutines ();
			if (isServer) {
				if (this._nextLevelIndex < this.GetCurrentLevels ().Length) {
					this.currentLevelIndex = this._nextLevelIndex;
					LoadLevel ();
				} else {
					this._gameState = GameState.Victory;
				}
			}
		}
			
		void Respawn()
		{
			if (isServer) {
				RespawnPlayers ();
				if (onRespawn != null)
					onRespawn ();
				RpcRespawn ();
				this._gameState = GameState.Playing;
			}
		}

		[Server]
		void GameOver ()
		{
			if (isServer) {
				if (!_startingTime.HasValue) {
					this._playerReadyToStart = 0;
					_startingTime = Time.time;
				}
				int count = 0;
				// Some connections can be null for some reason, so we have to do the count by ourselves
				foreach (var connection in NetworkServer.connections) {
					if (connection != null)
						count++;
				}
				// If all clients are ready we can start the level
				if (count == this._playerReadyToStart || (Time.time >= _startingTime.Value + restartTimeOut)) {
					this._nextLevelIndex = 0;
					this.score = 0;
					this._gameState = GameState.LevelEnd;
					this._playerReadyToStart = 0;
					_startingTime = null;
					if (NetworkManager.singleton.numPlayers < CustomNetworkLobbyManager.s_Singleton.minPlayers) {
						RpcNetworkErrorMessage (NetworkErrorMessage.NotEnoughPlayers);
						CustomNetworkLobbyManager.s_Singleton.GoBackButton ();
					}
				}
			}
		}

		[Server]
		void Victory ()
		{
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
			switch (this._gameState) {
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
			case GameState.Respawn:
				Respawn ();
				break;
			case GameState.GameOver:
				GameOver ();
				break;
			case GameState.Victory:
				Victory ();
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
			// We update all the start positions to have the sphere still be in the center of the arena
			var startPositions = DynamicStartPositions.GetPositions (networkPlayersInfo.Count);
			for (int i = 0; i < startPositions.Count && i < networkPlayersInfo.Count; i++) {
				networkPlayersInfo [i].playerInfo.startPosition = startPositions [i];
			}
			if (networkPlayersInfo.Count < CustomNetworkLobbyManager.s_Singleton.minPlayers)
				CustomNetworkLobbyManager.s_Singleton.GoBackButton();
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
			if (this._gameState != GameState.Inactive) {
				foreach (var networkPlayerInfo in networkPlayersInfo) {
					var startPos = networkPlayerInfo.playerInfo.startPosition;
					GameObject gamePlayer;
					gamePlayer = (GameObject)Instantiate (((LobbyManager)LobbyManager.singleton).gamePlayerPrefab,
							startPos,
							Quaternion.identity);
					NetworkServer.ReplacePlayerForConnection (networkPlayerInfo.networkConnection,
						gamePlayer,
						networkPlayerInfo.playerInfo.playerControllerId);
					gamePlayer.GetComponent<Player> ().playerInfo = networkPlayerInfo.playerInfo;
					gamePlayer.GetComponent<Player> ().playerInfo.mass = 1.0f;
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
			if (isServer) {
				this._gameState = GameState.LevelStart;
			}
		}



		// ----------------- Public Methods ------------------

		/// <summary>
		/// Exits the level.
		/// </summary>
		[Server]
		public void ExitLevel ()
		{
			this._nextLevelIndex = this.currentLevelIndex + 1;
			this._gameState = GameState.PreLevelEnd;
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
			this.totalTime = 0;
			this._gameState = GameState.LevelEnd;
		}

		public void LoseLife(int amount)
		{
			if (!firstDamage) {
				firstDamage = true;
				if (onFirstDamage != null)
					onFirstDamage ();
				RpcFirstDamage ();
			}
			life = Math.Max(life - amount, 0);
		}

		public void StartRespawn() 
		{
			if (this.isPlaying)
				this._gameState = GameState.Respawn;
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

		[ClientRpc]
		void RpcRespawn ()
		{
			if (!isServer) {
				if (onRespawn != null)
					onRespawn ();
			}
		}

		[ClientRpc]
		void RpcFirstDamage ()
		{
			if (!isServer) {
				if (!firstDamage) {
					if (onFirstDamage != null)
						onFirstDamage ();
					firstDamage = true;
				}
			}
		}

		[ClientRpc]
		void RpcTimeWarning ()
		{
			if (!isServer) {
				if (!timeWarning) {
					if (onTimeWarning != null)
						onTimeWarning ();
					timeWarning = true;
				}
			}
		}

		[ClientRpc]
		void RpcNetworkErrorMessage (NetworkErrorMessage networkErrorMessage)
		{
			((CustomNetworkLobbyManager)(CustomNetworkLobbyManager.singleton)).errorMessage = networkErrorMessage;
		}

		[ClientRpc]
		void RpcReadySetGo()
		{
			if (!isServer) {
				var readySetGo = GameObject.FindObjectOfType<ReadySetGo> ();
				readySetGo.GetComponent<Animator> ().SetTrigger ("Activation");
			}
		}


		// ---------------------------------------------------

		[Server]
		/// <summary>
		/// Reset the position of all players
		/// </summary>
		void RespawnPlayers()
		{
			var players = GameObject.FindGameObjectsWithTag ("Player");
			var sphereCDM = GameObject.FindGameObjectWithTag ("SphereCDM");
			foreach (var player in players) {
				if (player.transform.parent != null)
					GameObject.Destroy (player.transform.parent.gameObject);
				GameObject.Destroy (player);
			}
			GameObject.Destroy (sphereCDM);
			foreach (var networkPlayerInfo in networkPlayersInfo) {
				var startPos = networkPlayerInfo.playerInfo.startPosition;
				var gamePlayer = (GameObject)Instantiate (((LobbyManager)LobbyManager.singleton).gamePlayerPrefab,
					startPos,
					Quaternion.identity);
				NetworkServer.ReplacePlayerForConnection (networkPlayerInfo.networkConnection,
					gamePlayer,
					networkPlayerInfo.playerInfo.playerControllerId);
				gamePlayer.GetComponent<Player> ().playerInfo = networkPlayerInfo.playerInfo;
				gamePlayer.GetComponent<Player> ().playerInfo.mass = 1.0f;
			}
			sphereCDM = GameObject.Instantiate (this.sphereCDM, Vector3.zero, Quaternion.identity);
			NetworkServer.Spawn (sphereCDM);
		}
		/// <summary>
		/// Spawn all networkPrefabs
		/// </summary>
		void SpawnNetworkPrefabs ()
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
			NetworkServer.SpawnObjects ();
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
			this._gameState = GameState.WaitingForPlayers;
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
				if (isPlaying) {
					time--;
					totalTime++;
					if (!timeWarning && time < 30) {
						RpcTimeWarning ();
						if (onTimeWarning != null)
							onTimeWarning ();
					}
				}
			}
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
