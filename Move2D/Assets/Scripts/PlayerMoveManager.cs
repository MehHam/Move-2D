using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//Adding this allows us to access members of the UI namespace including Text.
using UnityEngine.UI;

[RequireComponent(typeof(PlayerKeyboardMove))]
[RequireComponent(typeof(PlayerDragMove))]
[RequireComponent(typeof(PlayerAccelerometerMove))]
[RequireComponent(typeof(PlayerGyroscopeMove))]
public class PlayerMoveManager : NetworkBehaviour
{
	//Store a reference to the UI Text component which will display the 'You win' message.
	private Slider _slider;

	// Use this for initialization
	void Start ()
	{
		_slider = GameObject.Find ("Slider").GetComponent<Slider> ();
	}

	//FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
	void FixedUpdate ()
	{
		if (!isLocalPlayer)
			return;
		this.GetComponent<PlayerKeyboardMove> ().enabled = (_slider.value == 0);
		this.GetComponent<PlayerDragMove> ().enabled = (_slider.value == 1);
		this.GetComponent<PlayerAccelerometerMove> ().enabled = (_slider.value == 2);
		this.GetComponent<PlayerGyroscopeMove> ().enabled = (_slider.value == 3);

	}
}
