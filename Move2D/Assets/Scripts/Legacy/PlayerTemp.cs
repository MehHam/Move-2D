using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerTemp
{
	public static ArrayList playersNamesList;
	// Use this for initialization

	public string namePlayer;
	//public Rigidbody2D rigidB;
	public int mass;
	public GameObject go;
	public Vector2 originPlayer;
//	public List<Vector2> bufferPosition;
	public Color color;

	public PlayerTemp ()
	{
	}


	public PlayerTemp (string namePlayer,/* Rigidbody2D rigidB, */int masse, GameObject go, Vector2 originPlayer, ArrayList bufferPosition, Color color)
	{ 					//contructor
		this.namePlayer = namePlayer;
		//this.rigidB=rigidB;
		this.mass = masse;
		this.go = go;
		this.originPlayer = originPlayer;
//		this.bufferPosition = bufferPosition;
		this.color = color;
	}


	public void InitializePlayer (PlayerTemp joueur)
	{
		joueur.playerOrigin (joueur.originPlayer);
//		joueur.bufferPosition.Clear ();

	}

	public void setMass (int mass)
	{
		this.mass = mass;
	}

	/*
	public void setRigidb(Rigidbody2D rigidB){
		this.rigidB = rigidB;
	}
	*/

	public string getName ()
	{
	
		namePlayer = go.name;
		return namePlayer;
	}
/*
	public List<Vector2> updatebufferPosition ()
	{		bufferPosition.Add (this.gameObject.transform.position);
	}
*/

	public ArrayList addPlayertoList (ArrayList playerList, PlayerTemp joueur)
	{
		playerList.Add (joueur);
		return playerList;
	}

	public Vector2 playerOrigin (Vector2 originGO)
	{
		originPlayer = originGO;
		return originGO;
	}
/*
	public ArrayList playerMotion (Vector2 posPLAYER)
	{
		bufferPosition.Add (posPLAYER);
		return bufferPosition;
	}
	*/

	public ArrayList addPlayersNames ()
	{
		playersNamesList.Add (namePlayer);
		return playersNamesList;
	}
	


}
