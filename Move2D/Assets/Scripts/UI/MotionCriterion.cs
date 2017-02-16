using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

[RequireComponent (typeof(ProgressRadialBehaviour))]
public class MotionCriterion : MonoBehaviour
{
	private bool _active;
	private GameObject _pointFollow;
	private GameObject _sphereCDM;

	void OnEnable()
	{
		GameManager.OnLevelStarted += OnLevelStarted;
	}

	void OnDisable()
	{
		GameManager.OnLevelStarted -= OnLevelStarted;
	}

	void OnLevelStarted()
	{
		_active = (GameManager.singleton.GetCurrentLevel ().gameMode == GameManager.GameMode.MotionPointFollow);
		if (_active) {
			this.transform.parent.gameObject.GetComponent<CanvasGroup> ().alpha = 1;
			_pointFollow = GameObject.FindGameObjectWithTag ("PointFollow");
			_sphereCDM = GameObject.FindGameObjectWithTag ("SphereCDM");
		} else {
			this.transform.parent.gameObject.GetComponent<CanvasGroup> ().alpha = 0;
		}
	}

	void Update ()
	{
		if (_active)
		{
			var criterion = xiSquareCriterion (_sphereCDM, _pointFollow);
			this.GetComponent<ProgressRadialBehaviour>().Value = criterion;
		}
	}

	float xiSquareCriterion (GameObject sphereCDM, GameObject pointFollow)
	{
		float criterion;
		int radiusCriterion = 20;

		criterion = Vector3.SqrMagnitude (sphereCDM.transform.position - pointFollow.transform.position) /
			Vector3.SqrMagnitude (new Vector3 (0, 0, radiusCriterion));

		return 100 * (1.0f - Mathf.Max(criterion, 0.01f));
	}
}
