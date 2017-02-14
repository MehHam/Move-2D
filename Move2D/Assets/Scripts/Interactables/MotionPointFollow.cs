using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using ProgressBar;
using UnityEngine.UI;

public class MotionPointFollow : NetworkBehaviour, IInteractable
{
	public enum MotionMode 
	{
		Random,
		Cos,
		Rotate
	}

	[SyncVar] private Vector3 _syncposFollow;
	[SyncVar] private int _syncBarValue;
	public GameObject sphereCDM;

	public int amplitude = 8;
	public float velocity = 10.0f;
	public float randomPositionRange = 7.0f;
	public int randomTransitions = 5;
	public Vector2 centrePos = new Vector2(0.0f, 0.0f);

	private GameObject _progressBar;
	private Vector2 _tempPos;

	#region IInteractable implementation

	[Server]
	public void Effect (SphereCDM sphere)
	{
		GameManager.singleton.IncreaseScore ();
	}
	#endregion

	[ServerCallback]
	void Start ()
	{
		if (GameManager.singleton.GetCurrentLevel ().gameMode != GameManager.GameMode.MotionPointFollow) {
			this.enabled = false;
			return;
		}
		sphereCDM = GameObject.FindGameObjectWithTag ("SphereCDM");
		StartMovePattern ();
	}

	[Server]
	void StartMovePattern()
	{
		switch (GameManager.singleton.GetCurrentLevel ().motionMode) {
			case MotionMode.Random:
				StartCoroutine (RandomPattern ());
				break;
			case MotionMode.Cos:
				StartCoroutine (CosPattern ());
				break;
			case MotionMode.Rotate:
				StartCoroutine (RotationPattern ());
				break;
		}
	}

	[Server]
	IEnumerator RandomPattern()
	{
		float timeInterval = GameManager.singleton.GetCurrentLevel ().time / randomTransitions;
		while (true)
		{
			var pos = new Vector2 (Random.Range (-randomPositionRange, randomPositionRange),
				          Random.Range (-randomPositionRange, randomPositionRange));
			Debug.Log (pos);
			this.transform.position = pos;
			yield return new WaitForSeconds (timeInterval);
		}
	}

	[Server]
	IEnumerator CosPattern()
	{
		while (true)
		{
			var pos = new Vector2 (0.0f, amplitude * Mathf.Cos (6.24f * Time.fixedTime / velocity));
			Debug.Log (pos);
			this.transform.position = pos;
			yield return new WaitForFixedUpdate ();
		}
	}

	[Server]
	IEnumerator RotationPattern()
	{
		while (true)
		{
			this.transform.RotateAround (centrePos, Vector3.forward * 10f, velocity * Time.fixedDeltaTime);
			Debug.Log (this.transform.position);
			yield return new WaitForFixedUpdate ();
		}
	}
}

