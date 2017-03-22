using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Move2D
{
	/// <summary>
	/// Creates all the lines between the players
	/// </summary>
	public class PlayerLineManager : NetworkBehaviour
	{
		public PlayerLine linePrefab;
		List<PlayerLine> _lines;

		void OnEnable ()
		{
			Player.onPlayerDestroy += OnPlayerDestroy;
		}

		void OnDisable ()
		{
			Player.onPlayerDestroy -= OnPlayerDestroy;
		}

		void OnPlayerDestroy (Player player)
		{
			for (int i = 0; i < _lines.Count; i++) {
				if (_lines [i].object1 == player || _lines [i].object2 == player) {
					Destroy (_lines [i].gameObject);
					Destroy (_lines [i]);
				}
				_lines.RemoveAt (i);
			}
		}

		void Start ()
		{
			this._lines = new List<PlayerLine> ();
			InitLines ();
		}

		void PlayerToSphereInit(List<PlayerLine> lines)
		{
			var players = GameObject.FindGameObjectsWithTag ("Player");
			var sphereCDM = GameObject.FindGameObjectWithTag ("SphereCDM");
			foreach (var player in players) {
				var line = Instantiate (linePrefab, new Vector3 (0, 0, -20.0f), Quaternion.identity);
				line.GetComponent<PlayerLine> ().object1 = player;
				line.GetComponent<PlayerLine> ().object2 = sphereCDM;
				lines.Add (line);
			}
		}

		void PlayerToPlayerInit(List<PlayerLine> lines)
		{
			var players = GameObject.FindGameObjectsWithTag ("Player");
			for (int i = 0; i < players.Length; i++) {
				for (int j = i + 1; j < players.Length; j++) {
					var line = Instantiate (linePrefab, new Vector3 (0, 0, -20.0f), Quaternion.identity);
					line.GetComponent<PlayerLine> ().object1 = players [i];
					line.GetComponent<PlayerLine> ().object2 = players [j];
					lines.Add (line);
				}
			}
		}

		void InitLines ()
		{
			foreach (var line in this._lines) {
				Destroy (line.gameObject);
				Destroy (line);
			}
			this._lines = new List<PlayerLine> ();
			PlayerToSphereInit (this._lines);
			// PlayerToPlayerInit (this._lines);
		}
	}
}
