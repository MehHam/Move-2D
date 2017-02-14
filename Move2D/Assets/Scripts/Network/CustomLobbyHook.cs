using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class CustomLobbyHook : LobbyHook {
	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
		gamePlayer.GetComponent<Renderer>().material.color = lobbyPlayer.GetComponent<LobbyPlayer> ().playerColor;
		gamePlayer.GetComponent<Player> ().color = lobbyPlayer.GetComponent<LobbyPlayer> ().playerColor;
	}
}
