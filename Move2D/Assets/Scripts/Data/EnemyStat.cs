using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	[CreateAssetMenu (fileName = "EnemyStat", menuName = "Data/Enemy", order = 22)]
	public class EnemyStat : ScriptableObject
	{
		public enum MovementType
		{
			Lerp,
			Linear,
			Teleport,
			Follow
		}

		/// <summary>
		/// The enemy name
		/// </summary>
		[Tooltip ("The enemy name")]
		public string enemyName;
		/// <summary>
		/// How the enemy moves.
		/// Lerp = The enemy does a lerp movement
		/// Linear = The enemy moves linearly from one point to another
		/// Teleport = The enemy teleports from one point to another
		/// Follow = Follow the sphere CDM
		/// </summary>
		[Tooltip ("How the enemy moves")]
		public MovementType movementType;
		/// <summary>
		/// The enemy speed. If the movement type is teleport, the speed is the cooldown time between each teleport
		/// </summary>
		[Tooltip ("The enemy speed")]
		public float speed;
		/// <summary>
		/// The number of life points loses when the sphere touches the enemy
		/// </summary>
		[Tooltip ("The number of life points the player loses when the sphere touches the enemy")]
		public int damageValue = 1;
		/// <summary>
		/// Is the enemy destroyed when the sphere touches it ?
		/// </summary>
		public bool isDestroyedOnHit;
		/// <summary>
		/// The duration of the blink animation
		/// </summary>
		public float blinkDuration = 3.0f;
	}
}