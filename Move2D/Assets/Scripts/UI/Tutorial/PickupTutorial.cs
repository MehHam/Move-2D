using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	public class PickupTutorial : Tutorial
	{
		protected override void OnEnable ()
		{
			Pickup.onPickupEnter += OnPickupEnter;
			base.OnEnable ();
		}

		void OnPickupEnter ()
		{
			if (_activated) {
				Deactivate ();
				NextTutorial ();
			}
		}

		protected override void OnDisable ()
		{
			Pickup.onPickupEnter -= OnPickupEnter;
			base.OnDisable ();
		}

		protected override TutorialType GetTutorialType ()
		{
			return TutorialType.Pickup;
		}
	}
}
