using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	public class ProgressBarTutorial : Tutorial
	{
		#region implemented abstract members of Tutorial
		protected override TutorialType GetTutorialType ()
		{
			return TutorialType.ProgressBar;
		}
		#endregion
	}
}
