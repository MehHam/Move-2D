using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Move2D
{
	public class LifeBarUi : MonoBehaviour
	{
		public float fillSpeed;
		public float[] fillSteps;

		bool _enabled = false;

		void OnEnable ()
		{
			GameManager.onLevelStarted += OnLevelStarted;
		}

		void OnDisable ()
		{
			GameManager.onLevelStarted -= OnLevelStarted;
		}
			
		void Awake()
		{
			this.GetComponent<Animator> ().SetInteger ("Life", GameManager.maxLife);
		}

		void OnLevelStarted ()
		{
			_enabled = true;
		}

		void Update ()
		{
			if (_enabled) {
				this.GetComponent<Image> ().fillAmount =
					Mathf.Lerp (this.GetComponent<Image> ().fillAmount,
						fillSteps [GameManager.singleton.life],
						Time.deltaTime * fillSpeed);
				this.GetComponent<Animator> ().SetInteger ("Life", GameManager.singleton.life);
			}
		}
	}
}
