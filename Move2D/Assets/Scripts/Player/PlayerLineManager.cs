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
		public SphereCDM _sphereCDM;
		List<PlayerLine> _lines = new List<PlayerLine> ();

		void OnEnable ()
		{
			Player.onPlayerDestroy += OnPlayerDestroy;
			GameManager.onLevelStarted += OnLevelStarted;
			GameManager.onRespawn += OnLevelStarted;
		}

		void OnDisable ()
		{
			Player.onPlayerDestroy -= OnPlayerDestroy;
			GameManager.onLevelStarted -= OnLevelStarted;
			GameManager.onRespawn -= OnLevelStarted;
		}

		void OnPlayerDestroy (Player player)
		{
			var line = _lines.Find (x => x.object1 == player.gameObject || x.object2 == player.gameObject);
			if (line != null) {
				_lines.Remove (line);
				Destroy (line.gameObject);
				Destroy (line);
			}
		}

		void OnLevelStarted ()
		{
			InitLines ();
		}

		void PlayerToSphereInit(List<PlayerLine> lines)
		{
			var players = GameObject.FindGameObjectsWithTag ("Player");
			foreach (var player in players) {
				if (player != null) {
					var line = Instantiate (linePrefab, new Vector3 (0, 0, -20.0f), Quaternion.identity);
					line.GetComponent<PlayerLine> ().object1 = player;
					line.GetComponent<PlayerLine> ().object2 = this.gameObject;
					line.GetComponent<PlayerLine> ().spherePhysics = this.GetComponent<SpherePhysics> ();
					lines.Add (line);
				}
			}
		}

		void PlayerToPlayerInit(List<PlayerLine> lines)
		{
			var players = GameObject.FindGameObjectsWithTag ("Player");
			for (int i = 0; i < players.Length; i++) {
				for (int j = i + 1; j < players.Length; j++) {
					if (players [i] != null && players [j] != null) {
						var line = Instantiate (linePrefab, new Vector3 (0, 0, -20.0f), Quaternion.identity);
						line.GetComponent<PlayerLine> ().object1 = players [i];
						line.GetComponent<PlayerLine> ().object2 = players [j];
						line.GetComponent<PlayerLine> ().spherePhysics = this.GetComponent<SpherePhysics> ();
						lines.Add (line);
					}
				}
			}
		}

		void OnDestroy ()
		{
			foreach (var line in this._lines) {
				if (line != null) {
					Destroy (line.gameObject);
					Destroy (line);
				}
			}
			this._lines.Clear ();
		}

		void InitLines ()
		{
			foreach (var line in this._lines) {
				Destroy (line.gameObject);
				Destroy (line);
			}
			this._lines.Clear ();
			PlayerToSphereInit (this._lines);
			Debug.Log (this._lines.Count);
			//PlayerToPlayerInit (this._lines);
		}
	}
}
