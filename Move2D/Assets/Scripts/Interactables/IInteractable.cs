using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implement this whenever you want to create an item that can be interacted with the sphere
/// </summary>
public interface IInteractable
{
	/// <summary>
	/// Called when the sphere enters the trigger
	/// </summary>
	void OnEnterEffect(SphereCDM sphere);
	/// <summary>
	/// Called each frame when the sphere is staying inside the trigger
	/// </summary>
	void OnStayEffect(SphereCDM sphere);
	/// <summary>
	/// Called when the sphere exits the trigger
	/// </summary>
	void OnExitEffect(SphereCDM sphere);
}

