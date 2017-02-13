using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ProgressBar;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SpherePhysics: NetworkBehaviour
{
	public List<Player> players = new List<Player>();

	void OnPlayerConnected (NetworkPlayer player)
	{
		players = FindPlayers ();
	}

	void OnPlayerDisconnected (NetworkPlayer player)
	{
		players = FindPlayers ();
	}
		
	void Start ()
	{
		players = FindPlayers ();
	}
	// Update is called once per frame
	[ServerCallback]
	void FixedUpdate ()
	{
		MovePosition (players, GetTotalMass(players));
	}

	public List<Player> FindPlayers ()
	{  	
		var players = GameObject.FindGameObjectsWithTag ("Player");
		var res = new List<Player> ();

		foreach (var player in players) {
			res.Add (player.GetComponent<Player> ());
		}
		return res;
	}

	public float GetTotalMass (List<Player> players)
	{
		if (players.Count == 0)
			return 1.0f;
		var sum = 0.0f;
		foreach (var player in players) {
			sum += player.mass;
		}
		return sum;
	}

	public Vector2 MovePosition (List<Player> players, float sum)
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
