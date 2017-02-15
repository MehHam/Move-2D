using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {
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
	}
		
	public static GameManager singleton { get; private set; }

	public Slider slider;
	public Animator readySetGo;

	public Level[] levels;

	private AsyncOperation _nextLevel = null;

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
		this.time = levels [currentLevelIndex].time;
		readySetGo.SetTrigger ("Activation");
	}

	[Server]
	void StartLevel ()
	{
		if (isServer) {
			StopTime ();
			time = levels [currentLevelIndex].time;
			StartTime ();
			paused = false;
		}
		RpcLoadNextLevel ();
		if (!isClient)
			StartCoroutine (LoadNextLevel ());
	}

	[ClientRpc]
	void RpcLoadNextLevel()
	{
		StartCoroutine (LoadNextLevel());
	}

	[ClientRpc]
	void RpcAllowSceneActivation()
	{
		_nextLevel.allowSceneActivation = true;
	}

	IEnumerator LoadNextLevel()
	{
		if (this.currentLevelIndex + 1 < this.levels.Length)
		{
			var sceneName = this.levels [this.currentLevelIndex + 1].sceneName;
			_nextLevel = SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Additive);
			_nextLevel.allowSceneActivation = false;

			Debug.Log (sceneName + " loading");

			yield return _nextLevel;

			Debug.Log (sceneName + " loaded");
		}
	}
		
	[Server]
	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		//StartLevel ();
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
			paused = true;
			this.currentLevelIndex++;
			var sceneName = this.levels [this.currentLevelIndex].sceneName;
			_nextLevel.allowSceneActivation = true;
			RpcAllowSceneActivation ();
			NetworkLobbyManager.singleton.ServerChangeScene (sceneName);
			StartLevel ();
		}
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
			StartLevel ();
	}

	public float GetSliderValue()
	{
		return slider.value;
	}
}
