using UnityEngine;
using System.Collections;

public class RotationSphere : MonoBehaviour {
	private GameObject SphereRot;
	private Rigidbody rb;
	public Vector3 origineRot;

	private GameObject floorRot1;
	private Rigidbody rb1;
	public Vector3 origineRot1;

	private GameObject floorRot2;
	private Rigidbody rb2;
	public Vector3 origineRot2;
	
	// Use this for initialization
	void Start () {
		SphereRot=GameObject.Find("SphereControlled");
		SphereRot.transform.position=new Vector3(247.0f,29.0f,3.7f);
		rb=SphereRot.GetComponent<Rigidbody>();
		origineRot=new Vector3(300.0f,28.8f,0.72f);

		floorRot1=GameObject.Find("Player 1Floor");
		rb1=floorRot1.GetComponent<Rigidbody>();
		origineRot1=rb1.transform.position;

		floorRot2=GameObject.Find("Player 2Floor");
		rb2=floorRot2.GetComponent<Rigidbody>();
		origineRot2=rb2.transform.position;

	}
	
	// Update is called once per frame
	void Update () {
		//SphereRot.transform.position=new Vector3(246.0f,29.0f,3.7f);
		//RotatingRigidBody(rb, rb.transform.position ,axeRot, 1.0f);
		RotatingRigidBody(rb,origineRot,Vector3.up,Time.deltaTime*5.0f);
		//RotatingRigidBody(rb1,origineRot1,Vector3.up,-Time.deltaTime*2.0f);
		//RotatingRigidBody(rb2,origineRot2,Vector3.up,Time.deltaTime*5.0f);
	}

	public void RotatingRigidBody(Rigidbody rb, Vector3 origin,Vector3 axis, float angle)
	{

		//Quaternion q = Quaternion.AngleAxis(angle,axis);
		rb.transform.RotateAround(origin,axis,angle);
		//rb.MovePosition(q*(rb.transform.position-origin)+origin);
		//rb.MoveRotation(q*rb.transform.rotation);
	}

}
