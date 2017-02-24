using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
using UnityEngine.SceneManagement;
using UnityEngine.Networking.NetworkSystem;

/// <summary>
/// Custom NetworkLobby class
/// </summary>
public class CustomNetworkLobbyManager : LobbyManager {
	public delegate void ClientEvent(NetworkMessage msg);
	public delegate void SceneLoadedEvent(GameObject lobbyPlayer, GameObject gamePlayer);
	/// <summary>
	/// Occurs when a  scene loaded.
	/// </summary>
	public static event SceneLoadedEvent onClientSceneLoaded;
	/// <summary>
	/// Event called whenever a client disconnect
	/// </summary>
	public static event ClientEvent onClientDisconnect;
	/// <summary>
	/// Event called whenever a client connects
	/// </summary>
	public static event ClientEvent onClientConnect;

	public override NetworkClient StartHost()
	{
		var networkClient = base.StartHost ();
		if (networkClient != null) {
			networkClient.RegisterHandler (MsgType.Disconnect, OnClientDisconnectCustom);
		}
		return networkClient;
	}

	/// <summary>
	/// Called when the host is stopped. Destroy the gameManager, display an error message and reset the default lobby scene
	/// </summary>
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

	/// <summary>
	/// Called when the client is stopped. Destroy the gameManager, display an error message and reset the default lobby scene
	/// </summary>
	public override void OnStopClient()
	{
		if (!_isMatchmaking && GameManager.singleton != null) {
			GameObject.Destroy (GameManager.singleton.gameObject);
			NetworkServer.Destroy (GameManager.singleton.gameObject);
			infoPanel.Display ("You were disconnected from server", "OK", null);
			CustomNetworkLobbyManager.networkSceneName = this.onlineScene;
		}
		base.OnStopClient ();
	}

	/// <summary>
	/// Called the server is stopped. Destroy the gameManager, display an error message and reset the default lobby scene
	/// </summary>
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

	/// <summary>
	/// Called when the server scene is changed
	/// </summary>
	public override void OnServerSceneChanged (string sceneName)
	{
		if (!_isMatchmaking && GameManager.singleton != null && GameManager.singleton.currentLevelIndex != 0) {
			GameManager.singleton.OnServerSceneChanged ();
			Debug.LogError ("ON SERVER SCENE CHANGED");
			foreach (var playerInfo in this.transform.GetComponentsInChildren<LobbyPlayer>()) {
				Debug.LogError ("ON SERVER SCENE CHANGED");
				playerInfo.enabled = true;
				var startPos = GetStartPosition ();
				GameObject gamePlayer;
				if (startPos != null)
					gamePlayer = (GameObject)Instantiate (gamePlayerPrefab, startPos.position, startPos.rotation);
				else
					gamePlayer = (GameObject)Instantiate (gamePlayerPrefab, Vector3.zero, Quaternion.identity);
				OnLobbyServerSceneLoadedForPlayer (playerInfo.gameObject, gamePlayer);
				NetworkServer.ReplacePlayerForConnection (playerInfo.connectionToClient, gamePlayer, playerInfo.playerControllerId);
				playerInfo.enabled = false;
			}
		}
		base.OnServerSceneChanged (sceneName);
	}

	/// <summary>
	/// Called when the client scene is changed
	/// </summary>
	public override void OnClientSceneChanged (NetworkConnection conn)
	{
		if (!_isMatchmaking && GameManager.singleton != null) {
			GameManager.singleton.OnClientSceneChanged (conn);
			/*
			foreach (var playerController in conn.playerControllers) {
				Debug.Log (playerController);
				if (playerController != null) {
					this.client.Send (MsgType.LobbySceneLoaded, new IntegerMessage (playerController.playerControllerId));
				}
			}*/
		}
		base.OnClientSceneChanged (conn);
	}

	public override bool OnLobbyServerSceneLoadedForPlayer (GameObject lobbyPlayer, GameObject gamePlayer)
	{
		gamePlayer.GetComponent<Renderer>().material.color = lobbyPlayer.GetComponent<LobbyPlayer> ().playerColor;
		gamePlayer.GetComponent<Player> ().color = lobbyPlayer.GetComponent<LobbyPlayer> ().playerColor;
		return true;
	}

	public void OnClientDisconnectCustom(NetworkMessage msg)
	{
		if (onClientDisconnect != null)
			onClientDisconnect (msg);
	}
}
