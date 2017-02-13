using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[RequireComponent (typeof (Rigidbody2D))]
public class PlayerAccelerometerMove : NetworkBehaviour
{
	private Vector2 gyROT = new Vector2 (0, 0);
	public Gyroscope gyro;

	public void Move ()
	{
		if (SystemInfo.supportsGyroscope) {
			gyro = Input.gyro;
			gyro.enabled = true;
			this.GetComponent<Rigidbody2D>().AddForce (gyROT, ForceMode2D.Force);
		}
	}

	void FixedUpdate ()
	{
		if (!isLocalPlayer)
			return;
		Move ();
	}				
}
