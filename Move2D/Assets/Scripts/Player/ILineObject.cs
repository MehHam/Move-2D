using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Move2D
{
	public interface ILineObject
	{
		Color GetColor();
		float GetMass();
	}
}
