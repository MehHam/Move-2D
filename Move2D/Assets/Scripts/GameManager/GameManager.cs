using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkSceneManager))]
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

	public Slider slider;
	public GameObject progressBar;
	public Animator readySetGo;

	public Level[] levels;

	const short LoadedMessage = MsgType.Highest + 1;
	const short LevelChangedMessage = MsgType.Highest + 2;
	const short AllowSceneActivationMessage = MsgType.Highest + 3;

	[SyncVar] public bool paused = true;
	[SyncVar] public int currentLevelIndex = 0;
	[SyncVar] public uint time = 0;
	[SyncVar] public int score = 0;

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
		ReadySetGo.onAnimationFinished += OnAnimationFinished;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
		ReadySetGo.onAnimationFinished -= OnAnimationFinished;
	}

	void Awake()
	{
		if (GameObject.FindObjectsOfType<GameManager>().Length > 1) {
			NetworkServer.Destroy (gameObject);
			return;
		}
		GameManager.singleton = this;
		DontDestroyOnLoad (this);
	}

	void Start()
	{
		StartLevel ();
	}

	void ImmediateStartLevel ()
	{
		if (isServer) {
			Debug.LogError ("???");
			Debug.Log ("Start Level");
			time = levels [currentLevelIndex].time;
			StartTime ();
			paused = false;
		}
		if (OnLevelStarted != null)
			OnLevelStarted ();
	}
		
	void StartLevel ()
	{
		if (GetCurrentLevel ().readyAnimation) {
			paused = true;
			readySetGo.SetTrigger ("Activation");
		}
		else
			ImmediateStartLevel ();
	}

	[Server]
	public void IncreaseScore()
	{
		this.score++;
	}

	[Server]
	public void IncreaseScore(int value)
	{
		this.score += value;
	}

	[Server]
	public void DecreaseScore()
	{
		this.score--;
		this.score = Mathf.Max (this.score, 0);
	}

	[Server]
	public void DecreaseScore(int value)
	{
		this.score -= value;
		this.score = Mathf.Max (this.score, 0);
	}

	[Server]
	IEnumerator DecreaseTime()
	{
		while (time > 0)
		{
			yield return new WaitForSeconds (1);
			time--;
		}
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
		var nextSceneName = this.levels [this.currentLevelIndex + 1].sceneName;
		this.currentLevelIndex++;
		//var currentSceneName = this.levels [this.currentLevelIndex].sceneName;
		NetworkManager.singleton.ServerChangeScene (nextSceneName);
	}
		
	void OnLevelFinishedLoading(Scene scene, LoadSceneMode loadSceneMode)
	{
		StartLevel ();
	}

	[Server]
	public void StartTime()
	{
		StartCoroutine (DecreaseTime());
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
		if (isServer)
			ImmediateStartLevel ();
	}

	public float GetSliderValue()
	{
		return slider.value;
	}
}
