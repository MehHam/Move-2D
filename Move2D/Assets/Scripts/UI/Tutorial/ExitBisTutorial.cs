using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	
	public class ExitBisTutorial : Tutorial
	{
		protected override void OnEnable ()
		{
			Exit.onExitEnter += OnExitEnter;
			LevelManager.onScoreReached += OnScoreReached;
			base.OnEnable ();
		}

		protected override void OnDisable ()
		{
			Exit.onExitEnter -= OnExitEnter;
			LevelManager.onScoreReached -= OnScoreReached;
			base.OnDisable ();
		}

		void OnExitEnter ()
		{
			if (_activated)
				Deactivate ();
		}


		void OnScoreReached ()
		{
			if (_activated) {
				Show ();
			}
		}

		#region implemented abstract members of Tutorial

		protected override TutorialType GetTutorialType ()
		{
			return TutorialType.ExitBis;
		}

		#endregion
	}
}
