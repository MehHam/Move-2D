using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	public class ExitTutorial : Tutorial
	{
		protected override void OnEnable ()
		{
			Exit.onExitEnter += OnExitEnter;
			base.OnEnable ();
		}

		protected override void OnDisable ()
		{
			Exit.onExitEnter -= OnExitEnter;
			base.OnDisable ();
		}

		void OnExitEnter ()
		{
			Deactivate ();
		}

		protected override TutorialType GetTutorialType ()
		{
			return TutorialType.Exit;
		}
	}
}
