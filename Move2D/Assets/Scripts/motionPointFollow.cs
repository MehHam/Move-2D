using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using ProgressBar;
using UnityEngine.UI;

public class MotionPointFollow : NetworkBehaviour
{
	public enum MotionMode 
	{
		Random,
		Cos,
		Rotate
	}
	[SyncVar] private Vector3 syncposFollow;
	[SyncVar] private int syncBarValue;
	private Vector2 centrePos;
	public static GameObject objectFollow;
	public GameObject sphereCDM;

	//private int translation;
	private int randomRange;
	private int velocity;

	private Rigidbody2D rb;
	public static MotionMode motionMode = MotionMode.Cos;
	//public static int modeCounter = 0;
	//public static int modeTimer;
	private int numbOfTransitions;
	private float timerPerTransition;
	private GameObject progressBar;
	private Vector2 tempPos;

	// Use this for initialization
	void Start ()
	{
		sphereCDM = GameObject.FindGameObjectWithTag ("Sphere CDM");
		centrePos = new Vector2 (0, 0);
		//translation = 1;
		randomRange = 7;
		numbOfTransitions = 5;

		//numbOfTransitions= (int)(modeTimer/numbOfTransitions);
	}

	void Update ()
	{
		if (GameManager.singleton.GetCurrentLevel ().gameMode == GameManager.GameMode.MotionPointFollow)
			transmitPosition();
	}

	//Update is called once per frame
	[ServerCallback]
	void FixedUpdate ()
	{
		if (GameManager.singleton.GetCurrentLevel ().gameMode == GameManager.GameMode.MotionPointFollow) {
			if (objectFollow == null || rb == null) {
				objectFollow = GameObject.Find ("pointFollow");
				if (objectFollow != null)
					rb = objectFollow.GetComponent<Rigidbody2D> ();
			}
		
			if (motionMode == MotionMode.Random) {
				
				if (modeCounter < (int)modeTimer / numbOfTransitions) {
					motionCriterion (rb, sphereCDM);
					modeCounter++;
				} else {

					randomMotion (rb, motionMode, 10, randomRange);
					modeCounter = 0;
				}
			} else {
				motionCriterion (rb, sphereCDM);
				randomMotion (rb, motionMode, 10, randomRange);
			}
		}

	}


	public void randomMotion (Rigidbody2D rb, MotionMode mode, float velocity, int randomRange)
	{

		switch (mode) {
		// random position
		case MotionMode.Random: 
			Vector2 randomPosition = new Vector2 ((int)Random.Range (-randomRange, randomRange), (int)Random.Range (-randomRange, randomRange));
			rb.position = 0.1f * velocity * randomPosition;
			break;
		// cos
		case MotionMode.Cos:
			int Amplitude = 8;
			rb.position = new Vector2 (0.0f, Amplitude * Mathf.Cos (6.24f * Time.fixedTime / velocity));
			//rb.position =new Vector2(0.0f,11);

			break;
		// rotate
		case MotionMode.Rotate:
			velocity = 10f;
			rb.transform.RotateAround (centrePos, Vector3.forward * 10f, velocity * Time.deltaTime);
			break;
		}

	}

	public void motionCriterion (Rigidbody2D rb, GameObject go)
	{
		progressBar = GameObject.Find ("ProgressRadialHollow");
		int criterion = (int)xiSquareCriterion (go, objectFollow, progressBar);
		progressBar.GetComponent<ProgressRadialBehaviour> ().Value = criterion;
		CmdTellServerFollowPos (rb.transform.position, criterion);
	}


	public float xiSquareCriterion (GameObject go1, GameObject go2, GameObject go)
	{
		float criterion;
		int radiusCriterion = 20;
		criterion = Vector3.SqrMagnitude (go1.transform.position - go2.transform.position) / Vector3.SqrMagnitude (new Vector3 (0, 0, radiusCriterion));

		if (criterion <= 0.01f) {
			criterion = 0.01f;

		}

		return 100 * (1.0f - criterion);
		
	}

	void transmitPosition ()
	{
		if (isServer) {
			if (objectFollow == null)
			objectFollow = GameObject.Find ("pointFollow");
			if (progressBar == null)
				progressBar = GameObject.Find ("ProgressRadialHollow");
			if (objectFollow != null)
				objectFollow.transform.position = syncposFollow;
			if (progressBar != null)
				progressBar.GetComponent<ProgressRadialBehaviour> ().Value = syncBarValue;
		} else {

			if (!isLocalPlayer && this.gameObject.GetComponent<PlayerID> ().playerUniqueIdentity != physics.players [0].namePlayer) {

				objectFollow = GameObject.Find ("pointFollow");
				progressBar = GameObject.Find ("ProgressRadialHollow");
				objectFollow.transform.position = syncposFollow;
				progressBar.GetComponent<ProgressRadialBehaviour> ().Value = syncBarValue;
			}

		}
	}

	[Command]
	void CmdTellServerFollowPos (Vector3 position, int barValue)
	{
		syncposFollow = position;
		syncBarValue = barValue;
	}

}

