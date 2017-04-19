using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using MovementEffects;

namespace Move2D
{
	/// <summary>  
	///  General class for the sphere
	/// </summary>  
	public class SphereCDM : NetworkBehaviour, ILineObject
	{
		/// <summary>
		/// How many seconds the sphere waits until it fades
		/// </summary>
		[Tooltip ("How many seconds the sphere waits until it fades")]
		public float waitTimeUntilFade = 5.0f;

		/// <summary>
		/// The duration of the damage animation
		/// </summary>
		[Tooltip ("The duration of the damage animation")]
		public float damageDuration = 2.0f;
		/// <summary>
		/// When the sphere is invincible, it doesn't take any damage
		/// </summary>
		public bool isInvincible = false;
		/// <summary>
		/// Time until which the sphere reappears if the sphere is in invisible mode
		/// </summary>
		[Tooltip ("Time until which the sphere reappears if the sphere is in invisible mode")]
		public float sphereApparitionTime = 5.0f;
		/// <summary>
		/// The duration of the sphere apparition.
		/// </summary>
		[Tooltip ("The duration of the sphere apparition.")]
		public float sphereApparitionDuration = 1.0f;

		public ScorePopupUI scorePopup;

		float _timeSinceLastApparition = 0.0f;

		// ----------------- Events ------------------

		void OnEnable ()
		{
			GameManager.onLevelStarted += OnLevelStarted;
			GameManager.onRespawn += OnLevelStarted;
			GameManager.onScoreChange += OnScoreChange;
		}


		void OnDisable ()
		{
			GameManager.onLevelStarted -= OnLevelStarted;
			GameManager.onRespawn -= OnLevelStarted;
			GameManager.onScoreChange -= OnScoreChange;
		}

		void OnScoreChange (int value)
		{
			RpcScoreChange (value);
		}

		void OnLevelStarted ()
		{
			SphereVisibility ();
		}

		// ----------------- Collider Events ------------------


		// Called whenever the sphere enters a trigger
		[ServerCallback]
		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.gameObject.GetComponent<IEnterInteractable> () != null)
				other.gameObject.GetComponent<IEnterInteractable> ().OnEnterEffect (this);
		}

		// Called each frame the sphere is inside a trigger
		[ServerCallback]
		void OnTriggerStay2D (Collider2D other)
		{
			if (other.gameObject.GetComponent<IStayInteractable> () != null)
				other.gameObject.GetComponent<IStayInteractable> ().OnStayEffect (this);
		}

		// Called whenever the sphere exits a trigger
		[ServerCallback]
		void OnTriggerExit2D (Collider2D other)
		{
			if (other.gameObject.GetComponent<IExitInteractable> () != null)
				other.gameObject.GetComponent<IExitInteractable> ().OnExitEffect (this);
		}

		IEnumerator FadeAtStartLevel ()
		{
			yield return new WaitForSeconds (waitTimeUntilFade);
			this.GetComponent<Blinker> ().FadeOut ();
		}

		/// <summary>
		/// Start the blink animation
		/// </summary>
		public void Blink (float blinkTime = 0.0f)
		{
			if (isServer)
				RpcBlink (blinkTime);
			this.GetComponent<Blinker> ().Blink (blinkTime);
			_timeSinceLastApparition = Time.time;
		}

		public void LoseLife(int amount)
		{
			if (!isInvincible) {
				GameManager.singleton.LoseLife (amount);
			}
		}

		public void DestroySphere()
		{
			if (!isInvincible) {
				isInvincible = true;
				GetComponent<SpherePhysics> ().enabled = false;
				Blink ();
				StartCoroutine (WaitForRespawn (1.0f));
				Destroy (this.GetComponent<PlayerLineManager> ());
				RpcDestroySphere ();
			}
		}

		/// <summary>
		/// Start the damage animation
		/// </summary>
		public void Damage ()
		{
			if (isServer)
				RpcDamage ();
			// this.GetComponent<Blinker> ().DamageAnimation (damageDuration);
		}

		void SphereVisibility ()
		{
			var color = this.GetComponent<Renderer> ().material.color;
			switch (GameManager.singleton.GetCurrentLevel ().sphereVisibility) {
			case Level.SphereVisibility.Visible:
				this.GetComponent<Renderer> ().material.color =
				new Vector4 (color.r, color.g, color.b, 1.0f);
				break;
			case Level.SphereVisibility.FadeAfterStartLevel:
				StartCoroutine (FadeAtStartLevel ());
				break;
			case Level.SphereVisibility.Invisible:
				this.GetComponent<SpriteRenderer> ().color = new Color (color.r, color.g, color.b, 0.0f);
				break;
			}
		}

		[Server]
		IEnumerator WaitForRespawn(float respawnTime)
		{
			yield return new WaitForSeconds (respawnTime);
			GameManager.singleton.StartRespawn ();
		}

		[ClientRpc]
		void RpcDamage ()
		{
			// this.GetComponent<Blinker> ().DamageAnimation (damageDuration);
		}

		[ClientRpc]
		void RpcBlink (float time)
		{
			this.GetComponent<Blinker> ().Blink (time);
		}

		[ClientRpc]
		void RpcScoreChange (int value)
		{
			var go = GameManager.Instantiate (
				        scorePopup,
				        this.transform.position,
				        Quaternion.identity
			        );
			go.text.text = value.ToString ();
			go.text.color = value >= 0 ? Color.white : Color.red;
		}

		[ClientRpc]
		void RpcDestroySphere()
		{
			GetComponent<SpherePhysics> ().enabled = false;
			this.GetComponent<Blinker> ().FadeOut (0.5f);
			Destroy (this.GetComponent<PlayerLineManager> ());
		}

		[ServerCallback]
		void Update()
		{
			if (Time.time - _timeSinceLastApparition > sphereApparitionTime) {
				Blink ();
			}
		}

		#region IPlayerLineObject implementation

		public Color GetColor ()
		{
			return this.GetComponent<Renderer> ().material.GetColor ("_Color");
		}

		public float GetMass ()
		{
			return 1.0f;
		}

		#endregion
	}
}
