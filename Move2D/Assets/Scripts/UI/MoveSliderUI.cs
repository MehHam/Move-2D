using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Move2D
{
	/// <summary>
	/// Set the player mass and displays it
	/// </summary>
	[RequireComponent (typeof(Slider))]
	[RequireComponent (typeof(CanvasGroup))]
	public class MoveSliderUI : MonoBehaviour
	{
		public delegate void MoveSliderHandler(float value);
		public static event MoveSliderHandler onSliderValueChanged;
		/// <summary>
		/// Text to display the current slider value
		/// </summary>
		public Text text;

		Player _player;

		void OnEnable()
		{
			GameManager.onLevelStarted += OnLevelStarted;
			GameManager.onRespawn += OnRespawn;
			MassZoneEnter.onMassZoneEnter += OnMassZoneEnter;
			MassZoneExit.onMassZoneExit += OnMassZoneExit;
		}

		void OnDisable()
		{
			GameManager.onLevelStarted -= OnLevelStarted;
			GameManager.onRespawn -= OnRespawn;
			MassZoneEnter.onMassZoneEnter -= OnMassZoneEnter;
			MassZoneExit.onMassZoneExit -= OnMassZoneExit;
		}
			
		void OnLevelStarted ()
		{
			SetVisibility (GameManager.singleton.GetCurrentLevel ().massModification);
			OnRespawn ();
		}

		void OnRespawn()
		{
			_player = FindLocalPlayer ();
			GetComponent<Slider> ().value = _player.mass;
			GetComponent<Slider> ().onValueChanged.AddListener (delegate {
				OnValueChanged ();
			});
		}
			
		void OnMassZoneEnter ()
		{
			SetVisibility (true);
		}
			
		void OnMassZoneExit ()
		{
			SetVisibility (false);
		}
			
		void OnValueChanged ()
		{
			_player.CmdSetMass (GetComponent<Slider> ().value);
			if (onSliderValueChanged != null)
				onSliderValueChanged (GetComponent<Slider> ().value);
		}

		Player FindLocalPlayer ()
		{
			foreach (var player in GameObject.FindObjectsOfType<Player>()) {
				if (player.isLocalPlayer)
					return player;
			}
			return null;
		}

		void SetVisibility (bool massModification)
		{
			this.GetComponent<CanvasGroup> ().interactable = massModification;
			this.GetComponent<CanvasGroup> ().blocksRaycasts = massModification;
			this.GetComponent<CanvasGroup> ().alpha = massModification ? 1.0f : 0.0f;
		}

		void Update ()
		{
			this.GetComponent<Slider> ().interactable = GameManager.singleton.isPlaying;
			text.text = (_player != null) ? "Mass: " + this._player.mass.ToString ("0.00") : "";
		}
	}
}
