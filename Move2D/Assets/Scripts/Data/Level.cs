using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The level data
/// </summary>
[CreateAssetMenu(fileName = "Level", menuName = "Data/Level", order = 22)]
public class Level : ScriptableObject
{
	/// <summary>
	/// All the type of gameMode:
	/// Default: Move the sphere around to get the pickups
	/// MotionPointFollow: Move the sphere to get it the closest to the motion point
	/// </summary>
	public enum GameMode
	{
		Default,
		MotionPointFollow,
	}
	/// <summary>
	/// The visibility of the sphere:
	/// Visible: The sphere starts and stays visible
	/// FadeAfterStartLevel: The sphere disappears after a certain time
	/// Invisible: The sphere starts completely invisible
	/// </summary>
	public enum SphereVisibility
	{
		Visible,
		FadeAfterStartLevel,
		Invisible,
	}
	/// <summary>
	/// Level index
	/// </summary>
	[Tooltip("Level index")]
	public uint index;
	/// <summary>
	/// The level duration
	/// </summary>
	[Tooltip("Level duration")]
	public uint time;
	/// <summary>
	/// The sceneName in the build, this should never be empty
	/// </summary>
	[Tooltip("The scene name in the build, this should never be empty")]
	public string sceneName;
	/// <summary>
	/// The gameMode of this level
	/// </summary>
	[Tooltip("The game mode of this level")]
	public GameMode gameMode;
	/// <summary>
	/// If the gameMode is MotionPointFollow, what kind of motion the point has
	/// </summary>
	[Tooltip("If the game mode if MotionPointFollow, what kind of motion the point has")]
	public MotionPointFollow.MotionMode motionMode;
	/// <summary>
	/// Whether the ready animation is displayed at the beginning of this level or not.
	/// </summary>
	[Tooltip("Whether the ready animation is displayed at the beginning of this level")]
	public bool readyAnimation = false;
	/// <summary>
	/// The behaviour of the sphere during the level
	/// </summary>
	[Tooltip("The behaviour of the sphere visibility during the level")]
	public SphereVisibility sphereVisibility = SphereVisibility.Visible;
	/// <summary>
	/// Mass modification enabled
	/// </summary>
	[Tooltip("Is mass modification enabled ?")]
	public bool massModification = true;
}