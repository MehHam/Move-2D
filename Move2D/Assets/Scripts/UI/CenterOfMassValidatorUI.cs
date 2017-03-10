using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CanvasGroup))]
public class CenterOfMassValidatorUI : NetworkBehaviour {
	public RangeScoreList rangeScoreList;
	public float cooldownDuration = 5.0f;

	private float _cooldownTime;
	private GameObject _sphereCDM;
	private GameObject _motionPointFollow;
	[SyncVar] private bool _isCooldown;

	void OnEnable()
	{
		GameManager.onLevelStarted += OnLevelStarted;
	}

	void OnDisable()
	{
		GameManager.onLevelStarted -= OnLevelStarted;
	}

	void OnLevelStarted ()
	{
		if (GameManager.singleton.GetCurrentLevel ().gameMode == Level.GameMode.MotionPointFollow
			&& GameManager.singleton.GetCurrentLevel ().sphereVisibility != Level.SphereVisibility.Visible) {
			_sphereCDM = GameObject.FindGameObjectWithTag ("SphereCDM");
			_motionPointFollow = GameObject.FindGameObjectWithTag ("PointFollow");
		}
	}

	[Server]
	public void ServerGuess()
	{
		_cooldownTime = Time.time;
		_isCooldown = true;
		var range = 100.0f - _sphereCDM.GetComponent<SpherePhysics> ().XISquareCriterion (_motionPointFollow.transform.position);
		foreach (var rangeScore in rangeScoreList.rangeScores) {
			if (rangeScore.IsInRange (range)) {
				Debug.Log (rangeScore.minRange + " " + rangeScore.maxRange + " " + rangeScore.score);
				GameManager.singleton.AddToScore (rangeScore.GetScore (range));
			}
		}
	}


	public void TryGuess()
	{
		_cooldownTime = Time.time;
		CustomNetworkLobbyManager.singleton.client.connection.playerControllers[0].gameObject.GetComponent<Player>().CmdTryGuess ();
	}

	bool IsActivated()
	{
		return (GameManager.singleton != null
			&& GameManager.singleton.isPlaying 
			&& GameManager.singleton.GetCurrentLevel ().gameMode == Level.GameMode.MotionPointFollow
			&& GameManager.singleton.GetCurrentLevel ().sphereVisibility != Level.SphereVisibility.Visible);	
	}

	void Update()
	{
		this.GetComponent<CanvasGroup> ().alpha = IsActivated() ? 1 : 0;
		this.GetComponent<CanvasGroup> ().interactable = IsActivated() && !_isCooldown;
		this.GetComponent<CanvasGroup> ().blocksRaycasts = IsActivated();
		if (isServer && _cooldownTime + cooldownDuration <= Time.time)
			_isCooldown = false;
	}
}
