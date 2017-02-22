using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
using UnityEngine.SceneManagement;

/// <summary>
/// Custom NetworkLobby class
/// </summary>
public class CustomNetworkLobbyManager : LobbyManager {
	public delegate void ClientEvent(NetworkMessage msg);
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
		if (!_isMatchmaking && GameManager.singleton != null) {
			GameManager.singleton.OnServerSceneChanged ();
		}	
		base.OnServerSceneChanged (sceneName);
	}

	/// <summary>
	/// Called when the client scene is changed
	/// </summary>
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

	public override bool OnLobbyServerSceneLoadedForPlayer (GameObject lobbyPlayer, GameObject gamePlayer)
	{
		var res = base.OnLobbyServerSceneLoadedForPlayer (lobbyPlayer, gamePlayer);
		var players = GameObject.FindObjectsOfType<Player> ();

		if (players.Length == NetworkServer.connections.Count) {
			for (int i = 0; i < players.Length; i++) {
				float slice = 2 * Mathf.PI / players.Length;
				float angle = slice * i;
				float x = Mathf.Cos (angle) * 15.0f;
				float y = Mathf.Sin (angle) * 15.0f;
				players [i].GetComponent<Rigidbody2D> ().position = new Vector2 (x, y);
				Debug.Log (players [i].GetComponent<Rigidbody2D> ().position);
			}
		}
		return res;
	}
}
