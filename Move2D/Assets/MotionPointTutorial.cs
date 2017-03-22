using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D {
	public class MotionPointTutorial : Tutorial {
		Transform _pointFollow = null;

		protected override void OnEnable ()
		{
			MotionPointFollow.onMotionPointEnter += OnMotionPointEnter;
			base.OnEnable ();
		}

		protected override void OnDisable ()
		{
			MotionPointFollow.onMotionPointEnter -= OnMotionPointEnter;
			base.OnDisable ();
		}

		void OnMotionPointEnter ()
		{
			Deactivate ();
			NextTutorial ();
		}

		protected override void Init ()
		{
			_pointFollow = GameObject.FindGameObjectWithTag ("PointFollow").transform;
		}

		protected override void Update()
		{
			if (_activated) {
				var viewportPos = Camera.main.WorldToViewportPoint (_pointFollow.position);
				var rectTransform = this.GetComponentInParent<Canvas> ().GetComponent<RectTransform> ();
				var pos = new Vector2 ((viewportPos.x * rectTransform.sizeDelta.x) - (rectTransform.sizeDelta.x * 0.5f), (viewportPos.y * rectTransform.sizeDelta.y) - (rectTransform.sizeDelta.y * 0.5f));
				this.GetComponent<RectTransform> ().anchoredPosition = pos;
			}
		}

		#region implemented abstract members of Tutorial
		protected override TutorialType GetTutorialType ()
		{
			return TutorialType.MotionPoint;
		}
		#endregion
}
}
