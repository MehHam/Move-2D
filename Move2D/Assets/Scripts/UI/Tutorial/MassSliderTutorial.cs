using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D {
	public class MassSliderTutorial : Tutorial {
		#region implemented abstract members of Tutorial
		protected override void OnEnable() {
			MoveSliderUI.onSliderValueChanged += OnSliderValueChanged;
			MassZone.onMassZoneEnter += OnMassZoneEnter;
		}

		protected override void OnDisable() {
			MoveSliderUI.onSliderValueChanged -= OnSliderValueChanged;
			MassZone.onMassZoneEnter -= OnMassZoneEnter;
		}

		void OnMassZoneEnter ()
		{
			if (_activated)
				Show ();
		}


		void OnSliderValueChanged (float value)
		{
			if (_activated)
				Deactivate ();
		}

		protected override TutorialType GetTutorialType ()
		{
			return TutorialType.MassSlider;
		}
		#endregion
}
}
