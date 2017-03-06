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
	/// The duration of the damage animation
	/// </summary>
	[Tooltip("The duration of the damage animation")]
	public float damageDuration = 2.0f;

	// ----------------- Events ------------------

	void OnEnable()
	{
		GameManager.onLevelStarted += OnLevelStarted;
	}

	void OnDisable()
	{
		GameManager.onLevelStarted -= OnLevelStarted;
	}

	void OnLevelStarted()
	{
		Debug.Log ("OnLevelStartedCalled");
		SphereVisibility ();
	}

	// ----------------- Collider Events ------------------


	// Called whenever the sphere enters a trigger
	[ServerCallback]
	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.GetComponent<IEnterInteractable> () != null)
			other.gameObject.GetComponent<IEnterInteractable> ().OnEnterEffect (this);
	}

	// Called each frame the sphere is inside a trigger
	[ServerCallback]
	void OnTriggerStay2D (Collider2D other)
	{
		if (other.gameObject.GetComponent<IStayInteractable> () != null)
			other.gameObject.GetComponent<IStayInteractable> ().OnStayEffect (this);
	}

	// Called whenever the sphere exits a trigger
	[ServerCallback]
	void OnTriggerExit2D (Collider2D other)
	{
		if (other.gameObject.GetComponent<IExitInteractable> () != null)
			other.gameObject.GetComponent<IExitInteractable> ().OnExitEffect (this);
	}
		
	IEnumerator FadeAtStartLevel()
	{
		yield return new WaitForSeconds (waitTimeUntilFade);
		this.GetComponent<Blinker> ().FadeOut ();
	}

	/// <summary>
	/// Start the blink animation
	/// </summary>
	public void Blink()
	{
		if (isServer)
			RpcBlink ();
		this.GetComponent<Blinker> ().Blink ();
	}

	/// <summary>
	/// Start the damage animation
	/// </summary>
	public void Damage()
	{
		if (isServer)
			RpcDamage ();
		this.GetComponent<Blinker> ().DamageAnimation (damageDuration);
	}

	void SphereVisibility()
	{
		var color = this.GetComponent<Renderer> ().material.color;
		switch (GameManager.singleton.GetCurrentLevel ().sphereVisibility) {
		case Level.SphereVisibility.Visible:
			this.GetComponent<Renderer> ().material.color =
				new Vector4 (color.r, color.g, color.b, 1.0f);
			break;
		case Level.SphereVisibility.FadeAfterStartLevel:
			StartCoroutine(FadeAtStartLevel ());
			break;
		case Level.SphereVisibility.Invisible:
			this.GetComponent<Blinker>().FadeOut ();
			break;
		}
	}
		
	[ClientRpc]
	void RpcDamage()
	{
		this.GetComponent<Blinker> ().DamageAnimation (damageDuration);
	}

	[ClientRpc]
	void RpcBlink()
	{
		this.GetComponent<Blinker> ().Blink ();
	}
}
