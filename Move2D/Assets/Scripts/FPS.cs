using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ProgressBar;
using UnityEngine.Networking;

public class FPS : MonoBehaviour 
{

//	private float updateInterval = 0.5f;
//	private physics phys=new physics();

	//private float accum = 0; // FPS accumulated over the interval
	//private int frames = 0; // Frames drawn over the interval
	//public float timeleft ; // Left time for current interval
	//public float timeElapsed;
	//public float deadTime; 
	private Text FPScounter;
	public float tempo=0;


	// Use this for initialization
	void Start () {

	
	
			FPScounter = GetComponent<Text>();
			//print ("FramesPerSecond needs a GUIText component!");
			
			
	
	//	timeleft = updateInterval;  
	//	timeElapsed=0.0f;

		//deadTime=physics.endGame;
	
	}

	
	// Update is called once per frame
	void Update () {
	
			/* 
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
		timeElapsed += Time.deltaTime;*/
		//tempo= (int) (levelDesign.endGame-levelDesign.localTime + levelDesign.localTime0);
		Debug.Log(tempo);
		// Interval ended - update GUI text and start new interval
		/*if( timeleft <= 0.0 )
		{
			// display two fractional digits (f2 format)
			//  GetComponent.<GUIText>().text = "Time	" + (timeElapsed).ToString("f2");//(accum/frames).ToString("f2");
			//GetComponent<GUIText>().text = "Time	" + (physics.endGame-(int)Network.time).ToString("f2");//(tempo).ToString("f2");//(accum/frames).ToString("f2");
			timeleft = updateInterval;
			accum = 0.0f;
			frames = 0;
		}*/

		if(physics.playerArr.Length>=physics.playerLimit)
		{
			//FPScounter.text = "Time	" + levelDesign.localTime.ToString("f2");//(tempo).ToString("f2");//(accum/frames).ToString("f2");

		}


	}


}