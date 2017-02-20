using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using ProgressBar;
using UnityEngine.UI;

/// <summary>
/// A special type of interactable object. It moves around the scene with different patterns, give points to the players if the sphere
/// collides with it.
/// </summary>
public class MotionPointFollow : NetworkBehaviour, IInteractable
{
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
	public int amplitude = 8;
	/// <summary>
	/// The speed of the motion point
	/// </summary>
	public float velocity = 10.0f;
	/// <summary>
	/// The range of the random position in which the motion point can appear
	/// </summary>
	public float randomPositionRange = 7.0f;
	/// <summary>
	/// Number of time the motion point will change positions during the level
	/// </summary>
	public int randomTransitions = 5;
	/// <summary>
	/// Interval of time during which the players won't win points when the sphere stays on the collider
	/// </summary>
	public float scoreCooldownTime = 0.5f;
	/// <summary>
	/// A center point around which the motion point rotates
	/// </summary>
	public Vector2 centrePos = new Vector2(0.0f, 0.0f);

	private bool _cooldown = false;
	private CoroutineHandle _coroutineHandle;
	private GameObject _progressBar;
	private Vector2 _tempPos;

	#region IInteractable implementation
	[Server]
	public void OnEnterEffect (SphereCDM sphere)
	{
		GameManager.singleton.IncreaseScore ();
		_coroutineHandle = Timing.RunCoroutine (ScoreCooldown (), Segment.FixedUpdate);
	}

	[Server]
	public void OnStayEffect (SphereCDM sphere)
	{
		if (!_cooldown) {
			GameManager.singleton.IncreaseScore ();
			_coroutineHandle = Timing.RunCoroutine (ScoreCooldown(), Segment.FixedUpdate);
		}
	}

	[Server]
	public void OnExitEffect (SphereCDM sphere)
	{
		Timing.KillCoroutines (_coroutineHandle);
	}
	#endregion

	[ServerCallback]
	void Start ()
	{
		if (GameManager.singleton.GetCurrentLevel ().gameMode != GameManager.GameMode.MotionPointFollow) {
			this.enabled = false;
			return;
		}
		StartMovePattern ();
	}

	[Server]
	void StartMovePattern()
	{
		switch (GameManager.singleton.GetCurrentLevel ().motionMode) {
			case MotionMode.Random:
				Timing.RunCoroutine (RandomPattern (), Segment.FixedUpdate);
				break;
			case MotionMode.Cos:
				Timing.RunCoroutine (CosPattern (), Segment.FixedUpdate);
				break;
			case MotionMode.Rotate:
				Timing.RunCoroutine (RotationPattern (), Segment.FixedUpdate);
				break;
		}
	}
		
	IEnumerator<float> RandomPattern()
	{
		float timeInterval = GameManager.singleton.GetCurrentLevel ().time / randomTransitions;
		while (true)
		{
			var pos = new Vector2 (Random.Range (-randomPositionRange, randomPositionRange),
				          Random.Range (-randomPositionRange, randomPositionRange));
			Debug.Log (pos);
			this.transform.position = pos;
			yield return Timing.WaitForSeconds (timeInterval);
		}
	}

	[Server]
	IEnumerator<float> CosPattern()
	{
		while (true)
		{
			var pos = new Vector2 (0.0f, amplitude * Mathf.Cos (6.24f * Time.fixedTime / velocity));
			Debug.Log (pos);
			this.transform.position = pos;
			yield return 0.0f;
		}
	}

	[Server]
	IEnumerator<float> RotationPattern()
	{
		while (true)
		{
			this.transform.RotateAround (centrePos, Vector3.forward * 10f, velocity * Time.fixedDeltaTime);
			yield return 0.0f;
		}
	}

	[Server]
	IEnumerator<float> ScoreCooldown()
	{
		_cooldown = true;
		yield return Timing.WaitForSeconds (scoreCooldownTime);
		_cooldown = false;
	}
}