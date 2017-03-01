using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Blinker : NetworkBehaviour {
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

	Color[] _colors;
	IEnumerator _routine;


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
	}

	void FadeIn (float newFadeTime)
	{
		if (_routine != null)
			StopCoroutine (_routine);
		_routine = FadeSequence (newFadeTime);
		StartCoroutine(_routine); 
	}
		
	/// <summary>
	/// Fades the element in
	/// </summary>
	public void FadeIn ()
	{
		FadeIn (fadeInTime);
	}
		
	/// <summary>
	/// Fades the element out
	/// </summary>
	public void FadeOut ()
	{
		FadeOut (fadeOutTime);
	}

	void FadeOut (float newFadeTime)
	{
		if (_routine != null)
			StopCoroutine (_routine);
		_routine = FadeSequence (-newFadeTime);
		StartCoroutine (_routine); 
	}
		
	IEnumerator PulsateRoutine(float duration)
	{
		var color = this.GetComponent<Renderer> ().material.color;
		this.GetComponent<Renderer> ().material.color = new Color (color.r, color.g, color.b, 0.5f);
		float time = 0;
		while (true) {
			FadeIn (0.5f);
			yield return new WaitForSeconds (0.25f);
			time += 0.25f;
			if (duration != 0 && time > duration)
				break;
			FadeOut (0.5f);
			yield return new WaitForSeconds (0.25f);
			if (duration != 0 && time > duration)
				break;
		}
		color = this.GetComponent<Renderer> ().material.color;
		this.GetComponent<Renderer> ().material.color = new Color (color.r, color.g, color.b, 1.0f);
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

	/// <summary>
	/// Start the damage animation
	/// </summary>
	public void DamageAnimation()
	{
		StartCoroutine (PulsateRoutine(0));
	}

	public void DamageAnimation(float duration)
	{
		StartCoroutine (PulsateRoutine (duration));
	}

	/// <summary>
	/// Stop the damage animation
	/// </summary>
	public void StopDamageAnimation()
	{
		StopAllCoroutines ();
		var color = this.GetComponent<Renderer> ().material.color;
		this.GetComponent<Renderer> ().material.color = new Color (color.r, color.g, color.b, 1.0f);
	}


	// ----------------- Client RPC ------------------

	[ClientRpc]
	/// <summary>
	/// Call to the client to fade the sphere in
	/// </summary>
	public void RpcFadeIn()
	{
		this.GetComponent<Blinker>().FadeIn ();
	}

	[ClientRpc]
	/// <summary>
	/// Request to the client to stop the damage animation
	/// </summary>
	public void RpcStopDamageAnimation()
	{
		StopDamageAnimation ();
	}

	[ClientRpc]
	/// <summary>
	/// Request to the client to start the damage animation for a duraction
	/// </summary>
	/// <param name="duration">The duration of the animation.</param>
	public void RpcDamageAnimationTimed(float duration)
	{
		DamageAnimation (duration);
	}

	[ClientRpc]
	/// <summary>
	/// Request to the client to start the damage animation
	/// </summary>
	public void RpcDamageAnimation()
	{
		DamageAnimation ();
	}

	[ClientRpc]
	/// <summary>
	/// Request to the client to blink this instance
	/// </summary>
	public void RpcBlink()
	{
		Blink ();
	}

	[ClientRpc]
	/// <summary>
	/// Call to the client to fade the sphere out
	/// </summary>
	public void RpcFadeOut()
	{
		FadeOut ();
	}
}
