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

		public const float scoreRatio = 2.0f / 3.0f;
		public bool levelHasStarted { get; private set; }
		public float scorePrerequisiteProgress { get { return InternalScorePrerequisiteProgress(); }}
		public int scorePrerequisite { get { return InternalScorePrerequisite (); } }
		public int pickupTotalScore { get; private set; }
		public int pickupStartingCount { get; private set; }
		public int pickupCount { get { return InternalPickupCount (); }}
		public bool isScoreReached { get { return InternalIsScoreReached (); } }

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
			_startingScore = GameManager.singleton.score;
			pickupTotalScore = InternalPickupTotalScore();
			pickupStartingCount = pickupCount;
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

		int InternalPickupTotalScore()
		{
			int count = 0;
			foreach (var pickup in GetComponentsInChildren<Pickup>()) {
				count += pickup.scoreValue;
			}
			return count;
		}

		int InternalScorePrerequisite()
		{
			return _startingScore + (int)((float)pickupTotalScore * scoreRatio);
		}

		int InternalPickupCount()
		{
			return GetComponentsInChildren<Pickup> ().Length;
		}

		float InternalScorePrerequisiteProgress()
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

		bool InternalIsScoreReached()
		{
			return GameManager.singleton.score >= scorePrerequisite;
		}

		void Update()
		{
			if (levelHasStarted && !_scoreReached && isScoreReached) {
				_scoreReached = true;
				if (onScoreReached != null)
					onScoreReached ();
			}
		}
	}
}
