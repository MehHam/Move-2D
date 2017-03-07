using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Creates all the lines between the players
/// </summary>
public class PlayerLineManager : MonoBehaviour {
	public PlayerLine linePrefab;
	List<PlayerLine> _lines;

	void OnEnable()
	{
		CustomNetworkLobbyManager.onClientDisconnect += OnClientDisconnect;
		CustomNetworkLobbyManager.onClientConnect += OnClientConnect;
	}
		
	void OnDisable()
	{
		CustomNetworkLobbyManager.onClientDisconnect -= OnClientDisconnect;
		CustomNetworkLobbyManager.onClientConnect -= OnClientConnect;
	}

	void OnClientConnect (NetworkConnection conn)
	{
		InitLines ();
	}

	void OnClientDisconnect (NetworkConnection conn)
	{
		InitLines ();
	}

	void Start()
	{
		_lines = new List<PlayerLine> ();
		InitLines ();
	}

	void InitLines()
	{
		foreach (var line in _lines) {
			Destroy (line);
		}
		var players = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < players.Length; i++) {
			for (int j = i + 1; j < players.Length; j++) {
				var line = Instantiate (linePrefab);
				line.GetComponent<PlayerLine> ().player1 = players [i];
				line.GetComponent<PlayerLine> ().player2 = players [j];
				_lines.Add (line);
			}
		}
	}
}
