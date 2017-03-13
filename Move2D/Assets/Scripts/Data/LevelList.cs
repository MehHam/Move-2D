using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Move2D
{
	/// <summary>
	/// Difficulty list for levels
	/// </summary>
	[CreateAssetMenu (fileName = "LevelList", menuName = "Data/List/Level", order = 22)]
	public class LevelList: ScriptableObject
	{
		/// <summary>
		/// Array displaying the data of all the beginner levels that will be played
		/// </summary>
		[Tooltip ("Array displaying the data of all the beginner levels that will be played")]
		public Level[] beginnerLevels;
		/// <summary>
		/// Array displaying the data of all the intermediate levels that will be played
		/// </summary>
		[Tooltip ("Array displaying the data of all the intermediate levels that will be played")]
		public Level[] intermediateLevels;
		/// <summary>
		/// Array displaying the data of all the expert levels that will be played
		/// </summary>
		[Tooltip ("Array displaying the data of all the expert levels that will be played")]
		public Level[] expertLevels;
	}
}