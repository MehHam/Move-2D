using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	/// <summary>
	/// Manages all the tutorials.
	/// </summary>
	public class TutorialManager : MonoBehaviour
	{
		void OnEnable ()
		{
			GameManager.onLevelStarted += OnLevelStarted;
		}

		void OnDisable ()
		{
			GameManager.onLevelStarted -= OnLevelStarted;
		}

		void Start ()
		{
			foreach (var tutorial in this.GetComponentsInChildren<Tutorial> ()) {
				tutorial.Deactivate ();
			}
		}
		/// <summary>
		/// Called when the level starts, activate all the tutorials on the current level list
		/// </summary>
		void OnLevelStarted ()
		{
			foreach (var tutorial in this.GetComponentsInChildren<Tutorial>()) {
				if (GameManager.singleton.GetCurrentLevel ().enabledTutorials.Contains (tutorial.type))
					tutorial.Activate ();
			}
		}
		/// <summary>
		/// Activate a tutorial
		/// </summary>
		/// <param name="type">The type of the tutorial that will activated.</param>
		/// <param name="forceShow">If set to <c>true</c> show the tutorial even if it's not set to showOnActivate.</param>
		public void ActivateTutorial (TutorialType type, bool forceShow = false)
		{
			foreach (var tutorial in this.GetComponentsInChildren<Tutorial>()) {
				if (tutorial.type == type)
					tutorial.Activate (forceShow);
			}
		}

		/// <summary>
		/// Deactivate a tutorial of the given type
		/// </summary>
		/// <param name="type">The type of the tutorial that will be deactivated.</param>
		public void DeactivateTutorial (TutorialType type)
		{
			foreach (var tutorial in this.GetComponentsInChildren<Tutorial>()) {
				if (tutorial.type == type)
					tutorial.Deactivate ();
			}
		}
	}
}