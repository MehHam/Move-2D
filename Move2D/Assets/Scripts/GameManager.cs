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
		if (GameManager.singleton != null)
			Destroy (this);
		GameManager.singleton = this;
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		if (isServer) {
			time = levels [currentLevelIndex].time;
			StartTime ();
		}
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
			NetworkLobbyManager.singleton.ServerChangeScene (this.levels [this.currentLevelIndex].sceneName);
		}
	}

	[Server]
	public void StartTime()
	{
		StartCoroutine ("TimeCount", DecreaseTime());
	}

	[Server]
	public void StopTime()
	{
		StopCoroutine ("TimeCount");
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
