using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[RequireComponent (typeof (Rigidbody2D))]
public class PlayerAccelerometerMove : NetworkBehaviour, IPlayerMotion
{
	private Vector2 gyROT = new Vector2 (0, 0);
	public Gyroscope gyro;
	public int sliderValue = 0;

	public bool IsActivated (int sliderValue)
	{
		return (this.sliderValue == sliderValue);
	}

	public void Move ()
	{
		if (!isLocalPlayer)
			return;
		if (SystemInfo.supportsGyroscope) {
			gyro = Input.gyro;
			gyro.enabled = true;
			this.GetComponent<Rigidbody2D>().AddForce (gyROT, ForceMode2D.Force);
		}
	}		
}
