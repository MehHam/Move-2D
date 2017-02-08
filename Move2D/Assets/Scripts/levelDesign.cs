using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ProgressBar;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class levelDesign: NetworkBehaviour
{
	[SyncVar] public int localTime = 0;
	[SyncVar] public int endGame;
	[SyncVar] public int levelNum = 0;
	[SyncVar] public bool level3switch;

	private int level0time=9;//30

	private int level1time=25;//25
	private int level11time=40;//40

	private int level2time=30;//30
	private int level22time=40;//40
	private int level3time=40;//40
	private int level4time=10;//10
	private int level5time=2;//2

	//public static int levelPublic=-1;
	public static int level2Timerlenght;

	public GameObject progressBar;
	public static int levelValue;
	public static bool switchLevel3;
	public static bool switchLevel11 = true;
	public static bool switchLevel22;

	public static int localTime0 = 0;
	private Text timeText;
	private Text levelText;
	public GameObject goCDM;
	private int level0timer;

	void Start ()
	{
		level3switch = true;
	}

	void Update ()
	{
		timeUpdate ();
		SetTimeText (localTime);
		SetLevelText (levelNum);
		levelGestion (levelNum);
		levelValue = levelNum;


		/*if(levelNum==11 && switchLevel11==true){

			physics.playerArr[0].go.transform.position=GameObject.Find("StartPosition1").transform.position;
			//physics.playerArr[1].go.transform.position=GameObject.Find("StartPosition3").transform.position;

			switchLevel11=false;
		}*/

		if(levelNum==4)
		{
			int timeStamp4 = level3time+level11time+level2time+level22time+level1time+level0time;
			progressBar=GameObject.Find("ProgressRadialBar");
			progressBar.GetComponentInChildren<ProgressRadialBehaviourMOD>().Value = 100.0f*(localTime-timeStamp4)/level4time;
		}

		
		if (levelNum == 5) {
			levelNum = 0;
			localTime0 = 0;
			level3switch = true;
			
			level2Timerlenght = level3time;
			motionPointFollow.motionMode = (int)Random.Range (-0.0f, 2.9f);
			progressBar.GetComponentInChildren<ProgressRadialBehaviourMOD> ().Value = 0;

		}

	}


	void timeUpdate ()
	{
		if (!isLocalPlayer)
			return;
		
		if (physics.players != null && physics.players.Count >= physics.playerLimit) {
			if (localTime0 != 0) {
				localTime = (int)CustomNetworkManager.s_singleton.GetEstimatedServerTime () - localTime0;
				levelNum = levelState (localTime, level0time, level1time, level11time, level2time, level22time, level3time, level4time, level5time);
				CmdTellServerLevel (localTime, endGame, levelNum, level3switch);
				//SetTimeUpdate(localTime);

			} else {
				endGame = level0time + level1time + level2time + level11time + level22time + level3time + level4time + level5time + localTime0;
				localTime0 = (int)CustomNetworkManager.s_singleton.GetEstimatedServerTime (); 
				endGame = level0time + level1time + level2time + level3time + level4time + level5time + level11time + level22time;

			}
		}

	}



	void positionSwitch (GameObject go, Vector3 pos)
	{
		go.GetComponent<Rigidbody2D> ().transform.position = pos;
	}


	void levelGestion (int lev)
	{
	
		switch (lev) {
			
		case 5:
			gestionGO ("Level0", "Canvas", true);
			gestionGO ("Level1", "Canvas", false);
			gestionGO ("Level2", "Canvas", false);
			gestionGO ("Level3", "Canvas", false);
			gestionGO ("Level4", "Canvas", false);

			break;

		case 4:
			GameObjectAlphaHide ("permanent", 0);
			gestionGO ("Background0", "Main_Background", true);
			gestionGO ("Background2bis", "Main_Background", false);
			
			gestionGO ("Level4", "Canvas", true);
			gestionGO ("Level2", "Canvas", false);
			
			break;
		

		case 22:
			gestionGO ("Level2", "Canvas", true);
			gestionGO ("Background2bis", "Main_Background", true);
			gestionGO ("Background1bis", "Main_Background", false);

			//physics.playerArr[1].go.transform.position=GameObject.Find("StartPosition3").transform.position;

			GameObject.Find ("SphereCDM").transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);			


			break;
		

		case 11:
			gestionGO ("Level2", "Canvas", false);
			GameObject.Find ("SphereCDM").transform.localScale = new Vector3 (2.0f, 2.0f, 2.0f);	
			motionPointFollow.objectFollow = GameObject.Find ("pointFollow");
			motionPointFollow.motionMode = 1;
			gestionGO ("Background1bis", "Main_Background", true);
			gestionGO ("Background2", "Main_Background", false);

			break;

			
		
		case 3:

			gestionGO ("Background2", "Main_Background", true);
			GameObject.Find ("SphereCDM").transform.localScale = new Vector3 (0.8f, 0.8f, 0.8f);
			motionPointFollow.motionMode = 2;

			break;
			
		case 2:
			motionPointFollow.objectFollow = GameObject.Find ("pointFollow");
			motionPointFollow.motionMode = 0;

			gestionGO ("Background2", "Main_Background", true);
			gestionGO ("Background1", "Main_Background", false);

			gestionGO ("Level2", "Canvas", true);
			gestionGO ("Level1", "Canvas", false);

			GameObject.Find ("SphereCDM").transform.localScale = new Vector3 (1, 1, 1);			
			level2Timerlenght = level3time;

			break;
		case 1:
			GameObjectAlphaHide ("permanent", 1);

			level2Timerlenght = level2time;
			motionPointFollow.modeTimer = level2time * 24;
			motionPointFollow.modeCounter = 10 + level2Timerlenght * 24;

			gestionGO ("Background1", "Main_Background", true);
			gestionGO ("Background0", "Main_Background", false);
			gestionGO ("Level1", "Canvas", true);
			gestionGO ("Level0", "Canvas", false);
		
						
			break;
		case 0:
			GameObjectAlphaHide ("permanent", 0);

			gestionGO ("Background0", "Main_Background", true);
			gestionGO ("Level0", "Canvas", true);
					
			GameObject.Find ("SphereCDM").transform.localScale = new Vector3 (2.0f, 2.0f, 2.0f);
						
			if (localTime < (int)(0.4f * level0time) && localTime >= 0) {
				gestionGO ("ready", "Level0", true);
				gestionGO ("set", "Level0", false);
				gestionGO ("go", "Level0", false);
				
			}
			if (localTime < (int)(0.8f * level0time) && localTime > (int)(0.4f * level0time)) {
				gestionGO ("ready", "Level0", false);
				gestionGO ("set", "Level0", true);
				gestionGO ("go", "Level0", false);
				
				
			}
			if (localTime <= level0time && localTime > (int)(0.8f * level0time)) {
				gestionGO ("ready", "Level0", false);
				gestionGO ("set", "Level0", false);
				gestionGO ("go", "Level0", true);
			}
			
			break;
		case -1:
			SceneManager.LoadScene ("MainMenu");
			break;
		}

	}

	void gestionGO (string goName, string parentName, bool status)
	{

		Transform transfo = GameObject.Find (parentName).transform;
		Transform GOT = transfo.FindChild (goName);
		GOT.gameObject.SetActive (status);

	}

	int levelState (int timer, int level0time, int level1time, int level11time, int level2time, int level22time, int level3time, int level4time, int level5time)
	{
		int lev = -2;
		if (timer >= 0 && timer < level0time)
			lev = 0;
		if (timer >= level0time && timer < level1time + level0time)
			lev = 1;
		if (timer >= level1time + level0time && timer < level2time + level1time + level0time)
			lev = 2;
		if (timer >= level2time + level1time + level0time && timer < level3time + level2time + level1time + level0time)
			lev = 3;
		if (timer >= level3time + level2time + level1time + level0time && timer <= level11time + level3time + level2time + level1time + level0time)
			lev = 11;
		if (timer >= level11time + level3time + level2time + level1time + level0time && timer <= level22time + level11time + level3time + level2time + level1time + level0time)
			lev = 22;
		if (timer >= level22time + level11time + level3time + level2time + level1time + level0time && timer <= level4time + level22time + level11time + level3time + level2time + level1time + level0time)
			lev = 4;
		if (timer >= level4time + level22time + level11time + level3time + level2time + level1time + level0time && timer <= level5time + level4time + level22time + level11time + level3time + level2time + level1time + level0time)
			lev = 5;
		
		/*if(timer>level5time+level4time+level3time+level2time+level1time+level0time)
			lev=0;*/
		
		return lev;
		
	}

	void SetTimeText (int localTi)
	{
		timeText = GameObject.Find ("timeScore").GetComponent<Text> ();
		timeText.text = "Time : " + (endGame - (localTi)).ToString ();
	}

	void SetLevelText (int localLev)
	{
		levelText = GameObject.Find ("levelText").GetComponent<Text> ();
		levelText.text = "Level : " + localLev.ToString ();
	}

	void GameObjectAlphaHide (string goName, int AlphaValue)
	{
		GameObject goTemp = GameObject.Find (goName);
		Color couleur;
		
		if (goTemp.name == "permanent") {
			foreach (Transform child in goTemp.transform) {
			
				couleur = child.GetComponent<Text> ().color;
				couleur.a = AlphaValue;
				child.GetComponent<Text> ().color = couleur;
			}
		}
		/*if(goTemp.name =="ready"|| goTemp.name =="set"||goTemp.name =="go"||goTemp.name =="Arrow"  )
		{
			couleur = goTemp.GetComponent<Image>().color;
			couleur.a = AlphaValue;
			goTemp.GetComponent<Image>().color=couleur;
		}*/
		
	}


	[Command]
	void CmdTellServerLevel (int time, int endG, int lev, bool levelswitch)
	{

		localTime = time;
		endGame = endG;
		levelNum = lev;
		level3switch = levelswitch;
		levelGestion (levelNum);
		//Apply the dammage

	}


} 



