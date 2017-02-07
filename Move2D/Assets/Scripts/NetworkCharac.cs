using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkCharac : NetworkBehaviour{

	public static ArrayList masseArray;
	public static ArrayList coloursArray;
	public static ArrayList positionsArray;
	public static ArrayList gameObjectsArray;

	private GameObject goObj1;
	private GameObject goObj2;
	//private int numbOfPlayerTags;
	//private bool networkSwitch = false;

	// Use this for initialization

	void Start(){
	//	numbOfPlayerTags=0;
		Vector3 temp=GameObject.Find ("CenterGO").transform.position;
		coloursArray = new ArrayList{Color.blue, Color.yellow,Color.green,Color.red, Color.black,Color.magenta,Color.white,Color.gray}; 
		positionsArray = new ArrayList{}; 
		for(int i=0;i<3;i++){

			positionsArray.ToArray()[i]=new Vector3(temp.x+Mathf.Cos (i*Mathf.PI/4),temp.y,temp.z+Mathf.Sin(i*45));
		}
		//gameObjectsArray = new ArrayList{};

		if(isLocalPlayer)
		{
			//gameObject.transform.position=new Vector3(300,29,20);
		//	audioListener.enabled=true;

		}

	}


}
