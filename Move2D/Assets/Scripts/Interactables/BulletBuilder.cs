using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MovementEffects;

namespace Move2D
{
	/// <summary>
	/// A Bullet builder. Is used to fire bullets.
	/// </summary>
	public class BulletBuilder: NetworkBehaviour
	{
		/// <summary>
		/// The bullet prefab.
		/// </summary>
		[Tooltip ("The bullet prefab")]
		public Bullet bulletPrefab;

		/// <summary>
		/// The radius of the spawn circle
		/// </summary>
		[Tooltip ("The radius of the spawn circle")]
		public float spawnRadius = 7.0f;

		public float angleIncrement = 1.0f;

		/// <summary>
		/// The center of the spawn circle
		/// </summary>
		public Vector3 spawnCenter = new Vector3 (0.0f, 0.0f, 0.0f);

		/// <summary>
		/// Cooldown between each spawn of bullet
		/// </summary>
		public float cooldownTime = 1.0f;

		public Pattern _pattern = Pattern.Random;

		/// <summary>
		/// The bullet pattern
		/// </summary>
		/// <value>The pattern.</value>
		public Pattern pattern {
			get {
				return _pattern;
			}
			set {
				this._pattern = value;
				ChangePattern (this._pattern);
			}
		}

		public enum Pattern
		{
			Random,
			Orbit,
			Circle,
			TargetSphere
		}

		private GameObject _sphereCDM;
		private bool _levelStarted = false;

		void OnEnable ()
		{
			GameManager.onLevelStarted += OnLevelStarted;
		}

		void OnDisable ()
		{
			GameManager.onLevelStarted -= OnLevelStarted;
		}

		IEnumerator RandomPattern ()
		{
			while (true) {
				SpawnRandom ();
				yield return new WaitForSeconds (cooldownTime);
			}
		}

		IEnumerator CirclePattern ()
		{
			var angle = Random.Range (0.0f, 360.0f);
			while (true) {
				SpawnCircle (angle * Mathf.Deg2Rad);
				yield return new WaitForSeconds (cooldownTime);
				angle = (angle + angleIncrement) % 360.0f;
			}
		}

		IEnumerator TargetSpherePattern ()
		{
			while (true) {
				Spawn (this.transform.position,
					Quaternion.identity,
					GetDirectionVector (
						this._sphereCDM.transform.position,
						this.transform.position
					)
				);
				yield return new WaitForSeconds (cooldownTime);
			}
		}

		IEnumerator OrbitPattern ()
		{
			var angle = Random.Range (0.0f, 360.0f);
			while (true) {
				SpawnOrbit (angle * Mathf.Rad2Deg);
				yield return new WaitForSeconds (cooldownTime);
				angle = (angle + angleIncrement) % 360.0f;
			}
		}

		void OnLevelStarted ()
		{
			this._sphereCDM = GameObject.FindGameObjectWithTag ("SphereCDM");
			this._levelStarted = true;
			StartPattern (this._pattern);
		}

		void SpawnOrbit (float angle)
		{
			float x = Mathf.Cos (angle) * this.spawnRadius;
			float y = Mathf.Sin (angle) * this.spawnRadius;
			Vector3 pos = new Vector3 (x + this.spawnCenter.x, y + this.spawnCenter.y, 0.0f + this.spawnCenter.z);
			Spawn (pos,
				Quaternion.identity,
				GetDirectionVector (this._sphereCDM.transform.position, pos));
		}

		void SpawnRandom ()
		{
			float sphereAngle = Vector3.Angle (Vector3.down, GetDirectionVector (this._sphereCDM.transform.position, this.spawnCenter)) * Mathf.Deg2Rad;
			float angle = Random.Range (sphereAngle + Mathf.PI * 0.5f, sphereAngle + Mathf.PI * 1.5f);
			float x = Mathf.Cos (angle) * this.spawnRadius;
			float y = Mathf.Sin (angle) * this.spawnRadius;
			Vector3 pos = new Vector3 (x + this.spawnCenter.x, y + this.spawnCenter.y, 0.0f + this.spawnCenter.z);
			Spawn (new Vector3 (x + this.spawnCenter.x, y + this.spawnCenter.y, 0.0f + this.spawnCenter.z),
				Quaternion.identity,
				GetDirectionVector (this._sphereCDM.transform.position, pos));
		}

		void SpawnCircle (float angle)
		{
			float x = Mathf.Cos (angle) * this.spawnRadius;
			float y = Mathf.Sin (angle) * this.spawnRadius;
			Vector3 pos = new Vector3 (x + this.spawnCenter.x, y + this.spawnCenter.y, 0.0f + this.spawnCenter.z);
			Spawn (this.transform.position, Quaternion.identity, GetDirectionVector(pos, this.spawnCenter));
		}

		/// <summary>
		/// Gets the direction vector.
		/// </summary>
		/// <returns>The direction vector.</returns>
		Vector3 GetDirectionVector (Vector3 target, Vector3 start)
		{
			var heading = target - start;
			var distance = heading.magnitude;
			return heading / distance;
		}

		void Spawn (Vector3 position, Quaternion rotation, Vector3 director)
		{
			var bullet = Instantiate (this.bulletPrefab, position, rotation);
			bullet.GetComponent<Bullet> ().direction = director;
			NetworkServer.Spawn (bullet.gameObject);
		}

		void StartPattern(Pattern pattern)
		{
			if (this._levelStarted) {
				switch (pattern) {
				case Pattern.Random:
					StartCoroutine (RandomPattern ());
					break;
				case Pattern.Circle:
					StartCoroutine (CirclePattern ());
					break;
				case Pattern.Orbit:
					StartCoroutine (OrbitPattern ());
					break;
				case Pattern.TargetSphere:
					StartCoroutine (TargetSpherePattern ());
					break;
				}
			}
		}

		void ChangePattern(Pattern pattern)
		{
			StopAllCoroutines();
			StartPattern (pattern);
		}

		void OnDestroy ()
		{
			StopAllCoroutines ();
		}
	}
}