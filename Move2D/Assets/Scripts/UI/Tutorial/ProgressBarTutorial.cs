using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	public class ProgressBarTutorial : Tutorial
	{
		protected override void OnEnable ()
		{
			LevelManager.onScoreReached += OnScoreReached;
			base.OnEnable ();
		}

		protected override void OnDisable ()
		{
			LevelManager.onScoreReached -= OnScoreReached;
			base.OnDisable ();
		}

		void OnScoreReached ()
		{
			if (_activated)
				Deactivate ();
		}

		#region implemented abstract members of Tutorial
		protected override TutorialType GetTutorialType ()
		{
			return TutorialType.ProgressBar;
		}
		#endregion
	}
}
