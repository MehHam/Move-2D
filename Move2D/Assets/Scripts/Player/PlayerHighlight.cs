using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Move2D
{
	public class PlayerHighlight : MonoBehaviour
	{
		void Update()
		{
			if (GetComponentInParent<Player>())
			{
				GetComponent<SpriteRenderer> ().enabled = GetComponentInParent<Player> ().isLocalPlayer;
				GetComponent<SpriteRenderer> ().color = GetComponentInParent<Player> ().color;
			}
		}
	}
}
