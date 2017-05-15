using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

[RequireComponent(typeof(Text))]
public class TextTyper : MonoBehaviour {
	public float letterCooldown = 0.2f;
	Text _text;
	string _message;

	void Start()
	{
		_text = this.GetComponent<Text> ();
		_message = _text.text;
		_text.text = new string(' ', _message.Length);
	}

	public void StartTyping()
	{
		StartCoroutine (TypeText ());
	}

	public void StopTyping()
	{
		StopCoroutine (TypeText ());
	}

	IEnumerator TypeText() {
		for (int i = 0; i < _message.Length; i++) {
			var builder = new StringBuilder (_text.text);
			builder[i] = _message[i];
			_text.text = builder.ToString ();
			yield return new WaitForSeconds (letterCooldown);
		}
	}
}
