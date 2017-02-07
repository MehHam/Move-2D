using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using System;

public class CustomNetworkManager : NetworkManager
{
	public class CustomMessageType {
		public static short TimeMessage = MsgType.Highest + 1;
		public static short PasswordMessage = MsgType.Highest + 2;
	}

	public class TimeMessage: MessageBase {
		public float time;
	}

    public Text clientsInfoText;
    public ClientHUD clientHudScript;
    public ServerHUD serverHudScript;

    private int connectedClients = 0;
	private float _offset = 0.0f;

    [HideInInspector]
    public string serverPassword;

	public static CustomNetworkManager singleton;

	void Awake()
	{
		if (FindObjectsOfType<CustomNetworkManager>().Length > 1)
			Destroy (gameObject);
	}
	void Start()
	{
		CustomNetworkManager.singleton = this;
		DontDestroyOnLoad (gameObject);
	}

    //Server Side
	public override void OnStartHost()
    {
        base.OnStartHost();
        RegisterServerHandles();

        serverPassword = serverHudScript.passwordText.text;
        connectedClients = 0;
        clientsInfoText.text = "Connected Clients : " + connectedClients;
    }

    //keeping track of Clients connecting.
    public override void OnServerConnect(NetworkConnection conn)
    {
		Debug.Log ("Connect");
        base.OnServerConnect(conn);
		connectedClients = NetworkServer.connections.Count;
        clientsInfoText.text = "Connected Clients : " + connectedClients;

        //Sending password information to client.
        StringMessage msg = new StringMessage(serverPassword);
        NetworkServer.SendToClient(conn.connectionId, MsgType.Highest + 1, msg);
    }

    //keeping track of Clients disconnecting.
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        connectedClients -= 1;
        clientsInfoText.text = "Connected Clients : " + connectedClients;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
    }

    //Client Side
    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
        RegisterClientHandles();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        clientHudScript.ConnectSuccses();
    }


	public override void OnClientDisconnect(NetworkConnection conn)
	{
		base.OnClientDisconnect(conn);
		clientHudScript.DisConnect(false);
	}
		
    //when client recieves password information from the server.
    public void OnReceivePassword(NetworkMessage netMsg)
    {
        //read the server password.
        var msg = netMsg.ReadMessage<StringMessage>().value;
        //serverPassword = msg;
        if (msg != clientHudScript.passwordText.text)
            clientHudScript.DisConnect(true);
    }

	//when client recieve time information from the server
	public void OnReceiveTime(NetworkMessage netMsg)
	{
		//read the server time
		var time = netMsg.ReadMessage<TimeMessage>().time;

		this._offset = time - Time.time;
	}

    //Messages that need to be Registered on Server and Client Startup.
    void RegisterServerHandles()
    {
        NetworkServer.RegisterHandler(MsgType.Highest + 1, OnReceivePassword);
    }

    void RegisterClientHandles()
    {
		client.RegisterHandler(CustomMessageType.PasswordMessage, OnReceivePassword);
		client.RegisterHandler (CustomMessageType.TimeMessage, OnReceiveTime);
    }

	public float GetEstimatedServerTime()
	{
		return Time.time + this._offset;
	}
}
