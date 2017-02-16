using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkSceneManager))]
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

	const short LoadedMessage = MsgType.Highest + 1;
	const short LevelChangedMessage = MsgType.Highest + 2;
	const short AllowSceneActivationMessage = MsgType.Highest + 3;

	private NetworkSceneManager _networkSceneManager;

	[SyncVar] public bool paused = true;
	[SyncVar] public int currentLevelIndex = 0;
	[SyncVar] public uint time = 0;
	[SyncVar] public int score = 0;

	void OnEnable()
	{
		NetworkSceneManager.OnLevelLoaded += OnLevelFinishedLoading;
		ReadySetGo.onAnimationFinished += OnAnimationFinished;
	}

	void OnDisable()
	{
		NetworkSceneManager.OnLevelLoaded -= OnLevelFinishedLoading;
		ReadySetGo.onAnimationFinished -= OnAnimationFinished;
	}

	void Awake()
	{
		/*NetworkServer.RegisterHandler (LoadedMessage, OnClientLevelFinishedLoading);
		NetworkServer.RegisterHandler (LevelChangedMessage, OnClientLevelChanged);
		if (isClient)
			CustomNetworkLobbyManager.singleton.client.RegisterHandler (AllowSceneActivationMessage, OnAllowSceneActivation);*/
		if (GameObject.FindObjectsOfType<GameManager>().Length > 1) {
			NetworkServer.Destroy (gameObject);
			return;
		}
		_networkSceneManager = this.GetComponent<NetworkSceneManager> ();
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
			time = levels [currentLevelIndex].time;
			StartTime ();
			paused = false;
			if (this.currentLevelIndex + 1 < this.levels.Length) {
				PreloadNextLevel ();
			}
		}
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
			ActivatePreloadedLevel ();
		}
	}

	void PreloadNextLevel ()
	{
		var nextSceneName = this.levels [this.currentLevelIndex + 1].sceneName;
		var currentSceneName = this.levels [this.currentLevelIndex].sceneName;
		_networkSceneManager.PreLoadLevel (nextSceneName, currentSceneName);
	}

	void ActivatePreloadedLevel ()
	{
		_networkSceneManager.ActivatePreloadedLevel ();
		this.currentLevelIndex++;
	}

	void LoadNextLevel ()
	{
		var nextSceneName = this.levels [this.currentLevelIndex + 1].sceneName;
		var currentSceneName = this.levels [this.currentLevelIndex].sceneName;
		_networkSceneManager.LoadLevel (nextSceneName, currentSceneName);
		this.currentLevelIndex++;
	}

	[Server]
	void OnLevelFinishedLoading()
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
			StartLevel ();
	}

	public float GetSliderValue()
	{
		return slider.value;
	}
}
