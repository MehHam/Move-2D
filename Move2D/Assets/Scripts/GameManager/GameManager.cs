using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {
	public delegate void GameManagerEvent();
	public static event GameManagerEvent OnLevelStarted;
	public enum GameMode
	{
		Default,
		MotionPointFollow,
	}
	[System.Serializable]
	public class Level
	{
		public uint index;
		public uint time;
		public string sceneName;
		public GameMode gameMode;
		public MotionPointFollow.MotionMode motionMode;
		public bool readyAnimation = false;
	}
		
	public static GameManager singleton { get; private set; }

	public Level[] levels;

	[SyncVar] public bool paused = true;
	[SyncVar] public int currentLevelIndex = 0;
	[SyncVar] public uint time = 0;
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

	void ImmediateStartLevel ()
	{
		StopTime ();
		time = levels [currentLevelIndex].time;
		StartTime ();
		paused = false;
		if (OnLevelStarted != null)
			OnLevelStarted ();
	}
		
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

	[Server]
	public void IncreaseScore()
	{
		IncreaseScore (1);
	}

	[Server]
	public void IncreaseScore(int value)
	{
		if (paused)
			return;
		this.score += value;
	}

	[Server]
	public void DecreaseScore()
	{
		DecreaseScore (1);
	}

	[Server]
	public void DecreaseScore(int value)
	{
		if (paused)
			return;
		this.score -= value;
		this.score = Mathf.Max (this.score, 0);
	}

	[Server]
	IEnumerator<float> DecreaseTime()
	{
		while (time > 0)
		{
			yield return Timing.WaitForSeconds (1);
			time--;
		}
		paused = true;
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

	[Server]
	void LoadNextLevel ()
	{
		this.currentLevelIndex++;
		var nextSceneName = this.levels [this.currentLevelIndex].sceneName;
		CustomNetworkLobbyManager.singleton.ServerChangeScene(nextSceneName);
	}

	[Server]
	public void StartTime()
	{
		Timing.RunCoroutine (DecreaseTime());
	}

	[Server]
	public void StopTime()
	{
		StopAllCoroutines ();
	}

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

	void OnAnimationFinished()
	{
		ImmediateStartLevel ();
	}
}
