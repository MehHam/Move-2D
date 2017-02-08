using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using ProgressBar;
using UnityEngine.UI;

public class motionPointFollow : NetworkBehaviour
{
	[SyncVar] private Vector3 syncposFollow;
	[SyncVar] private int syncBarValue;
	private Vector2 centrePos;
	public static GameObject objectFollow;
	public GameObject sphereCDM;

	//private int translation;
	private int randomRange;
	private int velocity;

	private Rigidbody2D rb;
	public static int motionMode;
	public static int modeCounter = 0;
	public static int modeTimer;
	private int numbOfTransitions;
	private float timerPerTransition;
	private GameObject progressBar;
	private Vector2 tempPos;

	// Use this for initialization
	void Start ()
	{

		centrePos = new Vector2 (0, 0);
		//translation = 1;
		randomRange = 7;
		motionMode = 1;
		numbOfTransitions = 5;

		//numbOfTransitions= (int)(modeTimer/numbOfTransitions);
	}

	void Update ()
	{

		if (levelDesign.levelValue == 2 || levelDesign.levelValue == 3 || levelDesign.levelValue == 22) {
			transmitPosition ();
		} else {
			return;
		}
		
	}

	//Update is called once per frame
	void FixedUpdate ()
	{
		if (!isLocalPlayer)
			return;

		if (levelDesign.levelValue == 2 || levelDesign.levelValue == 3 || levelDesign.levelValue == 22) {
			if (objectFollow == null || rb == null) {
				objectFollow = GameObject.Find ("pointFollow");
				if (objectFollow != null)
					rb = objectFollow.GetComponent<Rigidbody2D> ();
			}
		
			if (motionMode == 2) {
				
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


	public void randomMotion (Rigidbody2D rb, int option, float velocity, int randomRange)
	{

		switch (option) {
		case 2: 
			Vector2 randomPosition = new Vector2 ((int)Random.Range (-randomRange, randomRange), (int)Random.Range (-randomRange, randomRange));
			rb.position = 0.1f * velocity * randomPosition;
			break;

		case 1:
			int Amplitude = 8;
			rb.position = new Vector2 (0.0f, Amplitude * Mathf.Cos (6.24f * Time.fixedTime / velocity));
			//rb.position =new Vector2(0.0f,11);

			break;
		case 0:
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
		/*if(!isLocalPlayer || this.gameObject.GetComponent<PlayerID>().playerUniqueIdentity!=physics.playerArr[0].namePlayer)
		{
			Debug.Log(this.gameObject.GetComponent<PlayerID>().playerUniqueIdentity);
			return;
		}*/

		syncposFollow = position;
		syncBarValue = barValue;
		//syncRandomValue=barValue;

		//Apply the dammage
		
	}

}

