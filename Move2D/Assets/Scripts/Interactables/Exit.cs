using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Move2D
{
	/// <summary>
	/// An Exit. Starts the next level if the sphere stays on it for <item>cooldownTime</item> seconds.
	/// </summary>
	public class Exit : NetworkBehaviour, IEnterInteractable, IExitInteractable
	{
		public delegate void ExitHandler();
		public static event ExitHandler onExitEnter;
		/// <summary>
		/// The countdown time in seconds
		/// </summary>
		public const int countdownTime = 5;

		/// <summary>
		/// Whether the countdown is activated or not
		/// </summary>
		[Tooltip ("Whether the countdown is activated or not")]
		[SyncVar] public bool countdownActivated = false;
		/// <summary>
		/// How many seconds are left in the countdown
		/// </summary>
		[Tooltip ("How many seconds are left in the countdown")]
		[SyncVar] public int timeLeft;

		private Coroutine _countdown;

		IEnumerator ExitCountdown ()
		{
			this.timeLeft = countdownTime;
			this.countdownActivated = true;
			while (this.timeLeft > 0) {
				yield return new WaitForSeconds (1.0f);
				timeLeft--;
			}
			GameManager.singleton.ExitLevel ();
			this.countdownActivated = false;
		}

		[Server]
		void StartExitCountdown ()
		{
			this._countdown = StartCoroutine (ExitCountdown ());
		}

		[Server]
		void StopExitCountdown ()
		{
			if (this._countdown != null)
				StopCoroutine (_countdown);
			this.countdownActivated = false;
			this.timeLeft = 0;
		}

		public void OnEnterEffect (SphereCDM sphere)
		{
			StartExitCountdown ();
			RpcExitEvent ();
		}

		public void OnExitEffect (SphereCDM sphere)
		{
			StopExitCountdown ();
		}

		[ClientRpc]
		public void RpcExitEvent()
		{
			if (onExitEnter != null)
				onExitEnter ();
		}
	}
}