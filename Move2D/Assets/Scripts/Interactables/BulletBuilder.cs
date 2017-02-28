using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MovementEffects;

public class BulletBuilder: NetworkBehaviour
{
	/// <summary>
	/// The bullet prefab.
	/// </summary>
	[Tooltip("The bullet prefab")]
	public Bullet bulletPrefab;

	/// <summary>
	/// The radius of the spawn circle
	/// </summary>
	[Tooltip("The radius of the spawn circle")]
	public float spawnRadius = 7.0f;

	/// <summary>
	/// The center of the spawn circle
	/// </summary>
	public Vector3 spawnCenter = new Vector3(0.0f, 0.0f, 0.0f);

	/// <summary>
	/// Cooldown between each spawn of bullet
	/// </summary>
	public float cooldownTime = 1.0f;

	private GameObject _sphereCDM;

	void OnEnable()
	{
		GameManager.onLevelStarted += OnLevelStarted;
	}

	void OnDisable()
	{
		GameManager.onLevelStarted -= OnLevelStarted;
	}

	IEnumerator RandomPattern()
	{
		while (true) {
			SpawnRandom ();
			yield return new WaitForSeconds (cooldownTime);
		}
	}

	void OnLevelStarted()
	{
		StartCoroutine (RandomPattern ());
	}

	void Start()
	{
		_sphereCDM = GameObject.FindGameObjectWithTag ("SphereCDM");
	}

	void SpawnRandom()
	{
		float sphereAngle = Vector3.Angle (Vector3.down, GetDirectionVector(_sphereCDM.transform.position, spawnCenter)) * Mathf.Deg2Rad;
		float angle = Random.Range (sphereAngle + Mathf.PI * 0.5f, sphereAngle + Mathf.PI * 1.5f);
		float x = Mathf.Cos (angle) * spawnRadius;
		float y = Mathf.Sin (angle) * spawnRadius;
		Spawn (new Vector3 (x + spawnCenter.x, y + spawnCenter.y, 0.0f + spawnCenter.z));
	}

	/// <summary>
	/// Gets the direction vector.
	/// </summary>
	/// <returns>The direction vector.</returns>
	Vector3 GetDirectionVector(Vector3 target, Vector3 start)
	{
		var heading = target - start;
		var distance = heading.magnitude;
		return heading / distance;
	}

	void Spawn(Vector3 position)
	{
		var bullet = Instantiate (bulletPrefab, position, Quaternion.identity);
		bullet.GetComponent<Bullet> ().direction = GetDirectionVector (_sphereCDM.transform.position, bullet.transform.position);
		NetworkServer.Spawn (bullet.gameObject);
	}
}
