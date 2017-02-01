using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerID : NetworkBehaviour {

	[SyncVar] public string playerUniqueIdentity;
	private NetworkInstanceId playerNetID;
	//public static NetworkInstanceId playerNetID;
	//private Transform myTransform;
	public Gyroscope gyro;
	//public Rigidbody rb;
	const int accCte = 50;
	[SerializeField] Transform myTransform;

	public override void OnStartLocalPlayer()
	{
		//base.OnStartLocalPlayer();
		GetNetIdentity();
		SetIdentity();

	
	}

	void Awake(){

		myTransform = transform;

	}

	// Update is called once per frame
	void Update () {
	
		if(myTransform.name=="" ||myTransform.name=="Sphere(Clone)"||myTransform.name=="Sphere")
		{
			SetIdentity();

		}
	
	}


	[Client]
	void GetNetIdentity()
	{
		playerNetID=GetComponent<NetworkIdentity>().netId;
		CmdTellServerMyIdentity(MakeUniqueIdentity());


	}

	void SetIdentity()
	{
		if(!isLocalPlayer)
		{
			myTransform.name=playerUniqueIdentity;
		}else{
			myTransform.name=MakeUniqueIdentity();
		}
	}

	string MakeUniqueIdentity(){
		string uniqueName="Player " + playerNetID.ToString();
		return uniqueName;

	}
	[Command]
	void CmdTellServerMyIdentity(string name)
	{
		playerUniqueIdentity=name;
	}
}
