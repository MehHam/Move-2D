using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class Player_Health : NetworkBehaviour {
	[SyncVar (hook="OnHealthChanged")] private int health=0;
	private Text healthText;
	// Use this for initialization
	void Start () {

		healthText=GameObject.Find("FPSCounter").GetComponent<Text>();
		SetHealthtext();
	
	}
	
	// Update is called once per frame
	void Update () {
		SetHealthtext();
	}

	void SetHealthtext()
	{
		//if (isLocalPlayer)
		//{
			healthText.text="Score : " + health.ToString();
			
		//}

	}

	public void increaseHealth(int dmg){

		health = dmg;

	}

	void OnHealthChanged(int hlth)
	{
		health =hlth;
		SetHealthtext();
	}


}
