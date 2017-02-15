using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
	void OnEnterEffect(SphereCDM sphere);
	void OnStayEffect(SphereCDM sphere);
	void OnExitEffect(SphereCDM sphere);
}

