using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Move2D
{
	public enum TutorialType
	{
		Exit,
		Pickup,
		ExitBis,
		MotionPoint,
		Lava,
		Controls,
		None,
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

		public bool showOnActivate = true;

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

		protected virtual void Init()
		{
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
		}

		/// <summary>
		/// Show this tutorial
		/// </summary>
		public virtual void Activate (bool forceShow = false)
		{
			_activated = true;
			if (showOnActivate || forceShow) {
				this.GetComponent<CanvasGroup> ().alpha = 1;
				this.GetComponent<CanvasGroup> ().interactable = _interactable;
				this.GetComponent<CanvasGroup> ().blocksRaycasts = _blocksRaycast;
			}
			Init ();
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