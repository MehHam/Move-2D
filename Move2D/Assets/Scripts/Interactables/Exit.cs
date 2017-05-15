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

		int _scorePrerequisite;
		int _startingScore;

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
			// The countdown doesn't always have the same value. If the players are in a hurry or they have nothing left to do,
			// they should'nt have to wait as much
			if (GameManager.singleton.time < 2)
				this.timeLeft = 0;
			else if (GameManager.singleton.time < 30 || LevelManager.singleton.pickupCount == 0)
				this.timeLeft = Mathf.Min (2, countdownTime);
			else
				this.timeLeft = countdownTime;
			this.countdownActivated = true;
			while (this.timeLeft > 0 && GameManager.singleton.isPlaying) {
				yield return new WaitForSeconds (1.0f);
				timeLeft--;
			}
			if (GameManager.singleton.isPlaying) {
				GameManager.singleton.ExitLevel ();
			}
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
			if (LevelManager.singleton.hasScoreReached) {
				StartExitCountdown ();
				RpcExitEvent ();
			}
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

		void Activation()
		{
			bool activated = (LevelManager.singleton != null
				&& LevelManager.singleton.levelHasStarted
				&& LevelManager.singleton.hasScoreReached);
			var color = this.GetComponent<SpriteRenderer> ().color;
			this.GetComponent<SpriteRenderer> ().color = new Color (color.r, color.g, color.b, activated ? 1.0f : 0.3f);
			this.GetComponent<Collider2D> ().enabled = activated;
		}

		void Awake()
		{
			Activation ();
		}

		void Update()
		{
			Activation ();
		}
	}
}