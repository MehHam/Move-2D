using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
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

	const short LoadedMessage = MsgType.Highest + 1;
	const short LevelChangedMessage = MsgType.Highest + 2;
	const short AllowSceneActivationMessage = MsgType.Highest + 3;
	private AsyncOperation _nextLevel = null;
	private int _clientFinishedLoading = 0;
	private int _clientLevelChanged = 0;

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
		NetworkServer.RegisterHandler (LoadedMessage, OnClientLevelFinishedLoading);
		NetworkServer.RegisterHandler (LevelChangedMessage, OnClientLevelChanged);
		if (isClient)
			CustomNetworkLobbyManager.singleton.client.RegisterHandler (AllowSceneActivationMessage, OnAllowSceneActivation);
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
			time = levels [currentLevelIndex].time;
			StartTime ();
			paused = false;
		}
		RpcStartNextLevel ();
	}

	[ClientRpc]
	void RpcStartNextLevel()
	{
		StartCoroutine (LoadNextLevel ());
	}

	IEnumerator LoadNextLevel()
	{
		if (this.currentLevelIndex + 1 < this.levels.Length)
		{
			Debug.Log ("Coroutine");
			var sceneName = this.levels [this.currentLevelIndex + 1].sceneName;
			_nextLevel = SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Single);
			_nextLevel.allowSceneActivation = false;

			while (_nextLevel.progress < 0.9f) {
				Debug.Log (_nextLevel.progress);
				yield return null;
			}

			Debug.Log ("NextLevel.progress == 0.9f");
			CustomNetworkLobbyManager.singleton.client.Send (LoadedMessage, new EmptyMessage ());

			while (!_nextLevel.isDone)
				yield return null;

			CustomNetworkLobbyManager.singleton.client.Send (LevelChangedMessage, new EmptyMessage ());
			ClientScene.Ready (this.connectionToServer);
		}
	}

	void OnClientLevelChanged(NetworkMessage msg)
	{
		_clientLevelChanged++;
		Debug.Log (_clientLevelChanged);
	}

	void OnClientLevelFinishedLoading(NetworkMessage msg)
	{
		_clientFinishedLoading++;
		Debug.Log (_clientFinishedLoading);
	}

	void OnAllowSceneActivation(NetworkMessage msg)
	{
		_nextLevel.allowSceneActivation = true;
	}

	[Command]
	void CmdInTheLevel()
	{
		_clientLevelChanged++;
		Debug.Log (_clientLevelChanged);
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
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
	IEnumerator WaitForAllClients()
	{
		Debug.Log ("wait for all clients");
		Debug.Log (NetworkServer.connections.Count);
		NetworkServer.SetAllClientsNotReady ();
		while (_clientFinishedLoading < NetworkServer.connections.Count) {
			yield return null;
		}
		Debug.Log (_clientFinishedLoading);
		_clientFinishedLoading = 0;
		_nextLevel.allowSceneActivation = true;
		NetworkServer.SendToAll (AllowSceneActivationMessage, new EmptyMessage ());
		while (_clientLevelChanged < NetworkServer.connections.Count) {
			yield return null;
		}
		paused = true;
		this.currentLevelIndex++;
		_clientLevelChanged = 0;
		yield return SceneManager.LoadSceneAsync (this.levels [this.currentLevelIndex].sceneName);
		NetworkServer.SpawnObjects ();
		StartLevel ();
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
			StartCoroutine (WaitForAllClients());
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
