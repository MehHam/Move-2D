using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerScoring : NetworkBehaviour {

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

		if(score!= goCDM.GetComponent<GameScore>().count){
		
		score = goCDM.GetComponent<GameScore>().count;
		CmdTellServerScore(score);
		
		}

	}


	[Command]
	void CmdTellServerScore(int damage){

		gameObject.GetComponent<PlayerScore>().increaseHealth(damage);
		//Apply the dammage
	
	}


}
