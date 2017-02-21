using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using MovementEffects;

/// <summary>  
///  General class for the sphere
/// </summary>  
public class SphereCDM : NetworkBehaviour
{
	/// <summary>
	/// How many seconds the sphere waits until it fades
	/// </summary>
	[Tooltip("How many seconds the sphere waits until it fades")]
	public float waitTimeUntilFade = 5.0f;
	/// <summary>
	/// How many seconds the sphere takes to fade in
	/// </summary>
	[Tooltip("How many seconds the sphere takes to fade in")]
	public float fadeInTime = 5.0f;
	/// <summary>
	/// How many seconds the sphere takes to fade out
	/// </summary>
	[Tooltip("How many seconds the sphere takes to fade out")]
	public float fadeOutTime = 5.0f;
	/// <summary>
	/// The duration of the blink.
	/// </summary>
	[Tooltip("The duration of the blink")]
	public float blinkDuration = 0.5f;
	/// <summary>
	/// The maximum alpha of the blink.
	/// </summary>
	[Tooltip("The maximum alpha of the blink")]
	public float maxBlinkAlpha = 0.5f;

	// store colors
	private Color[] _colors;

	// Called whenever the sphere enters a trigger
	[ServerCallback]
	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.GetComponent<IInteractable> () != null)
			other.gameObject.GetComponent<IInteractable> ().OnEnterEffect (this);
	}

	// Called each frame the sphere is inside a trigger
	[ServerCallback]
	void OnTriggerStay2D (Collider2D other)
	{
		if (other.gameObject.GetComponent<IInteractable> () != null)
			other.gameObject.GetComponent<IInteractable> ().OnStayEffect (this);
	}

	// Called whenever the sphere exits a trigger
	[ServerCallback]
	void OnTriggerExit2D (Collider2D other)
	{
		if (other.gameObject.GetComponent<IInteractable> () != null)
			other.gameObject.GetComponent<IInteractable> ().OnExitEffect (this);
	}

	// check the alpha value of most opaque object
	float MaxAlpha()
	{
		float maxAlpha = 0.0f; 
		Renderer[] rendererObjects = GetComponentsInChildren<Renderer>(); 
		foreach (Renderer item in rendererObjects)
		{
			maxAlpha = Mathf.Max (maxAlpha, item.material.color.a); 
		}
		return maxAlpha; 
	}

	// fade sequence
	IEnumerator FadeSequence (float fadingOutTime)
	{
		Debug.Log ("FadeSequenceStarted");
		// log fading direction, then precalculate fading speed as a multiplier
		bool fadingOut = (fadingOutTime < 0.0f);
		float fadingOutSpeed = 1.0f / fadingOutTime; 

		// grab all child objects
		Renderer[] rendererObjects = GetComponentsInChildren<Renderer>(); 
		if (_colors == null)
		{
			//create a cache of colors if necessary
			_colors = new Color[rendererObjects.Length]; 

			// store the original colours for all child objects
			for (int i = 0; i < rendererObjects.Length; i++)
			{
				_colors[i] = rendererObjects[i].material.color; 
			}
		}

		// make all objects visible
		for (int i = 0; i < rendererObjects.Length; i++)
		{
			rendererObjects[i].enabled = true;
		}


		// get current max alpha
		float alphaValue = MaxAlpha();  

		// iterate to change alpha value 
		while ( (alphaValue >= 0.0f && fadingOut) || (alphaValue <= 1.0f && !fadingOut)) 
		{
			alphaValue += Time.deltaTime * fadingOutSpeed;
			Debug.Log (alphaValue);

			for (int i = 0; i < rendererObjects.Length; i++)
			{
				Color newColor = (_colors != null ? _colors[i] : rendererObjects[i].material.color);
				newColor.a = Mathf.Min ( newColor.a, alphaValue ); 
				newColor.a = Mathf.Clamp (newColor.a, 0.0f, 1.0f); 				
				rendererObjects[i].material.SetColor("_Color", newColor) ; 
			}

			yield return null;
		}

		// turn objects off after fading out
		if (fadingOut)
		{
			for (int i = 0; i < rendererObjects.Length; i++)
			{
				rendererObjects[i].enabled = false; 
			}
		}
		Debug.Log ("fade sequence end : " + fadingOut);
	}

	void FadeIn (float newFadeTime)
	{
		StopAllCoroutines ();
		StartCoroutine(FadeSequence(newFadeTime)); 
	}

	void FadeIn ()
	{
		FadeIn (fadeInTime);
	}

	void FadeOut (float newFadeTime)
	{
		StopAllCoroutines ();
		StartCoroutine(FadeSequence(-newFadeTime)); 
	}

	void FadeOut ()
	{
		FadeOut (fadeOutTime);
	}

	[ClientRpc]
	/// <summary>
	/// Request to the client to blink this instance
	/// </summary>
	public void RpcBlink()
	{
		Blink ();
	}

	/// <summary>
	/// Blink this instance.
	/// </summary>
	public void Blink()
	{
		var color = this.GetComponent<Renderer> ().material.color;
		// Only blink if the sphere is already invisible
		if (color.a > 0)
			return;
		this.GetComponent<Renderer> ().material.color = new Color (color.r, color.g, color.b, maxBlinkAlpha);
		FadeOut (blinkDuration);
	}

	IEnumerator FadeAtStartLevel()
	{
		Debug.Log ("Fade At Start Level");
		yield return Timing.WaitForSeconds (waitTimeUntilFade);
		FadeOut ();
	}

	void OnEnable()
	{
		GameManager.OnLevelStarted += OnLevelStarted;
	}

	void OnDisable()
	{
		GameManager.OnLevelStarted -= OnLevelStarted;
	}

	void SphereVisibility()
	{
		var color = this.GetComponent<Renderer> ().material.color;
		switch (GameManager.singleton.GetCurrentLevel ().sphereVisibility) {
			case GameManager.SphereVisibility.Visible:
				this.GetComponent<Renderer> ().material.color =
				new Vector4 (color.r, color.g, color.b, 1.0f);
				break;
			case GameManager.SphereVisibility.FadeAfterStartLevel:
				StartCoroutine(FadeAtStartLevel ());
				break;
			case GameManager.SphereVisibility.Invisible:
				FadeOut ();
				break;
		}
	}

	void OnLevelStarted()
	{
		Debug.Log ("OnLevelStartedCalled");
		SphereVisibility ();
	}
}
