using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	/// <summary>
	/// Implement this whenever you want to create an item that have an effect when it collides with the sphere
	/// </summary>
	public interface IEnterInteractable
	{
		/// <summary>
		/// Called when the sphere enters the trigger
		/// </summary>
		void OnEnterEffect (SphereCDM sphere);
	}

	/// <summary>
	/// Implement this whenever you want to create an item that have an effect when it stays within the sphere collider
	/// </summary>
	public interface IStayInteractable
	{
		/// <summary>
		/// Called each frame when the sphere is staying inside the trigger
		/// </summary>
		void OnStayEffect (SphereCDM sphere);
	}

	/// <summary>
	/// Implement this whenever you want to create an item that have an effect when it stays within the sphere collider
	/// </summary>
	public interface IExitInteractable
	{
		/// <summary>
		/// Called when the sphere exits the trigger
		/// </summary>
		void OnExitEffect (SphereCDM sphere);
	}
}

