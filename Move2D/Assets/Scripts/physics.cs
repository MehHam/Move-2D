using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ProgressBar;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class physics: NetworkBehaviour
{ 
	private Rigidbody2D rigidBod;

	//public static float translation = 3*Time.deltaTime;

	public int totalMass=1;

	public static Player joueurCDM ;
	public GameObject goCDM;


	public GameObject[] playersArrays;
	public bool isRegularCDM=true;
	private Vector2 forwardCDMposition;
	
	private Vector2 rigidCDMpos;


	private GameObject go;

	public static Player[] playerArr;
	private Vector2 posThe;
	private int numOfPlayers;
	public static int playerLimit=2;

	void Start()
    {
    }

	// Update is called once per frame

	void Update ()
    {
		playersArrays=findPlayers();


		if(goCDM==null|| joueurCDM == null){

			goCDM = GameObject.Find("SphereCDM");
			joueurCDM=new Player(goCDM.name,/*goCDM.GetComponent<Rigidbody2D>(),*/0,goCDM,goCDM.transform.position,new ArrayList{},Color.red);
			goCDM.transform.position=cdmPosition(playerArr,totalMass);
			playerArr=playersArrayConstruction(playersArrays);	
		}
		if(playersArrays.Length==playerLimit)
		{
			playerArr=playersArrayConstruction(playersArrays);

			joueurCDM.updatebufferPosition(joueurCDM);

			for(int i =0;i<playerArr.Length;i++)
			{
				playerArr[i].updatebufferPosition(playerArr[i]);
			}
			totalMass= sumMass(playerArr);

			Rigidbody2D rb;
			rb=joueurCDM.go.GetComponent<Rigidbody2D>();
			//rb.MovePosition(cdmPosition(playerArr,totalMass));
			forwardCDMposition=cdmPosition(playerArr, totalMass);
		
			//rigidCDMpos=((playerArr[0].masse*(Vector2)playerArr[0].go.transform.position) + (playerArr[1].masse*(Vector2)playerArr[1].go.transform.position) )/totalMass;

			for(int i=0;i<playerArr.Length;i++)
			{
				rigidCDMpos += (Vector2)playerArr[i].go.transform.position*(playerArr[i].mass*1.0f)/(float)totalMass; 
			}


		if(levelDesign.levelValue!=3)
			{
				rb.MovePosition(forwardCDMposition);

			}else{

			if(levelDesign.switchLevel3==false) 
			{
			if(forwardCDMposition==rigidCDMpos)
			{
				rb.MovePosition(forwardCDMposition);
				//float alpha = Mathf.Atan(playerArr[0].go.transform.position.y/playerArr[0].go.transform.position.x);
				//playerArr[1].go.GetComponent<Rigidbody2D>().MovePosition(new Vector2(Vector3.Magnitude(playerArr[0].go.transform.position)*Mathf.Cos(alpha+Mathf.PI),Vector3.Magnitude(playerArr[0].go.transform.position)*Mathf.Sin(alpha+Mathf.PI)));
			}else{
					//float alpha = Mathf.Atan(playerArr[0].go.transform.position.y/playerArr[0].go.transform.position.x);
					//playerArr[1].go.GetComponent<Rigidbody2D>().MovePosition(new Vector2(Vector3.Magnitude(playerArr[0].go.transform.position)*Mathf.Cos(alpha+Mathf.PI),Vector3.Magnitude(playerArr[0].go.transform.position)*Mathf.Sin(alpha+Mathf.PI)));
					//playerArr[1].go.GetComponent<Rigidbody2D>().MovePosition((Vector2)joueurCDM.go.transform.position+ new Vector2(joueurCDM.go.transform.position.x- playerArr[0].go.transform.position.x, joueurCDM.go.transform.position.y- playerArr[0].go.transform.position.y));

					rb.MovePosition(forwardCDMposition);
					Debug.Log("not possible");
			}
		}
		} 
		}
	}

	public GameObject[] findPlayers()
	{
		  	
		GameObject[] arr= GameObject.FindGameObjectsWithTag("Player");

		GameObject[] arrGO;
		arrGO= new GameObject[arr.Length];
	
		for(int i =0; i < arr.Length;i++)
		{
			
			if(arr[i].name != "Sphere(Clone)" && arr[i].name != "Sphere")
				{
				arrGO[i] = arr[i];
				} 
		}
			
		return arrGO;
	}

	public Player[] playersArrayConstruction(GameObject[] arr)
	{
		Player[] tabPlayer = new Player[arr.Length];

		for(int i=0; i<tabPlayer.Length; i++)
		{

			//GameObject go;
			//go = GameObject.Find(arr[i].name);

			tabPlayer[i]=new Player(arr[i].name,/*arr[i].GetComponent<Rigidbody2D>(),*/1,arr[i],(Vector2)arr[i].transform.position,new ArrayList{},Color.red);
		//	Debug.Log(tabPlayer[i].namePlayer);

		}

		return tabPlayer;
	}


	public int sumMass(Player[] arr)
	{
		int sum=0;
		for(int i=0;i< arr.Length;i++)
		{
			sum+=arr[i].mass;

		}
		return sum;
	}

	public Vector2 cdmPosition(Player[] arr, int sum)
	{
		float somme =1.0f*sum;
		Vector2 posTemp=Vector2.zero;

		Vector2 posCDM=joueurCDM.go.transform.position;
		if(arr != null && arr.Length>=2){
		
		for(int i=0;i<arr.Length;i++)
		{
				posTemp += (Vector2)arr[i].go.transform.position*(arr[i].mass*1.0f)/(float)somme; 
		}
		posCDM=posTemp;

		}else{	
		if(arr != null && arr.Length==1){
			posCDM=(Vector2)arr[0].go.transform.position;
			}else{
		posCDM=new Vector2(0,0);
		}
		}
		return posCDM;
	}

} 
