using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

[NetworkSettings(channel=0, sendInterval=0.1f)]
public class Player_SyncRotation : NetworkBehaviour { //script to Sync motion parameter
	
	[SyncVar] private Quaternion syncPlayerRotation;
		
	[SerializeField] Transform myTransform;
	[SerializeField] float learpRate;
	[SerializeField] private bool useHistoricalInterpolation=false;


	//private Quaternion syncRot;
	private NetworkClient nclient;
	private int latency;
	private Text latencyText;
	private List<Vector3> syncPlayerRotList = new List<Vector3>();
	private float normalLerpRate = 20;
	private float FasterLearpRate=30;
	private float closeEnought=0.5f;

	private float threshold =1;
	private Quaternion lastPlayerRot;



	void Start(){
	


		learpRate=normalLerpRate;

	}

	void Update () {
		
		LearpRotations();

		
	}

	// Update is called once per frame
	void FixedUpdate () {

		TransmitRotations();

	
	
	}

	void LearpRotations()
	{
		if(!isLocalPlayer){
			myTransform.rotation=Quaternion.Lerp(myTransform.rotation, syncPlayerRotation, Time.deltaTime);

		}

	}

	/*[Client]
	void SyncRotationValues(Quaternion latestPos){

		syncPlayerRotation = latestPos;


	}*/

	[Command]
	void CmdProvideRotationstoServer(Quaternion playerRot)
	{
		
		syncPlayerRotation=playerRot;
	}


	[ClientCallback]
	void TransmitRotations()
	{
		if(isLocalPlayer){
			if(Quaternion.Angle(myTransform.rotation,lastPlayerRot)>threshold){
				CmdProvideRotationstoServer(myTransform.rotation);

				lastPlayerRot=myTransform.rotation;
			}

		}
	}








}
