using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEllipsisText : MonoBehaviour {
	string _baseText;
	public float cooldownTime = 0.5f;


	void Start () {
		_baseText = this.GetComponent<Text> ().text;
		this.GetComponent<CanvasGroup> ().alpha = 0;
		StartCoroutine (WaitForConnection());
		StartCoroutine (FadeIn ());
	}

	IEnumerator FadeIn()
	{
		while (this.GetComponent<CanvasGroup> ().alpha < 1) {
			this.GetComponent<CanvasGroup> ().alpha = Mathf.Lerp (this.GetComponent<CanvasGroup> ().alpha, 1, 0.5f * Time.deltaTime);
			yield return new WaitForEndOfFrame ();
		}
	}

	IEnumerator FadeOut()
	{
		while (this.GetComponent<CanvasGroup> ().alpha > 0) {
			this.GetComponent<CanvasGroup> ().alpha = Mathf.Lerp (1, this.GetComponent<CanvasGroup> ().alpha, 50.0f * Time.deltaTime);
			yield return new WaitForEndOfFrame ();
		}
	}

	IEnumerator WaitForConnection()
	{
		while (true)
		{
			this.GetComponent<Text>().text = _baseText;
			yield return new WaitForSeconds(cooldownTime);
			this.GetComponent<Text>().text = _baseText + ".";
			yield return new WaitForSeconds(cooldownTime);
			this.GetComponent<Text>().text = _baseText + "..";
			yield return new WaitForSeconds(cooldownTime);
			this.GetComponent<Text>().text = _baseText + "...";
			yield return new WaitForSeconds(cooldownTime);
		}
	}

	public void Activate()
	{
		StopAllCoroutines ();
		StartCoroutine (WaitForConnection ());
		StartCoroutine (FadeIn ());
	}

	public void Deactivate()
	{
		StopAllCoroutines ();
		this.GetComponent<CanvasGroup> ().alpha = 0;
	}
}
