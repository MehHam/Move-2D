using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <summary>
/// Set the player mass and displays it
/// </summary>
[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(CanvasGroup))]
public class MoveSliderUI : MonoBehaviour {
	/// <summary>
	/// Text to display the current slider value
	/// </summary>
	public Text text;

	Player _player;
	IEnumerator Start()
	{
		SetVisibility ();
		do {
			yield return new WaitForEndOfFrame();
			_player = FindLocalPlayer ();
		} while (_player == null);
		GetComponent<Slider> ().value = _player.mass;
		GetComponent<Slider>().onValueChanged.AddListener(delegate {OnValueChanged();});
	}

	void OnValueChanged()
	{
		_player.CmdSetMass (GetComponent<Slider> ().value);
	}

	Player FindLocalPlayer()
	{
		foreach (var player in GameObject.FindObjectsOfType<Player>()) {
			if (player.isLocalPlayer)
				return player;
		}
		return null;
	}

	void SetVisibility()
	{
		var massModification = GameManager.singleton.GetCurrentLevel ().massModification;
		this.GetComponent<CanvasGroup> ().interactable = massModification;
		this.GetComponent<CanvasGroup> ().blocksRaycasts = massModification;
		this.GetComponent<CanvasGroup> ().alpha = massModification ? 1.0f : 0.0f;
	}

	void Update()
	{
		this.GetComponent<Slider> ().interactable = !GameManager.singleton.paused;
		SetVisibility ();
		text.text = (_player != null) ? "Mass: " + this._player.mass.ToString("0.00") : "";
	}
}
