using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// An interactable pickup, gives points to the players when the sphere collides with it
/// </summary>
public class Pickup : NetworkBehaviour, IInteractable {
	/// <summary>
	/// The number of points given to the players when the sphere collides with the pickup.
	/// </summary>
	[Tooltip("The number of points given to the players when the sphere collides with the pickup.")]
	public int scoreValue = 1;
	#region IInteractable implementation
	[Server]
	public void OnEnterEffect (SphereCDM sphere)
	{
		this.gameObject.SetActive(false);
		GameManager.singleton.IncreaseScore (scoreValue);
		if (GameManager.singleton.GetCurrentLevel ().sphereVisibility == Level.SphereVisibility.FadeAfterStartLevel ||
		    GameManager.singleton.GetCurrentLevel ().sphereVisibility == Level.SphereVisibility.Invisible) {
			sphere.Blink ();
		}
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
