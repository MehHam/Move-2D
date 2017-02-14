using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Pickup : NetworkBehaviour, IInteractable {
	#region IInteractable implementation
	[Server]
	public void Effect (SphereCDM sphere)
	{
		this.gameObject.SetActive(false);
		GameManager.singleton.IncreaseScore ();
		RpcDisable ();
	}

	[ClientRpc]
	public void RpcDisable()
	{
		this.gameObject.SetActive (false);
	}
	#endregion
}
