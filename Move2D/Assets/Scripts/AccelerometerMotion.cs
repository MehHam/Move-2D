using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class AccelerometerMotion : NetworkBehaviour
{


	//[SyncVar] public Transform playerUniqueTransform;
	const float gyroCte = 0.5f;
	const int accCte = 50;

	Rigidbody2D rb;
	private Vector2 accel=new Vector2(0,0);
	private Vector2 gyROT=new Vector2(0,0);
	public Gyroscope gyro;
	private Slider slider;
	private Transform transf;


	void Awake(){

		rb=GetComponent<Rigidbody2D>();

	}

	void FixedUpdate()
	{	
		if(levelDesign.levelValue!=0 && levelDesign.levelValue!=4 && levelDesign.levelValue!=5 )
		{
		slider=GameObject.Find ("Slider").GetComponent<Slider>();
		rb=gameObject.GetComponent<Rigidbody2D>();
		transf=rb.transform;
		//
		//if (!isLocalPlayer)
		//	return;
		if(slider.value==3){

		accMotion();
		}else{
			if(slider.value==2){
				if (!isLocalPlayer)
					return;
				gyroMotion();

			}

		}
		}

	}

	public void gyroMotion()
	{

		
		if(SystemInfo.supportsGyroscope)
		{
			Debug.Log("Flat gyro");
			gyro=Input.gyro;
			gyro.enabled=true;

			float h=gyro.rotationRateUnbiased.z;

			//gyROT=new Vector2(-gyro.attitude.y*gyro.attitude.y*gyroCte*Time.deltaTime*10.0f* Mathf.Cos (gyro.attitude.y*Time.deltaTime),-gyro.attitude.y*gyro.attitude.y*gyroCte*Time.deltaTime*10.0f* Mathf.Sin (gyro.attitude.y*Time.deltaTime));
			//gyROT=new Vector2(gyro.rotationRateUnbiased.z*gyroCte*Time.deltaTime,gyro.rotationRateUnbiased.z*gyroCte*Time.deltaTime);
			//gyROT=new Vector3(Input.acceleration.x*gyroCte*Time.deltaTime,0,Input.acceleration.y*gyroCte*Time.deltaTime);
			//gyROT=new Vector2(-gyro.rotationRateUnbiased.x*gyroCte*Time.deltaTime,gyro.gravity.y*gyroCte*Time.deltaTime);
			//gyROT=new Vector2(gyroCte*Time.deltaTime*Mathf.Cos(gyro.rotationRateUnbiased.z),gyroCte*Time.deltaTime*Mathf.Sin(gyro.rotationRateUnbiased.z));
			//gyROT=new Vector2(gyro.gravity.x*Time.deltaTime*gyroCte,gyro.gravity.y*Time.deltaTime*gyroCte);

			//gyROT=new Vector2(-gyro.gravity.x*gyroCte,gyro.gravity.y*gyroCte);
			//
			//gyROT=new Vector2(1f*Mathf.Cos(Mathf.Atan(gyro.gravity.y/gyro.gravity.x))*Time.deltaTime*gyroCte,1f*Mathf.Sin(Mathf.Atan(gyro.gravity.y/gyro.gravity.x))*Time.deltaTime*gyroCte);
			float radiusR=15.7f;
			/*float theta =Mathf.Acos(gyro.gravity.y/gyro.gravity.magnitude);
			float alpha=2*theta+Mathf.PI;
			Vector2 playerPos= new Vector2(radiusR*Mathf.Cos(alpha),radiusR*Mathf.Sin(alpha)); //TODO change the angle expression to take into account the sign of cos(alpha) and cos(-alpha)
			*/
			float alpha=0f;
			float theta =Mathf.Atan(gyro.gravity.x/gyro.gravity.y);

			if(theta< Mathf.PI/2 && theta> -Mathf.PI/2){
			alpha=2*theta;
			}
			Vector2 playerPos= new Vector2(radiusR*Mathf.Cos(alpha),radiusR*Mathf.Sin(alpha));
			if(playerPos.magnitude>radiusR*0.95f && playerPos.magnitude<radiusR*1.05f){
			rb.transform.position=Vector2.Lerp(rb.transform.position,playerPos,0.5f);
				//rb.transform.position=playerPos;
			}
			//rb.AddTorque(h,ForceMode2D.Force);
			//rb.transform.rotation=new Quaternion(accel.y*3*accCte*Time.deltaTime,accel.z*3*accCte*Time.deltaTime,accel.x*3*accCte*Time.deltaTime,0.0f);
			//rb.AddForce(new Vector2(10*Mathf.Cos(gyro.rotationRateUnbiased.z),10*Mathf.Sin(gyro.rotationRateUnbiased.z)),ForceMode2D.Impulse);
			//rb.AddForce(gyROT,ForceMode2D.Impulse);
			//rb.AddForce(gyROT,ForceMode2D.Impulse);
			//Debug.Log (Mathf.Acos(gyro.gravity.y/gyro.gravity.magnitude));

		}else{
		Debug.Log("gyro not supported");

	}
	}

	public void accMotion()
	{
		if(SystemInfo.supportsGyroscope)
		{
			Debug.Log("Ball Player gyro");
			gyro=Input.gyro;
			gyro.enabled=true;
			
			accel=new Vector3(Input.acceleration.x,0,Input.acceleration.y);
			//gyROT=new Vector2(10*Time.deltaTime*Mathf.Cos(gyro.rotationRateUnbiased.z),10*Time.deltaTime*Mathf.Sin(gyro.rotationRateUnbiased.z));
			//gyROT=new Vector2(gyro.gravity.x*Time.deltaTime*gyroCte,gyro.gravity.y*Time.deltaTime*gyroCte);
			
			//gyROT=new Vector2(gyro.gravity.x*Time.deltaTime*gyroCte,-gyro.gravity.y*Time.deltaTime*gyroCte);
			//gyROT=new Vector2(Mathf.Cos(Mathf.Atan(gyro.gravity.y/-gyro.gravity.x))*accCte,Mathf.Sin(Mathf.Atan(gyro.gravity.y/-gyro.gravity.x))*accCte);
			
			//rb.transform.position+=accel*accCte*Time.deltaTime;
			//rb.transform.rotation=new Quaternion(accel.y*3*accCte*Time.deltaTime,accel.z*3*accCte*Time.deltaTime,accel.x*3*accCte*Time.deltaTime,0.0f);
			//rb.AddForce(new Vector2(10*Mathf.Cos(gyro.rotationRateUnbiased.z),10*Mathf.Sin(gyro.rotationRateUnbiased.z)),ForceMode2D.Impulse);
			//rb.AddForce(gyROT,ForceMode2D.Impulse);
			rb.AddForce(gyROT,ForceMode2D.Force);
			
		}else{
			Debug.Log("gyro not supported");
			
		}

	}



	
}
