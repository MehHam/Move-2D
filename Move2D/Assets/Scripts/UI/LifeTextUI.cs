using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Move2D
{
	public class LifeTextUI : MonoBehaviour
	{
		/// <summary>
		/// The base text to be displayed
		/// </summary>
		public string baseText = "";
		// Update is called once per frame
		void Update ()
		{
			this.GetComponent<Text> ().text = baseText +
				((GameManager.singleton == null) ? "/"
					: new string('o', GameManager.singleton.life));
		}
	}
}
