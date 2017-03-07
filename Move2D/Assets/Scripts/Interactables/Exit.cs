﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Exit : NetworkBehaviour, IEnterInteractable, IExitInteractable {
	/// <summary>
	/// The countdown time in seconds
	/// </summary>
	public const int countdownTime = 5;

	/// <summary>
	/// Whether the countdown is activated or not
	/// </summary>
	[Tooltip("Whether the countdown is activated or not")]
	[SyncVar] public bool countdownActivated = false;
	/// <summary>
	/// How many seconds are left in the countdown
	/// </summary>
	[Tooltip("How many seconds are left in the countdown")]
	[SyncVar] public int timeLeft;

	private Coroutine _countdown;

	IEnumerator ExitCountdown()
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
	void StartExitCountdown()
	{
		this._countdown = StartCoroutine (ExitCountdown ());
	}

	[Server]
	void StopExitCountdown()
	{
		if (this._countdown != null)
			StopCoroutine (_countdown);
		this.countdownActivated = false;
		this.timeLeft = 0;
	}

	public void OnEnterEffect (SphereCDM sphere)
	{
		StartExitCountdown ();
	}

	public void OnExitEffect (SphereCDM sphere)
	{
		StopExitCountdown ();
	}
}