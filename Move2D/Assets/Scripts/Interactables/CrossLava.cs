using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using UnityEngine;
using UnityEngine.Networking;

public class CrossLava : NetworkBehaviour, IInteractable {
	public float scoreCooldownTime;
	private bool _cooldown;
	private CoroutineHandle _coroutineHandle;

	#region IInteractable implementation
	[Server]
	public void OnEnterEffect (SphereCDM sphere)
	{
		GameManager.singleton.DecreaseScore ();
		_coroutineHandle = Timing.RunCoroutine (ScoreCooldown ());
	}

	[Server]
	public void OnStayEffect (SphereCDM sphere)
	{
		if (!_cooldown) {
			GameManager.singleton.DecreaseScore ();
			Timing.RunCoroutine (ScoreCooldown());
		}
	}

	[Server]
	public void OnExitEffect (SphereCDM sphere)
	{
		if (_coroutineHandle != null)
			Timing.KillCoroutines (_coroutineHandle);
	}
	#endregion

	[Server]
	IEnumerator<float> ScoreCooldown()
	{
		_cooldown = true;
		yield return Timing.WaitForSeconds (scoreCooldownTime);
		_cooldown = false;
	}
}
