using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class PlayerScore : NetworkBehaviour
{
	[SyncVar (hook = "OnScoreChanged")] private int _score = 0;
	public Text scoreText;
	// Use this for initialization
	void Start ()
	{
		SetHealthtext ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		SetHealthtext ();
	}

	void SetHealthtext ()
	{
		scoreText.text = "Score : " + _score.ToString ();
	}

	public void increaseHealth (int score)
	{
		_score = score;
	}

	void OnScoreChanged (int score)
	{
		this._score = score;
		SetHealthtext ();
	}


}
