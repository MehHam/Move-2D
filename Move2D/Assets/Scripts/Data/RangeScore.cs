using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The score given if the sphere is at a certain range of the Motion Point
/// </summary>
[System.Serializable]
public class RangeScore
{
	/// <summary>
	/// The min range in purcentage, inclusive.
	/// </summary>
	public float minRange;
	/// <summary>
	/// The max range in purcentage, non-inclusive.
	/// </summary>
	public float maxRange;
	/// <summary>
	/// The score given if the range is hit
	/// </summary>
	public int score;

	/// <summary>
	/// Determines whether the purcentage is in range
	/// </summary>
	/// <returns><c>true</c> if the purcentage is in range otherwise, <c>false</c>.</returns>
	/// <param name="purcentage">Purcentage.</param>
	public bool IsInRange(float purcentage) {
		return (purcentage >= minRange && purcentage < maxRange); 
	}
	/// <summary>
	/// Gets the score according to the range
	/// </summary>
	/// <returns>The score.</returns>
	/// <param name="purcentage">Range.</param>
	public int GetScore(float purcentage) {
		if (IsInRange (purcentage))
			return score;
		return 0;
	}
}

