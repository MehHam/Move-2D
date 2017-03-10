using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerLine : MonoBehaviour {
	/// <summary>
	/// The first player object
	/// </summary>
	[Tooltip("The first player object")]
	public GameObject player1;
	/// <summary>
	/// The second player object
	/// </summary>
	[Tooltip("The second player object")]
	public GameObject player2;
	/// <summary>
	/// The alpha of the line
	/// </summary>
	[Tooltip("The alpha of the line")]
	public float alpha;

	LineRenderer _lineRenderer;

	void Start()
	{
		_lineRenderer = GetComponent<LineRenderer> ();
	}

	void SetColors()
	{
		var startColor = player1.GetComponent<Player> ().color;
		var endColor = player2.GetComponent<Player> ().color;
		_lineRenderer.startColor = new Color (startColor.r, startColor.g, startColor.b, Input.GetKey (KeyCode.Tab) ? alpha : 0.0f);
		_lineRenderer.endColor = new Color (endColor.r, endColor.g, endColor.b, Input.GetKey (KeyCode.Tab) ? alpha : 0.0f);
		_lineRenderer.startWidth = player1.GetComponent<Player> ().mass;
		_lineRenderer.endWidth = player2.GetComponent<Player> ().mass;
		_lineRenderer.material.mainTextureOffset = new Vector2 (Time.time, 0.0f);
	}

	void SetPositions()
	{
		Vector3[] positions = new Vector3[2];
		positions [0] = player1.transform.position;
		positions [1] = player2.transform.position;
		_lineRenderer.numPositions = positions.Length;
		_lineRenderer.SetPositions (positions);
	}

	void LateUpdate()
	{
		SetColors ();
		SetPositions ();
	}
}
