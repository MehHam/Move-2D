using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using ProgressBar;
using UnityEngine.UI;

public class motionKiller : NetworkBehaviour {
	[SyncVar] private Vector3 syncposFollow;
	//private Vector2 centrePos; 
	private GameObject objectFollow;
	//private string nomme="";

	//private int translation;
	//private int randomRange;
	private int velocity;

	private Rigidbody2D rb;
	public static int motionMode;
	public static int modeCounter=0;
	public static int modeTimer;
	//private int numbOfTransitions;
	private float timerPerTransition;
	private GameObject progressBar;
	private Vector2 tempPos;

	// Use this for initialization
	void Start () {

		//centrePos=new Vector2(0,0);
		//translation = 1;
		//randomRange= 7;
		motionMode=1;
		//numbOfTransitions=10;


	//numbOfTransitions= (int)(modeTimer/numbOfTransitions);
	}

	void Update()
	{
		//if(levelDesign.levelValue==3)
		//{
		transmitPosition("Killer1");
		transmitPosition("Killer2");
		transmitPosition("Killer3");
		transmitPosition("Killer4");

		//}
		
	}

	// Update is called once per frame
	void FixedUpdate () {
		if(!isLocalPlayer)
			return;
		if(levelDesign.levelValue==3){	
			KillerMotion(GameObject.Find("Killer1").GetComponent<Rigidbody2D>(),2,10.0f,-3.5f,1.0f,0.0f);
			KillerMotion(GameObject.Find("Killer2").GetComponent<Rigidbody2D>(),2,10.0f,-8.5f,-6.0f,0.0f);
			KillerMotion(GameObject.Find("Killer3").GetComponent<Rigidbody2D>(),1,10.0f,-1.75f,1.5f,7.0f);
			KillerMotion(GameObject.Find("Killer4").GetComponent<Rigidbody2D>(),2,10.0f,0.0f,2.3f,6.50f);
		}	

		

	}


	public void KillerMotion(Rigidbody2D rb, int option,float velocity, float randomRangeNeg, float randomRangePos, float positionFixe){

		switch(option)
		{
			case 2: // oscillator vertical
			rb.position= new Vector2(positionFixe,-1.0f+ randomRangeNeg + (randomRangePos-randomRangeNeg)*Mathf.Pow(Mathf.Cos(6.24f*Time.fixedTime/velocity),2));
			break;

			case 1: //oscillator horizontal
			rb.position= new Vector2(randomRangeNeg + (randomRangePos-randomRangeNeg)*Mathf.Pow(Mathf.Cos(6.24f*Time.fixedTime/velocity),2), positionFixe);
			break;

		}

	}

	void transmitPosition(string nameFollow){

		if(!isLocalPlayer || this.gameObject.GetComponent<PlayerID>().playerUniqueIdentity!=physics.playerArr[0].namePlayer){

			objectFollow=GameObject.Find(nameFollow);
			objectFollow.transform.position= syncposFollow;

		}
	}

	[Command]
	void CmdTellServerFollowPos(Vector3 position, int barValue){

		syncposFollow=position;

		
	}

}

