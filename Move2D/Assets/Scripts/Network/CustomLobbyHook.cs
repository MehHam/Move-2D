using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

/// <summary>
/// Custom class that manage the transition between the lobbyPlayer and the gamePlayer
/// </summary>
public class CustomLobbyHook : LobbyHook {
	/// <summary>
	/// Sets the color of the player according to the color chosen in the lobby
	/// </summary>
	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
		gamePlayer.GetComponent<Renderer>().material.color = lobbyPlayer.GetComponent<LobbyPlayer> ().playerColor;
		gamePlayer.GetComponent<Player> ().color = lobbyPlayer.GetComponent<LobbyPlayer> ().playerColor;
	}
}
