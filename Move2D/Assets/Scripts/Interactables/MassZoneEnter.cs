using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Move2D
{
	public class MassZoneEnter : NetworkBehaviour, IEnterInteractable
	{
		public delegate void OnMassZoneHandler ();
		public static event OnMassZoneHandler onMassZoneEnter;

		public GameObject bridgeDirection;

		#region IEnterInteractable implementation

		public void OnEnterEffect (SphereCDM sphere)
		{
			sphere.isInvincible = true;
			RpcOnEnter ();
		}

		#endregion

		[ClientRpc]
		void RpcOnEnter()
		{
			foreach (var renderer in bridgeDirection.GetComponentsInChildren<SpriteRenderer>()) {
				var color = renderer.color;
				renderer.color = new Color (color.r, color.g, color.b, 0.0f);
			}
			bridgeDirection.SetActive (true);
			if (onMassZoneEnter != null)
				onMassZoneEnter ();
		}
	}
}
