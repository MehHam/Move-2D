using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
using UnityEngine.SceneManagement;

public class CustomNetworkLobbyManager : LobbyManager {
	public delegate void ClientEvent(NetworkMessage msg);
	public static event ClientEvent onClientDisconnect;
	public static event ClientEvent onClientConnect;

	public override NetworkClient StartHost()
	{
		var networkClient = base.StartHost ();
		if (networkClient != null) {
			networkClient.RegisterHandler (MsgType.Disconnect, OnClientDisconnectCustom);
		}
		return networkClient;
	}
		
	public override void OnStopHost()
	{
		if (!_isMatchmaking && GameManager.singleton != null) {
			GameObject.Destroy (GameManager.singleton.gameObject);
			NetworkServer.Destroy (GameManager.singleton.gameObject);
			infoPanel.Display ("The host disconnected", "OK", null);
			CustomNetworkLobbyManager.networkSceneName = this.onlineScene;
		}
		base.OnStopHost ();
	}

	public override void OnStopClient()
	{
		if (!_isMatchmaking && GameManager.singleton != null) {
			GameObject.Destroy (GameManager.singleton.gameObject);
			NetworkServer.Destroy (GameManager.singleton.gameObject);
			infoPanel.Display ("You were disconnected from server", "OK", null);
		}
		base.OnStopClient ();
	}

	public override void OnStopServer()
	{
		if (!_isMatchmaking && GameManager.singleton != null) {
			GameObject.Destroy (GameManager.singleton.gameObject);
			NetworkServer.Destroy (GameManager.singleton.gameObject);
			infoPanel.Display ("The server was stopped", "OK", null);
			CustomNetworkLobbyManager.networkSceneName = this.onlineScene;
		}
		base.OnStopServer ();
	}

	public override void OnServerSceneChanged (string sceneName)
	{
		if (!_isMatchmaking && GameManager.singleton != null) {
			GameManager.singleton.OnServerSceneChanged ();
		}	
		base.OnServerSceneChanged (sceneName);
	}

	public override void OnClientSceneChanged (NetworkConnection conn)
	{
		if (!_isMatchmaking && GameManager.singleton != null) {
			GameManager.singleton.OnClientSceneChanged ();
		}
		base.OnClientSceneChanged (conn);
	}

	public void OnClientDisconnectCustom(NetworkMessage msg)
	{
		if (onClientDisconnect != null)
			onClientDisconnect (msg);
	}

	/*
	public override void OnLobbyServerPlayersReady ()
	{
		bool allready = true;
		for(int i = 0; i < lobbySlots.Length; ++i)
		{
			if(lobbySlots[i] != null)
				allready &= lobbySlots[i].readyToBegin;
		}

		if(allready)
			Timing.RunCoroutine(ServerCountdownPreloadCoroutine());
	}

	public IEnumerator ServerCountdownPreloadCoroutine()
	{
		float remainingTime = prematchCountdown;
		int floorTime = Mathf.FloorToInt(remainingTime);

		NetworkSceneManager.singleton.PreLoadLevel (playScene, SceneManager.GetActiveScene().name);

		while (remainingTime > 0)
		{
			yield return null;

			remainingTime -= Time.deltaTime;
			int newFloorTime = Mathf.FloorToInt(remainingTime);

			if (newFloorTime != floorTime)
			{//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
				floorTime = newFloorTime;

				for (int i = 0; i < lobbySlots.Length; ++i)
				{
					if (lobbySlots[i] != null)
					{//there is maxPlayer slots, so some could be == null, need to test it before accessing!
						Debug.LogError("?");
						(lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(floorTime);
					}
				}
			}
		}

		for (int i = 0; i < lobbySlots.Length; ++i)
		{
			if (lobbySlots[i] != null)
			{
				(lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(0);
			}
		}
		NetworkSceneManager.singleton.ActivatePreloadedLevel ();
	}

	void OnClientLevelLoaded(string sceneName)
	{
		Debug.LogError ("ClientLevelLoaded");
		OnClientSceneChanged (client.connection);
	}

	void OnServerLevelLoaded(string sceneName)
	{
		Debug.LogError ("Server Level Loaded");
		OnServerSceneChanged (sceneName);
	}
*/
}
