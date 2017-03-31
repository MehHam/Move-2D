using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Move2D
{
	public class GyroscopeUI : MonoBehaviour
	{
		void Awake()
		{
			if (SystemInfo.supportsGyroscope) {
				GetComponent<Toggle> ().isOn = GameManager.singleton.gyroscope;	
				GetComponent<Toggle> ().onValueChanged.AddListener (OnValueChanged);
				this.GetComponent<CanvasGroup> ().alpha = 1;
				this.GetComponent<CanvasGroup> ().interactable = true;
				this.GetComponent<CanvasGroup> ().blocksRaycasts = true;
			} else {
				this.GetComponent<CanvasGroup> ().alpha = 0;
				this.GetComponent<CanvasGroup> ().interactable = false;
				this.GetComponent<CanvasGroup> ().blocksRaycasts = false;
			}
		}

		void OnValueChanged (bool value)
		{
			GameManager.singleton.gyroscope = value;
		}
	}
}
