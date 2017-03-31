using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	public class PlayerHighlight : MonoBehaviour
	{
		void Start()
		{
			if (GetComponentInParent<Player>())
			{
				GetComponent<SpriteRenderer> ().enabled = GetComponentInParent<Player> ().isLocalPlayer;
				GetComponent<SpriteRenderer> ().color = GetComponentInParent<Player> ().color;
			}
		}
	}
}
