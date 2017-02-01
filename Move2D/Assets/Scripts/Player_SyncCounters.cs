using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

[NetworkSettings(channel=1, sendInterval=0.05f)]
public class Player_SyncCounters : NetworkBehaviour { //script to Sync motion parameter
	
	[SyncVar] public int syncPlayerCounter;
	
	[SerializeField] public static int count;
	[SerializeField] float learpRate;
	//[SerializeField] private bool useHistoricalInterpolation=false;
	
	
	//private Quaternion syncRot;
	private NetworkClient nclient;
	//private int latency;
	//private Text latencyText;
	//private List<Vector3> syncPlayerRotList = new List<Vector3>();
	private float normalLerpRate = 20;
	//private float FasterLearpRate=30;
	//private float closeEnought=0.5f;
	
	private float threshold =1;
	private int lastPlayerCounter;
	
	
	
	void Start(){
		
		learpRate=normalLerpRate;
		
	}
	
	void Update () {
		
		LearpCounter();
		
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		TransmitCounter();
		
		
		
	}
	
	void LearpCounter()
	{
		if(!isLocalPlayer){
			count=syncPlayerCounter;
			
		}
		else
			count=syncPlayerCounter;
		
	}
	
	/*[Client]
	void SyncRotationValues(Quaternion latestPos){

		syncPlayerCounter = latestPos;


	}*/
	
	[Command]
	void CmdProvideCountertoServer(int playerCounter)
	{
		
		syncPlayerCounter=playerCounter;
	}
	
	
	[ClientCallback]
	void TransmitCounter()
	{
		if(isLocalPlayer){
		
				CmdProvideCountertoServer(count);
				
				//lastPlayerCounter =physics.localTime;
		
			
		}
	}
	
	
	
	
	
	
	
	
}
