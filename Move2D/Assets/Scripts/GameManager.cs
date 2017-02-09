using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
	public static GameManager singleton;
	public const int maxTime = 200;

	[SyncVar] public uint time;
	[SyncVar] public int score;

	void Awake()
	{
		GameManager.singleton = this;
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

	IEnumerator DecreaseTime()
	{
		while (true)
		{
			yield return new WaitForSeconds (1);
			time--;
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
}
