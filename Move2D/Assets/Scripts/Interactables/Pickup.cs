using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Pickup : NetworkBehaviour, IInteractable {
	public int scoreValue = 1;
	#region IInteractable implementation
	[Server]
	public void OnEnterEffect (SphereCDM sphere)
	{
		this.gameObject.SetActive(false);
		GameManager.singleton.IncreaseScore (scoreValue);
		RpcDisable ();
	}

	[Server]
	public void OnStayEffect (SphereCDM sphere)
	{
	}

	[Server]
	public void OnExitEffect (SphereCDM sphere)
	{
	}
	#endregion

	[ClientRpc]
	public void RpcDisable()
	{
		this.gameObject.SetActive (false);
	}
}
