using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkTime : MonoBehaviour {
	public void SendTime()
	{
		CustomNetworkManager.TimeMessage msg = new CustomNetworkManager.TimeMessage();
		msg.time = Time.time;
		NetworkServer.SendToAll(CustomNetworkManager.CustomMessageType.TimeMessage, msg);
	}

	void FixedUpdate()
	{
		SendTime ();
	}
}
