using UnityEngine;
using System.Collections;

public class LineDrawer : MonoBehaviour {

		private  GameObject go1;
		private  GameObject go2;
		public  GameObject goCdM;
		private GameObject goPlayer1;
		private Rigidbody rigidBod;
		private NetworkCharac NetwChr = new NetworkCharac();
		private physics phys=new physics();
		private Player joueur=new Player();

	//	public static float translation = Time.deltaTime * 0.01f;
		public LineRenderer[] LineRenderers;
		
		
		//Camera camera;
		
		void Start()
		{
				
		}
		
		// Update is called once per frame
		void Update ()
		{


		Debug.Log (physics.joueurCDM.namePlayer);
		Debug.Log (physics.playerArr[0].namePlayer);
		Debug.Log (physics.playerArr[1].namePlayer);


		if(levelDesign.levelValue>= 1 && levelDesign.levelValue<=3)
		{
		DrawLine(physics.joueurCDM.originPlayer, physics.joueurCDM.go.transform.position,physics.joueurCDM.bufferPosition,0,physics.joueurCDM.go);

		for(int i=0;i< physics.playerArr.Length;i++)
		{

			DrawLine(physics.playerArr[i].originPlayer, physics.playerArr[i].go.transform.position,physics.playerArr[i].bufferPosition,1+i,physics.playerArr[i].go);
			DrawLinker(physics.playerArr[i].namePlayer,physics.joueurCDM.namePlayer,50,5+i);

		}
		
		}
		

	}

	void gameObjectColorChanger(GameObject go,Color color){

		go.GetComponent<Renderer>().material.color=color;

	
	}

void DrawLine(Vector3 origin, Vector3 end, ArrayList arr, int num,GameObject go)
	{
		LineRenderers[num].SetVertexCount(arr.Count);


		
	if(go==physics.joueurCDM.go || go.tag=="Player"){//go==(GameObject)NetworkCharac.gameObjectsArray.ToArray()[2]){
			//PointAlongLine = Vector3.Normalize(end - origin) + origin;
			Vector3[] points = new Vector3[arr.Count];

			for(int i=0;i<= arr.Count-1; i++){
				points[i] = (Vector3)arr.ToArray()[i];
			}

			LineRenderers[num].SetPositions(points);
			LineRenderers[num].SetWidth(2,5);
		}else{
			Debug.Log("pas de ligne");
		}
	}
		
		void DrawLinker(string go1name, string go2name, int increment,int num)
	{
		go1=GameObject.Find(go1name);
		go2=GameObject.Find(go2name);
		Vector3[] arr = new Vector3[increment];

		for(int i = 0;i <=increment-1;i++)
		{

			//arr.Add(go1.transform.position + i*((go2.transform.position -  go1.transform.position)/increment)) ;
			arr[i]=go1.transform.position + i*((go2.transform.position -  go1.transform.position)/increment);
		
		}
			LineRenderers[num].SetVertexCount(arr.Length);
			LineRenderers[num].SetPositions(arr);
			LineRenderers[num].SetWidth(5,5);
	}
		
		GameObject getGO(ArrayList arr, int index){
		GameObject go;
		go=(GameObject)arr.ToArray()[index];
		return go;
	}

		string goName(ArrayList arr, int index){
		GameObject go;
		go=(GameObject)arr.ToArray()[index];
		return go.name;
		}

}