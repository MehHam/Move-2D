using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

[NetworkSettings(channel=0, sendInterval=0.05f)]
public class Player_SyncPosition : NetworkBehaviour { //script to Sync motion parameter
	[SyncVar (hook ="SyncPositionValues")] private Vector3 syncPos;

		
	[SerializeField] Transform myTransform;
	[SerializeField] float learpRate;
	[SerializeField] private bool useHistoricalLearping=false;


	//private Quaternion syncRot;
	//private NetworkClient nclient;
	//private int latency;
	private Text latencyText;
	private List<Vector3> syncPosList = new List<Vector3>();
	private float normalLerpRate = 16;
	private float FasterLearpRate=27;
	private float closeEnough=0.1f;


	void Start(){
	

		//nclient=GameObject.Find("Manager_Network").GetComponent<NetworkManager>().client;
		//latencyText=GameObject.Find ("LatencyText").GetComponent<Text>();
		learpRate=normalLerpRate;

	}

	void Update () {
		
		LearpPosition();
		ShowLatency();
		
	}

	// Update is called once per frame
	void FixedUpdate () {

		TransmitPosition();
	
	
	}

	void LearpPosition()
	{
		if(!isLocalPlayer){
			if(useHistoricalLearping)
			{
				HistoricalLearping();
			}
			else{

				OrdinaryLearping();
			
			}
		}

	}

	/*void LearpRotation()
	{
		if(!isLocalPlayer){
			myTransform.rotation=Vector3.Lerp(myTransform.rotation,syncRot,Time.deltaTime*learpRate);
		}
		
	}*/

	[Command]
	void CmdProvidePositiontoServer(Vector3 pos)
	{
		syncPos=pos;

	}

	/*[Command]
	void CmdProvideRotationtoServer(Quaternion playerRot)
	{
		syncRot=playerRot;
	}*/

	[ClientCallback]
	void TransmitPosition()
	{
		if(isLocalPlayer){
			CmdProvidePositiontoServer(myTransform.position);

		}
	}

	[Client]
	void SyncPositionValues(Vector3 latestPos){

		syncPos = latestPos;
		syncPosList.Add(syncPos);

	}


	void ShowLatency()
	{
		if(isLocalPlayer)
		{
			//latency = nclient.GetRTT();

		}
	}

	void OrdinaryLearping(){
	
		myTransform.position=Vector3.Lerp(myTransform.position,syncPos,Time.deltaTime*learpRate);
		
	
	}

	void HistoricalLearping(){
	
		if(syncPosList.Count>0){

			myTransform.position=Vector3.Lerp(myTransform.position,syncPosList[0],Time.deltaTime*learpRate);

			if(Vector3.Distance(myTransform.position,syncPosList[0])<closeEnough){

				syncPosList.RemoveAt(0);
			}

			if(syncPosList.Count>10)
			{
				learpRate=FasterLearpRate;
			}else{
				learpRate= normalLerpRate; 

			}

		
		}

	}

}
