using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SphereCDM : NetworkBehaviour
{
	//OnTriggerEnter2D is called whenever this object overlaps with a trigger collider.
	[ServerCallback]
	void OnTriggerEnter2D (Collider2D other)
	{
		//Check the provided Collider2D parameter other to see if it is tagged "PickUp", if it is...
		if (other.gameObject.CompareTag ("PickUp")) {
			//... then set the other object we just collided with to inactive.
			other.gameObject.SetActive (false);
			//Add one to the current value of our count variable.
			GameManager.singleton.score++;
		}
		//Update the currently displayed count by calling the SetCountText function.
		//SetCountText ();

		if (other.gameObject.name == "PetitChartres" || other.gameObject.name == "CrossLava")
			GameManager.singleton.score--;
		if (other.gameObject.name == "pointFollow")
			GameManager.singleton.score++;
		else {
			if (other.gameObject.name == "massEffect" && levelDesign.levelValue == 3) {
				//gameObject.transform.position;
				if (physics.playerLimit >= 1) {
					for (int i = 0; i < physics.players.Count; i++) {
						int j = i + 1;
						physics.players [i].go.transform.position = GameObject.Find ("masseffect" + j).transform.position;
						physics.players [i].mass = (i * 4) + 1;
					}
				}
			}

			if (other.gameObject.name == "Teleporter1" && levelDesign.levelValue == 3) {

				gameObject.transform.position = GameObject.Find ("Teleporter3").transform.position;
				if (physics.playerLimit >= 1) {
					for (int i = 0; i < physics.players.Count; i++) {
						int j = i + 1;
						physics.players [i].go.transform.position = GameObject.Find ("Telep" + j).transform.position;
					}
				}
			}

			if (other.gameObject.name == "Teleporter2" && levelDesign.levelValue == 3) {
					
				gameObject.transform.position = GameObject.Find ("Teleporter4").transform.position;
				if (physics.playerLimit >= 1) {
					for (int i = 0; i < physics.players.Count; i++) {
						int j = i + 3;
						physics.players [i].go.transform.position = GameObject.Find ("Telep" + j).transform.position;
					}
				}
			}


			if (other.gameObject.name == "LabyrinthEnding1" || other.gameObject.name == "LabyrinthEnding2" && levelDesign.levelValue == 3) {
				GameManager.singleton.score++;
			} else {
				if (other.gameObject.name == "PetitChartres" || other.gameObject.name == "CrossLava") {
					GameManager.singleton.score--;
				} else {

					if (other.gameObject.name == "Killer1" || other.gameObject.name == "Killer2" || other.gameObject.name == "Killer3" || other.gameObject.name == "Killer4" && levelDesign.levelValue == 3) {
						GameManager.singleton.score++;
						gameObject.transform.position = Vector3.zero;
						if (physics.playerLimit >= 1) {
							for (int i = 0; i < physics.players.Count; i++) {
								int j = i + 1;
								physics.players [i].go.transform.position = GameObject.Find ("StartPosition" + j).transform.position;
							}
						}
					}
				}
			}
		}
	}

	[ServerCallback]
	void OnTriggerStay2D (Collider2D other)
	{
		if (other.gameObject.name == "pointFollow" || other.gameObject.name == "LabyrinthEnding1" || other.gameObject.name == "LabyrinthEnding2") {
			GameManager.singleton.score++;
		}
		if (other.gameObject.name == "CrossLava" || other.gameObject.name == "Killer1" || other.gameObject.name == "Killer2" || other.gameObject.name == "Killer3" || other.gameObject.name == "Killer4") {
			GameManager.singleton.score--;
		}

	}


}
