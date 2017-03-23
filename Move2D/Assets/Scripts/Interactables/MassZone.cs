using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Move2D
{
	public class MassZone : NetworkBehaviour, IEnterInteractable, IExitInteractable
	{
		public delegate void OnMassZoneHandler ();
		public static event OnMassZoneHandler onMassZoneEnter;
		public static event OnMassZoneHandler onMassZoneExit;
		#region IExitInteractable implementation

		public void OnExitEffect (SphereCDM sphere)
		{
			RpcOnExit ();
		}

		#endregion

		#region IEnterInteractable implementation

		public void OnEnterEffect (SphereCDM sphere)
		{
			RpcOnEnter ();
		}

		#endregion

		[ClientRpc]
		void RpcOnEnter()
		{
			if (onMassZoneEnter != null)
				onMassZoneEnter ();
		}

		[ClientRpc]
		void RpcOnExit()
		{
			if (onMassZoneExit != null)
				onMassZoneExit ();
		}
	}
}
