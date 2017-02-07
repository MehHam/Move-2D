using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject serverCanvas, clientCanvas;
    public ClientHUD clientHUDScript;
	public ServerHUD serverHUDScript;

	void Start()
	{
		var manager = GameObject.Find ("Manager_Network");
		serverCanvas = manager.transform.FindChild("Canvas_Server").gameObject;
		clientCanvas = manager.transform.FindChild ("Canvas_Client").gameObject;
		clientHUDScript = manager.GetComponent<ClientHUD>();
		serverHUDScript = manager.GetComponent<ServerHUD>();
	}

    public void StartServer()
    {
        serverHUDScript.enabled = true;
		clientHUDScript.enabled = false;
		clientCanvas.SetActive (false);
        serverCanvas.SetActive (true);
       // SceneManager.LoadScene("ServerClientMenu");
    }

    public void StartClient()
    {
		serverHUDScript.enabled = false;
        clientHUDScript.enabled = true;
		clientCanvas.SetActive (true);
		serverCanvas.SetActive (false);
      //  SceneManager.LoadScene("ServerClientMenu");
    }
}
