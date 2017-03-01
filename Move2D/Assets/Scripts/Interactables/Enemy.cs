using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// An Interactable Enemy. He goes from one point to another and ignore the sphere position completely
/// </summary>
[RequireComponent(typeof(Blinker))]
public class Enemy : NetworkBehaviour, IInteractable {
	public EnemyStat stats;


	/// <summary>
	/// The enemy speed. If the movement type is teleport, the speed is the cooldown time between each teleport
	/// </summary>
	public float speed {
		get { return stats.speed; }
	}
	/// <summary>
	/// How the enemy moves.
	/// Lerp = The enemy does a lerp movement
	/// Linear = The enemy moves linearly from one point to another
	/// Teleport = The enemy teleports from one point to another
	/// </summary>
	public EnemyStat.MovementType movementType {
		get { return stats.movementType; }
	}
	/// <summary>
	/// The number of points the player loses when 
	/// </summary>
	public int scoreValue {
		get { return stats.scoreValue; }
	}
	/// <summary>
	/// Is the enemy destroyed when the sphere touches it ?
	/// </summary>
	public bool isDestroyedOnHit {
		get { return stats.isDestroyedOnHit; }
	}
	/// <summary>
	/// Gets the duration of the blink animation.
	/// </summary>
	/// <value>The duration of the blink animation.</value>
	public float blinkDuration {
		get { return stats.blinkDuration; }
	}
	/// <summary>
	/// The enemy path. Those are all children gameObjects with the tag "path".
	/// He goes from one point to another in the order defined in the inspector.
	/// </summary>
	[Tooltip("The enemy path. The enemy goes from one point to another in the same order as the list")]
	public List<Transform> path = new List<Transform>();

	private const float _blinkTime = 0.25f;
	private bool _damaged = false;
	private int _currentDestinationIndex = 0;
	private float _startTime;
	private float _journeyLength;
	private Transform _sphereCDM;

	void Start()
	{
		if (path.Count != 0) {
			_currentDestinationIndex = 0;
			_startTime = Time.time;
			_journeyLength = Vector3.Distance (this.transform.position, path [_currentDestinationIndex].position);
			_sphereCDM = GameObject.FindGameObjectWithTag ("SphereCDM").transform;
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

	void MoveFollow()
	{
		this.transform.position = Vector3.MoveTowards (this.transform.position, _sphereCDM.position, speed * Time.deltaTime);
	}

	void Move()
	{
		switch (movementType) {
			case EnemyStat.MovementType.Lerp:
				MoveLerp ();
				break;
			case EnemyStat.MovementType.Linear:
				MoveLinear ();
				break;
			case EnemyStat.MovementType.Teleport:
				MoveTeleport ();
				break;
			case EnemyStat.MovementType.Follow:
				MoveFollow ();
				break;
		}
		if (this.transform.position == Destination()) {
			_currentDestinationIndex = (_currentDestinationIndex + 1) % path.Count;
			_startTime = Time.time;
			_journeyLength = Vector3.Distance (this.transform.position, path [_currentDestinationIndex].position);
		}
	}
		
	void Update()
	{
		if (path.Count != 0 && GameManager.singleton != null && !GameManager.singleton.paused) {
			Move ();
		}
	}

	[ClientRpc]
	void RpcBlink()
	{
		StartCoroutine (BlinkRoutine (this.blinkDuration));
	}

	void Blink()
	{
		if (isServer)
			RpcBlink ();
		StartCoroutine (BlinkRoutine (this.blinkDuration));
	}

	IEnumerator BlinkRoutine(float blinkDuration)
	{
		this._damaged = true;
		this.GetComponent<Blinker> ().DamageAnimation ();
		yield return new WaitForSeconds (blinkDuration);
		this.GetComponent<Blinker> ().StopDamageAnimation ();
		this._damaged = false;
	}

	public void OnEnterEffect (SphereCDM sphere)
	{
		if (_damaged)
			return;
		GameManager.singleton.DecreaseScore ();
		sphere.Damage ();
		if (this.isDestroyedOnHit)
			NetworkServer.Destroy (this.gameObject);
		else {
			Blink ();
		}
	}

	public void OnStayEffect (SphereCDM sphere)
	{
	}

	public void OnExitEffect (SphereCDM sphere)
	{
	}
}
