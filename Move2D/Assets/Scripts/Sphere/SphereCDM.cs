using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SphereCDM : NetworkBehaviour
{
	//OnTriggerEnter2D is called whenever this object overlaps with a trigger collider.
	[ServerCallback]
	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.GetComponent<IInteractable> () != null)
			other.gameObject.GetComponent<IInteractable> ().OnEnterEffect (this);
		//Update the currently displayed count by calling the SetCountText function.
		//SetCountText ();
		/*

		if (other.gameObject.name == "PetitChartres" || other.gameObject.name == "CrossLava")
			GameManager.singleton.score--;
		if (other.gameObject.name == "pointFollow")
			GameManager.singleton.score++;
		else {
			if (other.gameObject.name == "massEffect") {
				//gameObject.transform.position;
				for (int i = 0; i < this.GetComponent<SpherePhysics>().players.Count; i++) {
					int j = i + 1;
					this.GetComponent<SpherePhysics>().players [i].gameObject.transform.position = GameObject.Find ("masseffect" + j).transform.position;
					this.GetComponent<SpherePhysics>().players [i].mass = (i * 4) + 1;
				}
			}

			if (other.gameObject.name == "Teleporter1") {

				gameObject.transform.position = GameObject.Find ("Teleporter3").transform.position;
				for (int i = 0; i < this.GetComponent<SpherePhysics>().players.Count; i++) {
					int j = i + 1;
					this.GetComponent<SpherePhysics>().players [i].gameObject.transform.position = GameObject.Find ("Telep" + j).transform.position;
				}
			}

			if (other.gameObject.name == "Teleporter2") {
					
				gameObject.transform.position = GameObject.Find ("Teleporter4").transform.position;
				for (int i = 0; i < this.GetComponent<SpherePhysics>().players.Count; i++) {
					int j = i + 3;
					this.GetComponent<SpherePhysics>().players [i].gameObject.transform.position = GameObject.Find ("Telep" + j).transform.position;
				}
			}


			if (other.gameObject.name == "LabyrinthEnding1" || other.gameObject.name == "LabyrinthEnding2") {
				GameManager.singleton.score++;
			} else {
				if (other.gameObject.name == "PetitChartres" || other.gameObject.name == "CrossLava") {
					GameManager.singleton.score--;
				} else {

					if (other.gameObject.name == "Killer1" || other.gameObject.name == "Killer2" || other.gameObject.name == "Killer3" || other.gameObject.name == "Killer4") {
						GameManager.singleton.score++;
						gameObject.transform.position = Vector3.zero;
						for (int i = 0; i < this.GetComponent<SpherePhysics>().players.Count; i++) {
							int j = i + 1;
							this.GetComponent<SpherePhysics>().players [i].gameObject.transform.position = GameObject.Find ("StartPosition" + j).transform.position;
						}
					}
				}
			}
		}*/
	}

	[ServerCallback]
	void OnTriggerStay2D (Collider2D other)
	{
		if (other.gameObject.GetComponent<IInteractable> () != null)
			other.gameObject.GetComponent<IInteractable> ().OnStayEffect (this);
		/*
		if (other.gameObject.name == "pointFollow" || other.gameObject.name == "LabyrinthEnding1" || other.gameObject.name == "LabyrinthEnding2") {
			GameManager.singleton.score++;
		}
		if (other.gameObject.name == "CrossLava" || other.gameObject.name == "Killer1" || other.gameObject.name == "Killer2" || other.gameObject.name == "Killer3" || other.gameObject.name == "Killer4") {
			GameManager.singleton.score--;
		}
		*/
	}

	[ServerCallback]
	void OnTriggerExit2D (Collider2D other)
	{
		if (other.gameObject.GetComponent<IInteractable> () != null)
			other.gameObject.GetComponent<IInteractable> ().OnExitEffect (this);
	}

	void Start()
	{
	}
}
