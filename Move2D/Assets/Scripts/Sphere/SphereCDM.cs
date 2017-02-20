using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>  
///  This class performs the check on collision for the Sphere
/// </summary>  
public class SphereCDM : NetworkBehaviour
{
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
}
