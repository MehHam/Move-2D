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

		public ScorePopupUI scorePopup;

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
		public void Blink ()
		{
			if (isServer)
				RpcBlink ();
			this.GetComponent<Blinker> ().Blink ();
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
				this.GetComponent<Blinker> ().FadeOut (0.5f);
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
				this.GetComponent<Blinker> ().FadeOut ();
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
		void RpcBlink ()
		{
			this.GetComponent<Blinker> ().Blink ();
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

		#region IPlayerLineObject implementation

		public Color GetColor ()
		{
			return Color.white;
		}

		public float GetMass ()
		{
			return 1.0f;
		}

		#endregion
	}
}
