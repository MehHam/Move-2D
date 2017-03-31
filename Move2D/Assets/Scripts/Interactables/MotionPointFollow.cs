using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using ProgressBar;
using UnityEngine.UI;

namespace Move2D
{
	/// <summary>
	/// A special type of interactable object. It moves around the scene with different patterns, give points to the players if the sphere
	/// collides with it.
	/// </summary>
	public class MotionPointFollow : NetworkBehaviour, IEnterInteractable, IStayInteractable, IExitInteractable
	{
		public delegate void MotionPointFollowHandler();
		public static event MotionPointFollowHandler onMotionPointEnter;
		/// <summary>
		/// All the different types of motion mode
		/// Random: The motion point appears at random points in the arena
		/// Cos: The motion point ondulates in the arena
		/// Rotate: The motion point rotates around a center point.
		/// </summary>
		public enum MotionMode
		{
			Random,
			Cos,
			Rotate
		}

		/// <summary>
		/// Amplitude of the cos pattern
		/// </summary>
		[Tooltip ("Amplitude of the cos pattern")]
		public int amplitude = 8;
		/// <summary>
		/// The speed of the motion point
		/// </summary>
		[Tooltip ("The speed of the motion point")]
		public float velocity = 10.0f;
		/// <summary>
		/// The range of the random position in which the motion point can appear
		/// </summary>
		[Tooltip ("The range of the random positions in which the motion point can appear")]
		public float randomPositionRange = 7.0f;
		/// <summary>
		/// Number of time the motion point will change positions during the level
		/// </summary>
		[Tooltip ("Number of time the motion point will change positions during the level")]
		public int randomTransitions = 5;
		/// <summary>
		/// Interval of time during which the players won't win points when the sphere stays on the collider
		/// </summary>
		[Tooltip ("Interval of time during which the players won't win points when the sphere stays on the collider")]
		public float scoreCooldownTime = 0.5f;
		/// <summary>
		/// A center point around which the motion point rotates
		/// </summary>
		[Tooltip ("A center point around which the motion point rotates")]
		public Vector2 centrePos = new Vector2 (0.0f, 0.0f);

		private bool _cooldown = false;
		private Coroutine _coroutineHandle;
		private GameObject _progressBar;
		private Vector2 _tempPos;

		#region IInteractable implementation

		[Server]
		public void OnEnterEffect (SphereCDM sphere)
		{
			if (GameManager.singleton.isPlaying) { 
				GameManager.singleton.AddToScore ();
				_coroutineHandle = StartCoroutine (ScoreCooldown ());
			}
			RpcMotionPointEvent ();
		}

		[Server]
		public void OnStayEffect (SphereCDM sphere)
		{
			if (GameManager.singleton.isPlaying) {
				if (!_cooldown) {
					GameManager.singleton.AddToScore ();
					_coroutineHandle = StartCoroutine (ScoreCooldown ());
				}
			}
		}

		[Server]
		public void OnExitEffect (SphereCDM sphere)
		{
			StopCoroutine (_coroutineHandle);
		}

		#endregion

		[ServerCallback]
		void Start ()
		{
			StartMovePattern ();
		}

		[Server]
		void StartMovePattern ()
		{
			switch (GameManager.singleton.GetCurrentLevel ().motionMode) {
			case MotionMode.Random:
				StartCoroutine (RandomPattern ());
				break;
			case MotionMode.Cos:
				StartCoroutine (CosPattern ());
				break;
			case MotionMode.Rotate:
				StartCoroutine (RotationPattern ());
				break;
			}
		}

		IEnumerator RandomPattern ()
		{
			float timeInterval = GameManager.singleton.GetCurrentLevel ().time / randomTransitions;
			while (true) {
				if (GameManager.singleton.isPlaying) {
					var pos = new Vector2 (Random.Range (-randomPositionRange, randomPositionRange),
						         Random.Range (-randomPositionRange, randomPositionRange));
					Debug.Log (pos);
					this.transform.position = pos;
				}
				yield return new WaitForSeconds (timeInterval);
			}
		}

		[Server]
		IEnumerator CosPattern ()
		{
			while (true) {
				if (GameManager.singleton.isPlaying) {
					var pos = new Vector2 (0.0f, amplitude * Mathf.Cos (6.24f * Time.fixedTime / velocity));
					Debug.Log (pos);
					this.transform.position = pos;
				}
				yield return new WaitForFixedUpdate ();
			}
		}

		[Server]
		IEnumerator RotationPattern ()
		{
			while (true) {
				if (GameManager.singleton.isPlaying) {
					this.transform.RotateAround (centrePos, Vector3.forward * 10f, velocity * Time.fixedDeltaTime);
				}
				yield return new WaitForFixedUpdate ();
			}
		}

		[Server]
		IEnumerator ScoreCooldown ()
		{
			_cooldown = true;
			yield return new WaitForSeconds (scoreCooldownTime);
			_cooldown = false;
		}

		[ClientRpc]
		void RpcMotionPointEvent()
		{
			if (onMotionPointEnter != null)
				onMotionPointEnter ();
		}
	}
}