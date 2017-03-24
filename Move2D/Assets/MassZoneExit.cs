using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Move2D {
	public class MassZoneExit : NetworkBehaviour, IExitInteractable {
		public delegate void OnMassZoneHandler ();
		public static event OnMassZoneHandler onMassZoneExit;

		public GameObject bridgeDirection;

		#region IExitInteractable implementation
		public void OnExitEffect (SphereCDM sphere)
		{
			sphere.isInvincible = false;
			RpcOnExit ();
		}
		#endregion

		[ClientRpc]
		void RpcOnExit()
		{
			bridgeDirection.SetActive (false);
			if (onMassZoneExit != null)
				onMassZoneExit ();
		}
}
}
