using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DynamicStartPositions : NetworkBehaviour
{
	public float spawnRadius = 16.0f;

	void Awake ()
	{
		for (int i = 0; i < NetworkLobbyManager.singleton.numPlayers; i++) {
			float slice = 2 * Mathf.PI / NetworkLobbyManager.singleton.numPlayers;
			float angle = slice * i;
			float x = Mathf.Cos (angle) * spawnRadius;
			float y = Mathf.Sin (angle) * spawnRadius;
			var go = Instantiate (new GameObject (), new Vector2 (x, y), Quaternion.identity);
			go.transform.parent = this.transform;
			go.AddComponent<NetworkStartPosition> ();
		}
	}
}
