using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Move2D
{
	public enum TutorialType
	{
		None,
		Exit,
		Pickup,
		ExitBis,
		MotionPoint,
		TimeWarning,
		MassSlider,
		ProgressBar,
		Controls,
		LifeLoss,
	}

	[RequireComponent (typeof(CanvasGroup))]
	public abstract class Tutorial : MonoBehaviour
	{
		/// <summary>
		/// Gets the type of this tutorial
		/// </summary>
		/// <value>The type of this tutorial.</value>
		public TutorialType type { get { return GetTutorialType (); } }

		/// <summary>
		/// The type of the next tutorial
		/// </summary>
		public TutorialType nextTutorial = TutorialType.None;
		/// <summary>
		/// Shows the tutorial when it's activated
		/// </summary>
		public bool showOnActivate = true;
		/// <summary>
		/// Whether this tutorial has a timer or not
		/// </summary>
		public bool hasTimer = true;
		#if UNITY_EDITOR
		[HideWhenFalseAttribute("hasTimer")]
		#endif 
		/// <summary>
		/// If this tutorial has a timer, how much time is left until he's deactivated
		/// </summary>
		public float timer;

		protected bool _activated = false;
		protected bool _blocksRaycast;
		protected bool _interactable;

		protected virtual void OnEnable ()
		{
		}

		protected virtual void OnDisable ()
		{
		}

		protected virtual void Start ()
		{
			_blocksRaycast = this.GetComponent<CanvasGroup> ().blocksRaycasts;
			_interactable = this.GetComponent<CanvasGroup> ().interactable;
		}

		protected virtual void Update ()
		{
		}

		protected virtual void OnActivate()
		{
		}

		protected virtual void OnDeactivate()
		{
		}

		protected void Hide()
		{
			this.GetComponent<CanvasGroup> ().alpha = 0;
			this.GetComponent<CanvasGroup> ().interactable = false;
			this.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		}

		/// <summary>
		/// Hide this tutorial
		/// </summary>
		public virtual void Deactivate ()
		{
			_activated = false;
			this.GetComponent<CanvasGroup> ().alpha = 0;
			this.GetComponent<CanvasGroup> ().interactable = false;
			this.GetComponent<CanvasGroup> ().blocksRaycasts = false;
			OnDeactivate ();
		}

		protected void Show()
		{
			if (hasTimer)
				StartCoroutine (Countdown());
			this.GetComponent<CanvasGroup> ().alpha = 1;
			this.GetComponent<CanvasGroup> ().interactable = _interactable;
			this.GetComponent<CanvasGroup> ().blocksRaycasts = _blocksRaycast;
		}

		/// <summary>
		/// Show this tutorial
		/// </summary>
		public virtual void Activate (bool forceShow = false)
		{
			_activated = true;
			if (showOnActivate || forceShow) {
				Show ();
			}
			OnActivate ();
		}

		protected virtual IEnumerator Countdown()
		{
			yield return new WaitForSeconds (timer);
			if (_activated) {
				Deactivate ();
				NextTutorial ();
			}
		}

		/// <summary>
		/// Asks the TutorialManager to show the next tutorial
		/// </summary>
		protected virtual void NextTutorial ()
		{
			this.GetComponentInParent<TutorialManager> ().ActivateTutorial (nextTutorial, true);
		}

		protected abstract TutorialType GetTutorialType ();
	}
}