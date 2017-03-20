using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Move2D
{
	[RequireComponent (typeof(CanvasGroup))]
	public class CenterOfMassValidatorUI : NetworkBehaviour
	{
		public RangeScoreList rangeScoreList;
		public float cooldownDuration = 5.0f;

		private string _baseText;
		private GameObject _sphereCDM;
		private GameObject _motionPointFollow;
		private float _cooldownTime;
		[SyncVar] private bool _isCooldown;

		void OnEnable ()
		{
			GameManager.onLevelStarted += OnLevelStarted;
		}

		void OnDisable ()
		{
			GameManager.onLevelStarted -= OnLevelStarted;
		}

		void Start ()
		{
			_baseText = this.GetComponentInChildren<Text> ().text;
		}

		/// <summary>
		/// Called when the level starts
		/// </summary>
		void OnLevelStarted ()
		{
			if (GameManager.singleton.GetCurrentLevel ().gameMode == Level.GameMode.MotionPointFollow
			   && GameManager.singleton.GetCurrentLevel ().sphereVisibility != Level.SphereVisibility.Visible) {
				_sphereCDM = GameObject.FindGameObjectWithTag ("SphereCDM");
				_motionPointFollow = GameObject.FindGameObjectWithTag ("PointFollow");
			}
		}

		/// <summary>
		/// Called on the server by a player objet. Tries to guess whether the sphere is close to the motion point
		/// </summary>
		[Server]
		public void ServerGuess ()
		{
			_cooldownTime = Time.time;
			_isCooldown = true;
			RpcCooldownTime ();
			var range = 100.0f - _sphereCDM.GetComponent<SpherePhysics> ().XISquareCriterion (_motionPointFollow.transform.position);
			foreach (var rangeScore in rangeScoreList.rangeScores) {
				if (rangeScore.IsInRange (range)) {
					Debug.Log (rangeScore.minRange + " " + rangeScore.maxRange + " " + rangeScore.score);
					GameManager.singleton.AddToScore (rangeScore.GetScore (range));
				}
			}
		}

		/// <summary>
		/// Request the client to set its own cooldown time to synchronize the cooldown timer display
		/// </summary>
		[ClientRpc]
		void RpcCooldownTime ()
		{
			_cooldownTime = Time.time;
		}

		/// <summary>
		/// Call a command on a player character to try to guess if the sphere is close to the motion point
		/// </summary>
		public void TryGuess ()
		{
			_cooldownTime = Time.time;
			CustomNetworkLobbyManager.singleton.client.connection.playerControllers [0].gameObject.GetComponent<Player> ().CmdTryGuess ();
		}

		/// <summary>
		/// Determines whether the validator is visible.
		/// </summary>
		/// <returns><c>true</c> if this instance is visible; otherwise, <c>false</c>.</returns>
		bool IsActivated ()
		{
			return (GameManager.singleton != null
			&& GameManager.singleton.isPlaying
			&& GameManager.singleton.GetCurrentLevel ().gameMode == Level.GameMode.MotionPointFollow
			&& GameManager.singleton.GetCurrentLevel ().sphereVisibility != Level.SphereVisibility.Visible);	
		}

		void Update ()
		{
			this.GetComponent<CanvasGroup> ().alpha = IsActivated () ? 1 : 0;
			this.GetComponent<CanvasGroup> ().interactable = IsActivated () && !_isCooldown;
			this.GetComponent<CanvasGroup> ().blocksRaycasts = IsActivated ();
			if (isServer && _cooldownTime + cooldownDuration <= Time.time)
				_isCooldown = false;
			this.GetComponentInChildren<Text> ().text = 
				(_isCooldown) ?
				((int)(cooldownDuration - (Time.time - _cooldownTime)) + 1).ToString () :
				_baseText;
		}
	}
}