using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using UnityEngine;
using UnityEngine.Networking;

namespace Move2D
{
	/// <summary>
	/// An interactable zone that makes the players lose point if the sphere collides with it
	/// </summary>
	public class CrossLava : NetworkBehaviour, IEnterInteractable, IStayInteractable, IExitInteractable
	{
		/// <summary>
		/// Interval of time during which the players won't lose points when the sphere stays on the collider
		/// </summary>
		[Tooltip ("Interval of time during which the players won't lose points when the sphere stays on the collider")]
		public float scoreCooldownTime;

		private bool _cooldown;

		#region IInteractable implementation

		[Server]
		public void OnEnterEffect (SphereCDM sphere)
		{
			GameManager.singleton.AddToScore (-1);
			StartCoroutine (ScoreCooldown ());
			sphere.Damage ();
		}

		[Server]
		public void OnStayEffect (SphereCDM sphere)
		{
			if (!_cooldown) {
				GameManager.singleton.AddToScore (-1);
				StartCoroutine (ScoreCooldown ());
			}
		}

		[Server]
		public void OnExitEffect (SphereCDM sphere)
		{
			sphere.Damage ();
			if (GameManager.singleton.invisibleSphere) {
				sphere.GetComponent<Blinker> ().FadeOut ();
				sphere.GetComponent<Blinker> ().RpcFadeOut ();
			}
		}

		#endregion

		[Server]
		IEnumerator ScoreCooldown ()
		{
			_cooldown = true;
			yield return new WaitForSeconds (scoreCooldownTime);
			_cooldown = false;
		}
	}
}