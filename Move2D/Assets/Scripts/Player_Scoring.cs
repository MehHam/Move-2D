using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player_Scoring : NetworkBehaviour {

	//private float damage=1;
	//private float range=0;
	private GameObject goCDM;
	private int score=0;

	// Use this for initialization
	void Start () {
		
	
	}



	// Update is called once per frame
	void Update () {
	
		checkifshooting();

	}

	void checkifshooting()
	{
		if(!isLocalPlayer)
			return;

		shoot();

	}

	void shoot()
	{
		goCDM=GameObject.Find("SphereCDM");

		if(score!= goCDM.GetComponent<scoreCDM>().count){
		
		score = goCDM.GetComponent<scoreCDM>().count;
		CmdTellServerScore(score);
		
		}

	}


	[Command]
	void CmdTellServerScore(int damage){

		gameObject.GetComponent<Player_Health>().increaseHealth(damage);
		//Apply the dammage
	
	}


}
