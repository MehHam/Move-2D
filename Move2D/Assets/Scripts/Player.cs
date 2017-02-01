using UnityEngine;
using System.Collections;

public class Player {

	public static ArrayList playersNamesList;
	// Use this for initialization

	public string namePlayer; 
	//public Rigidbody2D rigidB;
	public int masse; 
	public GameObject go; 
	public Vector2 originPlayer; 
	public ArrayList bufferPosition;
	public Color color;

	public Player(){} 


	public Player(string namePlayer,/* Rigidbody2D rigidB, */int masse, GameObject go, Vector2 originPlayer, ArrayList bufferPosition,Color color)
	{ 					//contructor
		this.namePlayer = namePlayer;
		//this.rigidB=rigidB;
		this.masse=masse;
		this.go=go;
		this.originPlayer=originPlayer;
		this.bufferPosition=bufferPosition;
		this.color=color;

	} 


	public void InitializePlayer(Player joueur)
	{
		joueur.playerOrigin(joueur.originPlayer);
		joueur.bufferPosition.Clear();

	}

	public void setMass(int mass){
		masse= mass;
	}

	public void setRigidb(Rigidbody2D rigidB){
		rigidB= rigidB;
	}

	public string getName(){
	
		namePlayer=go.name;
		return namePlayer;
	}

	public ArrayList updatebufferPosition(Player joueur){
	
		joueur.bufferPosition.Add(go.transform.position);
		return joueur.bufferPosition;
	}

	public ArrayList addPlayertoList(ArrayList playerList,Player joueur)
	{
		playerList.Add(joueur);
		return playerList;
	}

	public Vector2 playerOrigin(Vector2 originGO)
	{
		originPlayer=originGO;
		return originGO;
	}

	public ArrayList playerMotion(Vector2 posPLAYER)
	{
	
		bufferPosition.Add(posPLAYER);
		return bufferPosition;
	}

	public ArrayList addPlayersNames(){

		playersNamesList.Add(namePlayer);
		return playersNamesList;
	}
	


}
