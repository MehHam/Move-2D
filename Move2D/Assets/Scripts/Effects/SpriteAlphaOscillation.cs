using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAlphaOscillation : MonoBehaviour {
	public float maxAlpha = 1.0f;
	public float speed = 0.0025f;
	bool _down = false;
	// Update is called once per frame

	void Start()
	{
		var color = GetComponent<SpriteRenderer> ().color;
		GetComponent<SpriteRenderer> ().color = new Color (color.r, color.g, color.b, maxAlpha);
	}

	void Update () {
		var color = GetComponent<SpriteRenderer> ().color;
		if (color.a >= maxAlpha)
			_down = true;
		if (color.a <= 0)
			_down = false;
		GetComponent<SpriteRenderer> ().color = new Color (color.r, color.g, color.b, Mathf.Min(color.a + (_down ? -speed : speed), maxAlpha));
	}
}
