using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DynamicStartPositions : NetworkBehaviour
{
	public static float spawnRadius = 16.0f;
	public GameObject startPosition;

	[ServerCallback]
	void Awake()
	{
		for (int i = 0; i < NetworkManager.singleton.numPlayers; i++) {
			float slice = 2 * Mathf.PI / NetworkManager.singleton.numPlayers;
			float angle = slice * i;
			float x = Mathf.Cos (angle) * spawnRadius;
			float y = Mathf.Sin (angle) * spawnRadius;
			var go = Instantiate (startPosition, new Vector2 (x, y), Quaternion.identity);
			go.transform.parent = this.transform;
			NetworkServer.Spawn (go);
		}
	}

	[Server]
	/// <summary>
	/// Sets the positions of all players
	/// </summary>
	public void SetAllPlayerPositions()
	{
		/*
		foreach (var connection in NetworkServer.connections) {
			if (connection != null)
			{
				foreach (var player in connection.playerControllers) {
					var transform = NetworkLobbyManager.singleton.GetStartPosition ();
					if (transform != null) {
						Debug.Log ("GetStartPosition");
						player.gameObject.GetComponent<Player> ().RpcSetPosition (transform.position);
					}
				}
			}
		}
		*/
	}
}
