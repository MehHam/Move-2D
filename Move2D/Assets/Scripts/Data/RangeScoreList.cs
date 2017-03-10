using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A List of RangeScore
/// </summary>
[CreateAssetMenu(fileName = "RangeScoreList", menuName = "Data/RangeScoreList", order = 22)]
public class RangeScoreList: ScriptableObject 
{
	/// <summary>
	/// A list of score given for each range of distance
	/// </summary>
	public RangeScore[] rangeScores;
}

