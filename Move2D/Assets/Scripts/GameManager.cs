using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

	public Level[] levels;

	[SyncVar] public int currentLevelIndex = 0;
	[SyncVar] public uint time = 0;
	[SyncVar] public int score = 0;

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void Awake()
	{
		if (GameObject.FindObjectsOfType<GameManager>().Length > 1) {
			Destroy (gameObject);
			return;
		}
		GameManager.singleton = this;
		DontDestroyOnLoad (this);
	}

	[ServerCallback]
	void Start()
	{
		StartLevel ();
	}

	[Server]
	void StartLevel ()
	{
		if (isServer) {
			StopTime ();
			time = levels [currentLevelIndex].time;
			StartTime ();
		}
	}

	[Server]
	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		StartLevel ();
	}

	[Server]
	public void IncreaseScore()
	{
		this.score++;
	}

	[Server]
	public void DecreaseScore()
	{
		this.score--;
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
			this.currentLevelIndex++;
			var sceneName = this.levels [this.currentLevelIndex].sceneName;
			SceneManager.LoadScene (sceneName);
			NetworkLobbyManager.singleton.ServerChangeScene (sceneName);
			foreach (var conn in NetworkServer.connections)
			{
				NetworkServer.SetClientReady (conn);
			}
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
		Destroy (this);
	}
}
