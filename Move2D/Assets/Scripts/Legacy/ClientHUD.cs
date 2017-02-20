using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

/*
public class ClientHUD : MonoBehaviour
{

    public GameObject connectToServer, disConnect, addressPanel, connecting, menuCam, disConnectMessage;
    public InputField portText, ipText, passwordText;
    public Text connectingText;

    private NetworkManager manager;

    private bool connected;

	private const int connectingTimer = 8; //how long we try to connect until the fail message appears.
	private const int failTimer = 2; //how long the fail message is showing.

    // Use this for initialization
    void Start()
    {
        if (!manager)
            manager = GetComponent<NetworkManager>();

        //checking if we have saved server info.
        if (PlayerPrefs.HasKey("nwPortC"))
        {
            manager.networkPort = Convert.ToInt32(PlayerPrefs.GetString("nwPortC"));
            portText.text = PlayerPrefs.GetString("nwPortC");
        }
        if (PlayerPrefs.HasKey("IPAddressC"))
        {
            manager.networkAddress = PlayerPrefs.GetString("IPAddressC");
            ipText.text = PlayerPrefs.GetString("IPAddressC");
        }
    }

	void OnEnable()
	{
		ToggleInteraction (true);
	}

    void Update()
    {
    }

	private void ToggleInteraction(bool interactable)
	{
		var server = GameObject.Find ("/Canvas_Main/Panel_Mid/Button_Server");
		Button server_button = null;
		if (server != null)
			server_button = server.GetComponent<Button> ();
		var client = GameObject.Find ("/Canvas_Main/Panel_Mid/Button_Client");
		Button client_button = null;
		if (client != null)
			client_button = server.GetComponent<Button> ();

		if (server_button != null)
			server_button.interactable = interactable;
		if (client_button != null)
			client_button.interactable = interactable;
		connectToServer.GetComponent<Button> ().interactable = interactable;
		portText.interactable = interactable;
		ipText.interactable = interactable;
		passwordText.interactable = interactable;
	}

	IEnumerator Timer(int timer)
	{
		var timerCount = timer;
		while (true)
		{
			timerCount--;
			if (timerCount <= 0 || connected)
				break;
			yield return new WaitForSeconds (1);
		}
	}

	IEnumerator ConnectionTimer()
	{
		connectingText.text = "Connecting !!";
		connecting.SetActive(true);
		ToggleInteraction (false);

		yield return Timing.RunCoroutine (Timer(connectingTimer));
		if (!connected) {
			connectingText.text = "Connection Failed !!";
			yield return Timing.RunCoroutine (Timer (failTimer));
		}
		connecting.SetActive (false);
		ToggleInteraction (true);
	}

    public void ConnectToServer()
    {
		Debug.Log ("Connect to server");
        if (ipText.text != "" && portText.text != "")//is the information filled in ?.
        {
            connected = false;
            disConnectMessage.SetActive(false);
			Timing.RunCoroutine("ConnectionTimer", ConnectionTimer());
            manager.networkAddress = ipText.text;
            manager.networkPort = Convert.ToInt32(portText.text);
            PlayerPrefs.SetString("IPAddressC", ipText.text);//saving the filled in ip.
            PlayerPrefs.SetString("nwPortC", portText.text);//saving the filled in port.

            manager.StartClient();
        }
    }

    //called by the CustomNetworkManager.
    public void ConnectSuccses()
    {
		Debug.Log ("Connect Success");
        connected = true;
        connecting.SetActive(false);
		HideConnectPanels ();
		ToggleInteraction (true);
        //menuCam.SetActive(false);   //if your player has a camera on him this one should be turned off when entering the game/lobby.
    }

    public void ButtonDisConnect()
    {
        DisConnect(false);
    }

	public void HideConnectPanels()
	{
		disConnect.SetActive(true);
		connectToServer.SetActive(false);
		addressPanel.SetActive(false);
	}

	public void ShowConnectPanels()
	{
		connectToServer.SetActive(true);
		disConnect.SetActive(false);
		addressPanel.SetActive(true);
	}

    public void DisConnect(bool showMessage)
    {
        if (showMessage)
            disConnectMessage.SetActive(true);
		ShowConnectPanels ();
        //menuCam.SetActive(true);  //turn the camera on again when returning to menu scene.
        manager.StopClient();
    }
}
*/