using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

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

	/// <summary>
	/// All the type of gameMode:
	/// Default: Move the sphere around to get the pickups
	/// MotionPointFollow: Move the sphere to get it the closest to the motion point
	/// </summary>
	public enum GameMode
	{
		Default,
		MotionPointFollow,
	}

	public enum Difficulty
	{
		Beginner,
		Intermediate,
		Expert,
	}

	public enum SphereVisibility
	{
		Visible,
		FadeAfterStartLevel,
		Invisible,
	}

	/// <summary>
	/// The level data
	/// </summary>
	[System.Serializable]
	public class Level
	{
		/// <summary>
		/// Level index
		/// </summary>
		[Tooltip("Level index")]
		public uint index;
		/// <summary>
		/// The level duration
		/// </summary>
		[Tooltip("Level duration")]
		public uint time;
		/// <summary>
		/// The sceneName in the build, this should never be empty
		/// </summary>
		[Tooltip("The scene name in the build, this should never be empty")]
		public string sceneName;
		/// <summary>
		/// The gameMode of this level
		/// </summary>
		[Tooltip("The game mode of this level")]
		public GameMode gameMode;
		/// <summary>
		/// If the gameMode is MotionPointFollow, what kind of motion the point has
		/// </summary>
		[Tooltip("If the game mode if MotionPointFollow, what kind of motion the point has")]
		public MotionPointFollow.MotionMode motionMode;
		/// <summary>
		/// Whether the ready animation is displayed at the beginning of this level or not.
		/// </summary>
		[Tooltip("Whether the ready animation is displayed at the beginning of this level")]
		public bool readyAnimation = false;
		/// <summary>
		/// The behaviour of the sphere during the level
		/// </summary>
		[Tooltip("The behaviour of the sphere visibility during the level")]
		public SphereVisibility sphereVisibility = SphereVisibility.Visible;
		/// <summary>
		/// Mass modification enabled
		/// </summary>
		[Tooltip("Is mass modification enabled ?")]
		public bool massModification = true;
	}

	public static GameManager singleton { get; private set; }

	public List<PlayerInfo> playerInfos;

	/// <summary>
	/// Array displaying the data of all the beginner levels that will be played
	/// </summary>
	[Tooltip("Array displaying the data of all the beginner levels that will be played")]
	public Level[] beginnerLevels;

	/// <summary>
	/// Array displaying the data of all the intermediate levels that will be played
	/// </summary>
	[Tooltip("Array displaying the data of all the intermediate levels that will be played")]
	public Level[] intermediateLevels;

	/// <summary>
	/// Array displaying the data of all the expert levels that will be played
	/// </summary>
	[Tooltip("Array displaying the data of all the expert levels that will be played")]
	public Level[] expertLevels;

	/// <summary>
	/// Gets a value indicating whether this <see cref="GameManager"/> has an invisible sphere.
	/// </summary>
	/// <value><c>true</c> if invisible sphere; otherwise, <c>false</c>.</value>
	public bool invisibleSphere { get { return (this.GetCurrentLevel ().sphereVisibility == SphereVisibility.Invisible ||
		this.GetCurrentLevel ().sphereVisibility == SphereVisibility.FadeAfterStartLevel);
		} }
	
	public const float arenaRadius = 16.0f;

	/// <summary>
	/// Has the game started or not
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
		Debug.Log ("On enable");
		//NetworkSceneManager.OnClientLevelLoaded += OnLevelFinishedLoading;
		//NetworkSceneManager.OnServerLevelLoaded += OnLevelFinishedLoading;
		CustomNetworkLobbyManager.onClientSceneLoaded += OnClientSceneLoaded;
		ReadySetGo.onAnimationFinished += OnAnimationFinished;
	}

	void OnDisable()
	{
		Debug.Log ("On disable");
		//NetworkSceneManager.OnClientLevelLoaded += OnLevelFinishedLoading;
		//NetworkSceneManager.OnServerLevelLoaded += OnLevelFinishedLoading;
		CustomNetworkLobbyManager.onClientSceneLoaded -= OnClientSceneLoaded;
		ReadySetGo.onAnimationFinished -= OnAnimationFinished;
	}

	void Awake()
	{
		if (singleton != null && singleton != this) {
			NetworkServer.Destroy (this.gameObject);
		} else {
			GameManager.singleton = this;
			DontDestroyOnLoad (this.gameObject);
		}
	}

	[Server]
	void Start()
	{
		StartLevel ();
		InitPlayerInfo ();
	}

	void InitPlayerInfo() {
		playerInfos = new List<PlayerInfo> ();
		foreach (var player in GameObject.FindGameObjectsWithTag("player")) {
			var playerInfo = new PlayerInfo ();
			playerInfo.name = player.GetComponent<Player> ().playerName;
			playerInfo.mass = player.GetComponent<Player> ().mass;
			playerInfo.color = player.GetComponent<Player> ().color;
			playerInfos.Add (playerInfo);
		}
	}

	// Starts immediately the level, ignore the ready set go animation
	void ImmediateStartLevel ()
	{
		if (isServer) {
			StopTime ();
			time = this.GetCurrentLevels() [currentLevelIndex].time;
			StartTime ();
			paused = false;
			gameStarted = true;
			if (onLevelStarted != null)
				onLevelStarted ();
			RpcLevelStarted ();
		}
	}

	[ClientRpc]
	void RpcLevelStarted()
	{
		if (!isServer && onLevelStarted != null)
			onLevelStarted ();
	}

	// Starts the level
	void StartLevel ()
	{
		var readySetGo = GameObject.FindObjectOfType<ReadySetGo> ();
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
	/// Increase the score by a value
	/// </summary>
	/// <param name="value">The value by which the score will be increased.</param>
	[Server]
	public void IncreaseScore(int value)
	{
		if (paused)
			return;
		this.score += value;
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

	// The time countdown
	[Server]
	IEnumerator<float> DecreaseTime()
	{
		while (time > 0)
		{
			yield return Timing.WaitForSeconds (1);
			time--;
		}
		paused = true;
		// If there's a level next, loads the next level when the time is finished
		if (this.currentLevelIndex + 1 < this.GetCurrentLevels().Length) {
			LoadNextLevel ();
		}
	}

	// Load the next level
	[Server]
	void LoadNextLevel ()
	{
		this.currentLevelIndex++;
		var nextSceneName = this.GetCurrentLevels() [this.currentLevelIndex].sceneName;
		gameStarted = false;
		CustomNetworkLobbyManager.singleton.ServerChangeScene(nextSceneName);
	}

	/// <summary>
	/// Starts the time countdown
	/// </summary>
	[Server]
	public void StartTime()
	{
		Timing.RunCoroutine (DecreaseTime());
	}

	/// <summary>
	/// Stops the time countdown
	/// </summary>
	[Server]
	public void StopTime()
	{
		StopAllCoroutines ();
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

	public void OnClientSceneChanged(NetworkConnection conn)
	{
		if (currentLevelIndex != 0 && onClientSceneChanged != null) {
			onClientSceneChanged (conn);
		}
	}

	public void OnServerSceneChanged()
	{
		if (currentLevelIndex != 0) {
			FindObjectOfType<DynamicStartPositions> ().SetAllPlayerPositions ();
			StartLevel ();
		}
	}

	void OnPlayerDisconnected()
	{
		StopTime ();
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
		Timing.KillAllCoroutines ();
	}

	[Server]
	void OnClientSceneLoaded(GameObject lobbyPlayer, GameObject gamePlayer)
	{
	}
}
