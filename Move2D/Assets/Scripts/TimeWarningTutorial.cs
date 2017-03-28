using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	public class TimeWarningTutorial : Tutorial
	{
		protected override void OnEnable ()
		{
			GameManager.onTimeWarning += OnTimeWarning;
			base.OnEnable ();
		}

		protected override void OnDisable ()
		{
			GameManager.onTimeWarning -= OnTimeWarning;
			base.OnDisable ();
		}

		void OnTimeWarning ()
		{
			if (_activated)
				Show ();	
		}

		#region implemented abstract members of Tutorial
		protected override TutorialType GetTutorialType ()
		{
			return TutorialType.TimeWarning;
		}
		#endregion

	}
}
