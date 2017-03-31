using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Move2D
{
	/// <summary>
	/// Manages the level-specific work like level stats etc.
	/// </summary>
	public class LevelManager : MonoBehaviour
	{
		public static LevelManager singleton;

		public delegate void LevelManagerHandler ();

		public static event LevelManagerHandler onScoreReached;

		int _startingScore = 0;
		bool _scoreReached = false;

		/// <summary>
		/// How much of the total ratio of pickup the player is expected to get in order to open the exit door
		/// </summary>
		public const float pickupScoreRatio = 2.0f / 3.0f;
		/// <summary>
		/// How much of the maximum amount of point the player is expected to get in order to open the exit door with a motion point.
		/// </summary>
		public const float motionPointScoreRatio = 1.0f / 20.0f;

		/// <summary>
		/// Has the level started yet ?
		/// </summary>
		/// <value><c>true</c> if the level has started; otherwise, <c>false</c>.</value>
		public bool levelHasStarted { get; private set; }

		/// <summary>
		/// Gets the ratio of progress before the players reach the score prerequisite
		/// </summary>
		/// <value>The score prerequisite progress.</value>
		public float scorePrerequisiteProgress { get { return InternalScorePrerequisiteProgress (); } }

		/// <summary>
		/// Gets the score prerequisite in order to open the exit door
		/// </summary>
		/// <value>The score prerequisite.</value>
		public int scorePrerequisite { get { return InternalScorePrerequisite (); } }

		/// <summary>
		/// Gets the total score expected should the players take all the pickups
		/// </summary>
		/// <value>The pickup total score.</value>
		public int pickupTotalScore { get; private set; }

		/// <summary>
		/// Gets the score total expected should the player stay on the motionPoint 100% of the time
		/// </summary>
		/// <value>The total score.</value>
		public int motionPointTotalScore { get; private set; }

		/// <summary>
		/// Gets the number of pickups when starting the scene.
		/// </summary>
		/// <value>The starting pickup count.</value>
		public int pickupStartingCount { get; private set; }

		/// <summary>
		/// Gets the number of pickups currently in the scene.
		/// </summary>
		/// <value>The pickup count.</value>
		public int pickupCount { get { return InternalPickupCount (); } }

		/// <summary>
		/// Gets a value indicating whether the score threshold to open the exit door has been reached.
		/// </summary>
		/// <value><c>true</c> if is score reached; otherwise, <c>false</c>.</value>
		public bool hasScoreReached { get { return InternalIsScoreReached (); } }

		void OnEnable ()
		{
			GameManager.onLevelStarted += OnLevelStarted;
		}

		void OnDisable ()
		{
			GameManager.onLevelStarted -= OnLevelStarted;
		}

		void OnLevelStarted ()
		{
			_startingScore = GameManager.singleton.score;
			pickupTotalScore = InternalPickupTotalScore ();
			pickupStartingCount = pickupCount;
			motionPointTotalScore = (GameManager.singleton.GetCurrentLevel ().spawnMotionPointFollow) ?
				InternalMotionPointTotalScore ()
				: 0;
			levelHasStarted = true;
		}

		void Awake ()
		{
			if (singleton != null && singleton != this) {
				Destroy (this.gameObject);
			} else {
				LevelManager.singleton = this;
			}
		}

		int InternalMotionPointTotalScore ()
		{
			var motionPoint = GameObject.FindGameObjectWithTag ("PointFollow");
			return (int)(
			    GameManager.singleton.GetCurrentLevel ().time
			    / motionPoint.GetComponent<MotionPointFollow> ().scoreCooldownTime
			);
		}

		int InternalPickupTotalScore ()
		{
			int count = 0;
			foreach (var pickup in GetComponentsInChildren<Pickup>()) {
				count += pickup.scoreValue;
			}
			return count;
		}

		int InternalScorePrerequisite ()
		{
			return _startingScore
			+ (int)(
			    (float)pickupTotalScore * pickupScoreRatio
			    + (float)motionPointTotalScore * motionPointScoreRatio
			);
		}

		int InternalPickupCount ()
		{
			return GetComponentsInChildren<Pickup> ().Length;
		}

		float InternalScorePrerequisiteProgress ()
		{
			if (scorePrerequisite - _startingScore == 0)
				return 1.0f;
			return Mathf.Min (1.0f,
				(
				    (float)(GameManager.singleton.score - _startingScore)
				    / (float)(scorePrerequisite - _startingScore)
				)
			);
		}

		bool InternalIsScoreReached ()
		{
			return GameManager.singleton.score >= scorePrerequisite;
		}

		void Update ()
		{
			if (levelHasStarted && !_scoreReached && hasScoreReached) {
				_scoreReached = true;
				if (onScoreReached != null)
					onScoreReached ();
			}
		}
	}
}
