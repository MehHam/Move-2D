using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using ProgressBar;
using UnityEngine.UI;

public class MotionPointFollow : NetworkBehaviour, IInteractable
{
	public enum MotionMode 
	{
		Random,
		Cos,
		Rotate
	}

	[SyncVar] private Vector3 _syncposFollow;
	[SyncVar] private int _syncBarValue;
	public GameObject sphereCDM;

	public int amplitude = 8;
	public float velocity = 10.0f;
	public float randomPositionRange = 7.0f;
	public int randomTransitions = 5;
	public float scoreCooldownTime = 0.5f;
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
		sphereCDM = GameObject.FindGameObjectWithTag ("SphereCDM");
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