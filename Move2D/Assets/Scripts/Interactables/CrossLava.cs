using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// An interactable zone that makes the players lose point if the sphere collides with it
/// </summary>
public class CrossLava : NetworkBehaviour, IInteractable {
	/// <summary>
	/// Interval of time during which the players won't lose points when the sphere stays on the collider
	/// </summary>
	[Tooltip("Interval of time during which the players won't lose points when the sphere stays on the collider")]
	public float scoreCooldownTime;

	private bool _cooldown;
	private CoroutineHandle _coroutineHandle;

	#region IInteractable implementation
	[Server]
	public void OnEnterEffect (SphereCDM sphere)
	{
		GameManager.singleton.DecreaseScore ();
		_coroutineHandle = Timing.RunCoroutine (ScoreCooldown ());
		sphere.DamageAnimation ();
		sphere.RpcDamageAnimation ();
	}

	[Server]
	public void OnStayEffect (SphereCDM sphere)
	{
		if (!_cooldown) {
			GameManager.singleton.DecreaseScore ();
			Timing.RunCoroutine (ScoreCooldown ());
		}
	}

	[Server]
	public void OnExitEffect (SphereCDM sphere)
	{
		if (_coroutineHandle != null)
			Timing.KillCoroutines (_coroutineHandle);
		sphere.StopDamageAnimation ();
		sphere.RpcStopDamageAnimation ();
		if (GameManager.singleton.invisibleSphere) {
			sphere.FadeOut ();
			sphere.RpcFadeOut ();
		}
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
