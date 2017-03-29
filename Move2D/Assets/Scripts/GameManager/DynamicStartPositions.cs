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
			for (int i = 0; i < NetworkManager.singleton.numPlayers; i++) {
				float slice = 2 * Mathf.PI / NetworkManager.singleton.numPlayers;
				float angle = slice * i;
				float x = Mathf.Cos (angle) * spawnRadius;
				float y = Mathf.Sin (angle) * spawnRadius;
				var go = Instantiate (startPosition, new Vector3 (x, y, -20), Quaternion.identity);
				go.transform.parent = this.transform;
				NetworkServer.Spawn (go);
			}
		}
	}
}