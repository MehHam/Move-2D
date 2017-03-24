using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D {
	public class MassSliderTutorial : Tutorial {
		#region implemented abstract members of Tutorial
		protected override void OnEnable() {
			MoveSliderUI.onSliderValueChanged += OnSliderValueChanged;
			MassZoneEnter.onMassZoneEnter += OnMassZoneEnter;
			MassZoneExit.onMassZoneExit += OnMassZoneExit;
		}

		protected override void OnDisable() {
			MoveSliderUI.onSliderValueChanged -= OnSliderValueChanged;
			MassZoneEnter.onMassZoneEnter -= OnMassZoneEnter;
			MassZoneExit.onMassZoneExit -= OnMassZoneExit;
		}

		void OnMassZoneEnter ()
		{
			if (_activated)
				Show ();
		}

		void OnMassZoneExit ()
		{
			if (_activated)
				Deactivate ();
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
