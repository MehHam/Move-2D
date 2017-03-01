using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// An Interactable Enemy. He goes from one point to another and ignore the sphere position completely
/// </summary>
public class Enemy : NetworkBehaviour, IInteractable {
	public enum MovementType {
		Lerp,
		Linear,
		Teleport
	}
	/// <summary>
	/// The enemy speed. If the movement type is teleport, the speed is the cooldown time between each teleport
	/// </summary>
	[Tooltip("The enemy speed")]
	public float speed = 1.0f;
	/// <summary>
	/// The enemy path. Those are all children gameObjects with the tag "path".
	/// He goes from one point to another in the order defined in the inspector.
	/// </summary>
	[Tooltip("The enemy path. The enemy goes from one point to another in the same order as the list")]
	public List<Transform> path = new List<Transform>();
	/// <summary>
	/// How the enemy moves.
	/// Lerp = The enemy does a lerp movement
	/// Linear = The enemy moves linearly from one point to another
	/// Teleport = The enemy teleports from one point to another
	/// </summary>
	public MovementType movementType;
	private int _currentDestinationIndex = 0;
	private float _startTime;
	private float _journeyLength;

	void Start()
	{
		if (path.Count != 0) {
			_currentDestinationIndex = 0;
			_startTime = Time.time;
			_journeyLength = Vector3.Distance (this.transform.position, path [_currentDestinationIndex].position);
		}
	}

	void FindPath()
	{
		path.Clear();
		Transform parent = transform;
		GetChildObject(parent);
	}

	void GetChildObject(Transform parent)
	{
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			if (child.tag == "Path")
			{
				path.Add(child);
			}
			if (child.childCount > 0)
			{
				GetChildObject(child);
			}
		}
	}

	Vector3 Destination()
	{
		return path [_currentDestinationIndex].position;
	}

	void MoveLinear()
	{
		this.transform.position = Vector3.MoveTowards (this.transform.position, Destination(), speed * Time.deltaTime);
	}

	void MoveLerp()
	{
		float distCovered = (Time.time - _startTime) * speed;
		float fracJourney = distCovered / _journeyLength;
		this.transform.position = Vector3.Lerp (this.transform.position, Destination(), fracJourney);

	}

	void MoveTeleport()
	{
		if (_startTime + speed <= Time.time) {
			this.transform.position = Destination ();
		}
	}

	void Move()
	{
		switch (movementType) {
			case MovementType.Lerp:
				MoveLerp ();
				break;
			case MovementType.Linear:
				MoveLinear ();
				break;
			case MovementType.Teleport:
				MoveTeleport ();
				break;
		}
		if (this.transform.position == Destination()) {
			_currentDestinationIndex = (_currentDestinationIndex + 1) % path.Count;
			_startTime = Time.time;
			_journeyLength = Vector3.Distance (this.transform.position, path [_currentDestinationIndex].position);
		}
	}

	[Server]
	void Update()
	{
		if (path.Count != 0 && GameManager.singleton != null && !GameManager.singleton.paused) {
			Move ();
		}
	}

	public void OnEnterEffect (SphereCDM sphere)
	{
		throw new System.NotImplementedException ();
	}

	public void OnStayEffect (SphereCDM sphere)
	{
		throw new System.NotImplementedException ();
	}

	public void OnExitEffect (SphereCDM sphere)
	{
		throw new System.NotImplementedException ();
	}
}
