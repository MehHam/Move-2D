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
	/// Called when the server is stopped. Destroy the gameManager, display an error message and reset the default lobby scene
	/// </summary>
	public override void OnStopServer()
	{
		if (!_isMatchmaking && GameManager.singleton != null) {
			GameObject.Destroy (GameManager.singleton.gameObject);
			NetworkServer.Destroy (GameManager.singleton.gameObject);
			// LobbyManager bug fix
			foreach (var playerInfo in GameObject.FindObjectsOfType<LobbyPlayer> ()) {
				GameObject.Destroy (playerInfo);
			}
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
		base.OnServerSceneChanged (sceneName);
		if (!_isMatchmaking && GameManager.singleton != null && GameManager.singleton.currentLevelIndex != 0) {
			GameManager.singleton.OnServerSceneChanged ();
			var playersInfo = GameObject.FindObjectsOfType<LobbyPlayer> ();
			// When the server is also a client, the Lobby Players are disabled and children of the LobbyManager
			if (playersInfo.Length == 0)
				playersInfo = this.transform.GetComponentsInChildren<LobbyPlayer> (true);
			foreach (var playerInfo in playersInfo) {
				var playerInfoPreviousState = playerInfo.enabled;
				// We enable the playerInfo if it wasn't enabled
				playerInfo.enabled = true;
				var startPos = GetStartPosition ();
				GameObject gamePlayer;
				if (startPos != null)
					gamePlayer = (GameObject)Instantiate (gamePlayerPrefab, startPos.position, startPos.rotation);
				else
					gamePlayer = (GameObject)Instantiate (gamePlayerPrefab, Vector3.zero, Quaternion.identity);
				Debug.Log(NetworkServer.ReplacePlayerForConnection (playerInfo.connectionToClient, gamePlayer, playerInfo.playerControllerId));
				OnLobbyServerSceneLoadedForPlayer (playerInfo.gameObject, gamePlayer);
				// The playerInfo returns to its previous state
				playerInfo.enabled = playerInfoPreviousState;
			}
		}
	}

	/// <summary>
	/// Called when the client scene is changed
	/// </summary>
	public override void OnClientSceneChanged (NetworkConnection conn)
	{
		base.OnClientSceneChanged (conn);
		if (!_isMatchmaking && GameManager.singleton != null) {
			GameManager.singleton.OnClientSceneChanged (conn);
		}
	}

	public override bool OnLobbyServerSceneLoadedForPlayer (GameObject lobbyPlayer, GameObject gamePlayer)
	{
		gamePlayer.GetComponent<Renderer>().material.color = lobbyPlayer.GetComponent<LobbyPlayer> ().playerColor;
		gamePlayer.GetComponent<Player> ().playerName = lobbyPlayer.GetComponent<LobbyPlayer> ().playerName;
		gamePlayer.GetComponent<Player> ().color = lobbyPlayer.GetComponent<LobbyPlayer> ().playerColor;
		return true;
	}

	public void OnClientDisconnectCustom(NetworkMessage msg)
	{
		if (onClientDisconnect != null)
			onClientDisconnect (msg);
	}
}
