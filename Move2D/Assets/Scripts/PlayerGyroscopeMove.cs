using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//Adding this allows us to access members of the UI namespace including Text.
using UnityEngine.UI;

[RequireComponent (typeof (Rigidbody2D))]
class PlayerGyroscopeMove : NetworkBehaviour
{
	public Gyroscope gyro;

	public void Move ()
	{
		if (SystemInfo.supportsGyroscope)
		{
			float radiusR = 15.7f;
			float alpha = 0f;
			float theta = Mathf.Atan (gyro.gravity.x / gyro.gravity.y);
			gyro = Input.gyro;
			gyro.enabled = true;

			if (theta < Mathf.PI / 2 && theta > -Mathf.PI / 2)
			{
				alpha = 2 * theta;
			}
			Vector2 playerPos = new Vector2 (radiusR * Mathf.Cos (alpha), radiusR * Mathf.Sin (alpha));
			if (playerPos.magnitude > radiusR * 0.95f && playerPos.magnitude < radiusR * 1.05f)
			{
				this.GetComponent<Rigidbody2D> ().transform.position = Vector2.Lerp (this.GetComponent<Rigidbody2D> ().position, playerPos, 0.5f);
			}
		}
	}

	void FixedUpdate ()
	{
		if (!isLocalPlayer)
			return;
		Move ();
	}
}

