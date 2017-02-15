using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CrossLava : NetworkBehaviour, IInteractable {
	public float scoreCooldownTime;
	private bool _cooldown;
	#region IInteractable implementation
	[Server]
	public void OnEnterEffect (SphereCDM sphere)
	{
		GameManager.singleton.DecreaseScore ();
		StartCoroutine (ScoreCooldown ());
	}

	[Server]
	public void OnStayEffect (SphereCDM sphere)
	{
		if (!_cooldown) {
			GameManager.singleton.DecreaseScore ();
			StartCoroutine (ScoreCooldown());
		}
	}

	[Server]
	public void OnExitEffect (SphereCDM sphere)
	{
		StopCoroutine ("ScoreCooldown");
	}
	#endregion

	[Server]
	IEnumerator ScoreCooldown()
	{
		_cooldown = true;
		yield return new WaitForSeconds (scoreCooldownTime);
		_cooldown = false;
	}
}
