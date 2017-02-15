using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

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
		if (!_isMatchmaking) {
			GameObject.Destroy (GameManager.singleton.gameObject);
			NetworkServer.Destroy (GameManager.singleton.gameObject);
			infoPanel.Display ("The host disconnected", "OK", null);
		}
		base.OnStopHost ();
	}

	public override void OnStopClient()
	{
		if (!_isMatchmaking) {
			GameObject.Destroy (GameManager.singleton.gameObject);
			NetworkServer.Destroy (GameManager.singleton.gameObject);
			infoPanel.Display ("You were disconnected from server", "OK", null);
		}
		base.OnStopClient ();
	}

	public override void OnStopServer()
	{
		if (!_isMatchmaking) {
			GameObject.Destroy (GameManager.singleton.gameObject);
			NetworkServer.Destroy (GameManager.singleton.gameObject);
			infoPanel.Display ("The server was stopped", "OK", null);
		}
		base.OnStopServer ();
	}

	public void OnClientDisconnectCustom(NetworkMessage msg)
	{
		if (onClientDisconnect != null)
			onClientDisconnect (msg);
	}
}
