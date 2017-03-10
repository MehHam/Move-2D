using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// A Score Popup. To be created whenever the player score increases or decreases
/// </summary>
[RequireComponent(typeof(TextMesh))]
public class ScorePopupUI : MonoBehaviour {
	public TextMesh				text;
	/// <summary>
	/// Speed of the score popup
	/// </summary>
	[Tooltip("Speed of the score popup")]
	public float			speed = 0.1f;
	/// <summary>
	/// Time until the score popup is destroyed
	/// </summary>
	[Tooltip("Time until the score popup is destroyed")]
	public float			fadeOutTime = 2.0f;


	void Awake () {
		text = GetComponent<TextMesh> ();
		Destroy (this.gameObject, this.fadeOutTime);
	}

	void Destroy()
	{
		Destroy (gameObject);
		Destroy (this);
	}

	// Update is called once per frame
	void Update () {
		this.transform.Translate(Vector3.up * speed);
		speed = Mathf.Lerp (speed, 0, 0.1f);
		this.text.color = new Color(this.text.color.r, this.text.color.g, this.text.color.b, Mathf.Lerp (this.text.color.a, 0, 0.2f));
	}
}
