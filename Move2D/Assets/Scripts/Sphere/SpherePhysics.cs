using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ProgressBar;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>  
///  This class manages the position of the sphere in relation to each of the other players
/// </summary>  
public class SpherePhysics: NetworkBehaviour
{
	/// <summary>
	/// The list of players that influence the sphere
	/// </summary>
	[Tooltip("The list of players that can influence the sphere")]
	public List<Player> players = new List<Player>();

	// Called whenever a client disconnects, update the player list
	void OnClientDisconnect (NetworkConnection conn)
	{
		Debug.Log ("connect");
		players = FindPlayers ();
	}

	// Called whenever a client connects, update the player list
	void OnClientConnect (NetworkConnection conn)
	{
		Debug.Log ("Disconnect");
		players = FindPlayers ();
	}

	// Called whenever this script is enabled, adds event listeners
	void OnEnable ()
	{
		CustomNetworkLobbyManager.onClientDisconnect += OnClientDisconnect;
		CustomNetworkLobbyManager.onClientConnect += OnClientConnect;
	}

	// Called whenever this script is disabled, remove event listeners
	void OnDisable ()
	{
		CustomNetworkLobbyManager.onClientDisconnect -= OnClientDisconnect;
		CustomNetworkLobbyManager.onClientConnect -= OnClientConnect;
	}

	void Start ()
	{
		players = FindPlayers ();
		this.transform.position = this.MovePosition (players, GetTotalMass(players));
	}

	void FixedUpdate ()
	{
		players = FindPlayers ();
		MovePosition (players, GetTotalMass(players));
	}

	// Find all the players currently in the scene
	List<Player> FindPlayers ()
	{  	
		var players = GameObject.FindGameObjectsWithTag ("Player");
		var res = new List<Player> ();

		foreach (var player in players) {
			res.Add (player.GetComponent<Player> ());
		}
		return res;
	}

	// Get the total mass of all players currently in the scene
	float GetTotalMass (List<Player> players)
	{
		if (players.Count == 0)
			return 1.0f;
		var sum = 0.0f;
		foreach (var player in players) {
			sum += player.mass;
		}
		return sum;
	}

	// Change the position of the sphere according to the players and their mass
	Vector2 MovePosition (List<Player> players, float sum)
	{
		Vector2 posTemp = Vector2.zero;
		if (players != null && players.Count >= 2) {
			foreach (var player in players)
				posTemp += (Vector2)player.gameObject.transform.position * (player.mass * 1.0f) / sum; 
			this.GetComponent<Rigidbody2D> ().MovePosition (posTemp);
		} else if (players != null && players.Count == 1) {
			this.GetComponent<Rigidbody2D> ().MovePosition ((Vector2)players [0].gameObject.transform.position);
		}
		else
			this.GetComponent<Rigidbody2D> ().MovePosition (new Vector2 (0, 0));
		return this.GetComponent<Rigidbody2D> ().position;
	}
} 
