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

	/// <summary>
	/// Event called whenever the level started
	/// </summary>
	public static event GameManagerEvent OnLevelStarted;

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
	}

	public static GameManager singleton { get; private set; }

	/// <summary>
	/// Array displaying the data of all the levels that will be played
	/// </summary>
	[Tooltip("Array displaying the data of all the levels that will be played")]
	public Level[] levels;
	/// <summary>
	/// Whether the game is paused or not
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
		ReadySetGo.onAnimationFinished += OnAnimationFinished;
	}

	void OnDisable()
	{
		//NetworkSceneManager.OnClientLevelLoaded += OnLevelFinishedLoading;
		//NetworkSceneManager.OnServerLevelLoaded += OnLevelFinishedLoading;
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

	void Start()
	{
		StartLevel ();
	}

	// Starts immediately the level, ignore the ready set go animation
	void ImmediateStartLevel ()
	{
		if (isServer) {
			StopTime ();
			time = levels [currentLevelIndex].time;
			StartTime ();
			paused = false;
		}
		if (OnLevelStarted != null)
			OnLevelStarted ();
	}

	// Starts the level
	void StartLevel ()
	{
		var readySetGo = GameObject.FindObjectOfType<ReadySetGo> ();
		if (GetCurrentLevel ().readyAnimation && readySetGo != null && readySetGo.GetComponent<Animator>() != null) {
			paused = true;
			readySetGo.GetComponent<Animator>().SetTrigger ("Activation");
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
		if (this.currentLevelIndex + 1 < this.levels.Length) {
			LoadNextLevel ();
		}
	}

	/*
	void PreloadNextLevel ()
	{
		var nextSceneName = this.levels [this.currentLevelIndex + 1].sceneName;
		var currentSceneName = this.levels [this.currentLevelIndex].sceneName;
		NetworkSceneManager.singleton.PreLoadLevel (nextSceneName, currentSceneName);
	}

	void ActivatePreloadedLevel ()
	{
		NetworkSceneManager.singleton.ActivatePreloadedLevel ();
		this.currentLevelIndex++;
	}
	*/

	// Load the next level
	[Server]
	void LoadNextLevel ()
	{
		this.currentLevelIndex++;
		var nextSceneName = this.levels [this.currentLevelIndex].sceneName;
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
		return this.levels [this.currentLevelIndex];
	}


	public void OnClientSceneChanged()
	{
		if (OnLevelStarted != null)
			OnLevelStarted ();
	}

	public void OnServerSceneChanged()
	{
		StartLevel ();
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
}
