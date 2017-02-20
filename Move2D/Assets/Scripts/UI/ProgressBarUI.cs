using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

/// <summary>
/// Class to be used to compute the distance between the Sphere and the PointFollow into a radial progress
/// </summary>
[RequireComponent (typeof(ProgressRadialBehaviour))]
public class ProgressBarUI : MonoBehaviour
{
	public float fadeInSpeed = 1.0f;
	private bool _active;
	private GameObject _pointFollow;
	private GameObject _sphereCDM;

	IEnumerator Start()
	{
		yield return new WaitForSeconds (0.5f);
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

	// Computing the distance between the sphere and the point as a purcentage
	float xiSquareCriterion (GameObject sphereCDM, GameObject pointFollow)
	{
		float criterion;
		int radiusCriterion = 20;

		criterion = Vector3.SqrMagnitude (sphereCDM.transform.position - pointFollow.transform.position) /
			Vector3.SqrMagnitude (new Vector3 (0, 0, radiusCriterion));

		return 100 * (1.0f - Mathf.Max(criterion, 0.01f));
	}
}
