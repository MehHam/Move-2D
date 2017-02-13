using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

[RequireComponent (typeof(ProgressRadialBehaviour))]
public class MotionCriterion : MonoBehaviour
{
	public GameObject pointFollow;
	public GameObject sphereCDM;

	void Start ()
	{
		pointFollow = GameObject.FindGameObjectWithTag ("PointFollow");
		sphereCDM = GameObject.FindGameObjectWithTag ("SphereCDM");
	}

	void Update ()
	{
		var progressBar = this.GetComponent<ProgressRadialBehaviour> ();
		int criterion = (int)xiSquareCriterion (sphereCDM, pointFollow);
		progressBar.Value = criterion;
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
