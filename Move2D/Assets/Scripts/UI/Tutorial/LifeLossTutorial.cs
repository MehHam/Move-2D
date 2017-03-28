using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	public class LifeLossTutorial : Tutorial
	{
		protected override void OnEnable ()
		{
			GameManager.onFirstDamage += OnFirstDamage;
			base.OnEnable ();
		}

		protected override void OnDisable ()
		{
			GameManager.onFirstDamage -= OnFirstDamage;
			base.OnDisable ();
		}

		void OnFirstDamage ()
		{
			if (_activated)
				Show ();	
		}

		#region implemented abstract members of Tutorial
		protected override TutorialType GetTutorialType ()
		{
			return TutorialType.LifeLoss;
		}
		#endregion
		
	}
}
