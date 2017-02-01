using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

// This script allows you to drag this GameObject using any finger, as long it has a collider
public class DragSphere : NetworkBehaviour
{
	// This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)
	public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;
	private Rigidbody2D rb;
	private Transform transf;
	private int counterFing;
	private Vector2 frameDelta=Vector2.zero;
	// This stores the finger that's currently dragging this GameObject
	private Lean.LeanFinger draggingFinger;
	public static bool isTouch=false;
	private Slider slider;

	void Start(){
		rb=gameObject.GetComponent<Rigidbody2D>();
		transf=rb.transform;
	}


	void FixedUpdate()
	{

		rb=gameObject.GetComponent<Rigidbody2D>();
		transf=rb.transform;

		slider=GameObject.Find ("Slider").GetComponent<Slider>();
		if(slider.value==1){
			rb.constraints=RigidbodyConstraints2D.None;
		// If there is an active finger, move this GameObject based on it
		if (draggingFinger != null && counterFing<10)
		{

			isTouch=true;
			if (!isLocalPlayer)
				return;
				
			//CmdMotion();
			frameDelta=draggingFinger.DeltaScreenPosition;
			CmdMotion(frameDelta);

			counterFing++;
			frameDelta=Vector2.zero;
		}else{
			counterFing=0;
			isTouch=false;
			
		}
		}else{
			isTouch=false;
		
			rb.constraints=RigidbodyConstraints2D.FreezeRotation;
			//rb.freezeRotation();
		}
	}

	public void CmdMotion(Vector2 deltaPosition)
	{
		Camera cam;
		cam = Camera.main;
		//Vector3 nouvVelocity;
		Vector3 herePosition;
		var screenPosition = cam.WorldToScreenPoint(transf.position);
		// Modify screen position
		screenPosition += (Vector3)deltaPosition;

		// Write new world position
		herePosition = cam.ScreenToWorldPoint(screenPosition);
		rb.MovePosition(herePosition);

		//nouvVelocity =0.07f*(cam.ScreenToWorldPoint(screenPosition)-(Vector3)physics.bufferPositionPlayer1[physics.bufferPositionPlayer1.Count-2])*Time.fixedTime;
		//rb.AddForce(herePosition*Time.deltaTime*0.7f);


	}
	protected virtual void OnEnable()
	{
		// Hook into the OnFingerDown event
		Lean.LeanTouch.OnFingerDown += OnFingerDown;
		// Hook into the OnFingerUp event
		Lean.LeanTouch.OnFingerUp += OnFingerUp;
	}

	protected virtual void OnDisable()
	{

		// Unhook the OnFingerDown event
		Lean.LeanTouch.OnFingerDown -= OnFingerDown;

		// Unhook the OnFingerUp event
		Lean.LeanTouch.OnFingerUp -= OnFingerUp;
	}
	public void OnFingerDown(Lean.LeanFinger finger)
	{
		// Raycast information
		var ray = finger.GetRay();
		//var hit = default(RaycastHit);

		Vector2 rayPos = finger.ScreenPosition;
		RaycastHit2D[] hit = Physics2D.RaycastAll(ray.origin,ray.direction,0f);
	
		// Was that collider this one?
			if (hit.Length>0)
			{
			rb=gameObject.GetComponent<Rigidbody2D>();
			transf=rb.transform;

				// Set the current finger to this one
				draggingFinger = finger;
			}

		/*// Was this finger pressed down on a collider?
		if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true)
		{
			// Was that collider this one?
			if (hit.collider.gameObject == gameObject)
			{
				// Set the current finger to this one
				draggingFinger = finger;
			}
		}*/
	}

	public void OnFingerUp(Lean.LeanFinger finger)
	{
		// Was the current finger lifted from the screen?
		if (finger == draggingFinger)
		{

			// Unset the current finger
			draggingFinger = null;
		}
	}





	}