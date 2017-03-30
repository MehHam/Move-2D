using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Move2D
{
	public class DynamicStartPositions : NetworkBehaviour
	{
		public static float spawnRadius = 16.0f;
		public GameObject startPosition;

		[ServerCallback]
		void Awake ()
		{
			var list = GetPositions (NetworkManager.singleton.numPlayers);
			foreach (var position in list) {
				var go = Instantiate (startPosition, position, Quaternion.identity);
				go.transform.parent = this.transform;
				NetworkServer.Spawn (go);
			}
		}
		/// <summary>
		/// Get the starting position for a number of players.
		/// </summary>
		/// <returns>A list containing the positions of the players </returns>
		/// <param name="numPlayers">The number of players</param>
		public static List<Vector3> GetPositions(int numPlayers)
		{
			var list = new List<Vector3> ();
			for (int i = 0; i < numPlayers; i++) {
				float slice = 2 * Mathf.PI / numPlayers;
				float angle = slice * i;
				float x = Mathf.Cos (angle) * spawnRadius;
				float y = Mathf.Sin (angle) * spawnRadius;
				Vector3 position = new Vector3 (x, y, -20);
				list.Add (position);
			}
			return list;
		}
	}
}