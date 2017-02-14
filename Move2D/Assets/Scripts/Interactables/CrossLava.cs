using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CrossLava : NetworkBehaviour, IInteractable {
	#region IInteractable implementation

	[Server]
	public void Effect (SphereCDM sphere)
	{
		GameManager.singleton.DecreaseScore ();
	}

	#endregion


}
