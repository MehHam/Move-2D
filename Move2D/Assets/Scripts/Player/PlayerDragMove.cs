using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

/// <summary>
/// A movement module manages the drag type of movement for the player
/// </summary>
[RequireComponent (typeof (Rigidbody2D))]
public class PlayerDragMove : NetworkBehaviour, IPlayerMotion, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public static GameObject DraggedInstance;

	Vector3 _startPosition;
	Vector3 _offset;
	float _zDistanceToCamera;


	/*
	/// <summary>
	/// This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)
	/// </summary>
	[Tooltip("This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)")]
	public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;
	private int counterFing;
	private Vector2 frameDelta = Vector2.zero;
	// This stores the finger that's currently dragging this GameObject
	private Lean.LeanFinger _draggingFinger;
	public static bool isTouch = false;
	public int sliderValue = 0;

	public bool IsActivated (int sliderValue)
	{
		return (this.sliderValue == sliderValue);
	}
		
	public void Move ()
	{
		// Only the local player should call this method
		if (!isLocalPlayer)
			return;
		
		this.GetComponent<Rigidbody2D>().constraints= RigidbodyConstraints2D.None;
		// If there is an active finger, move this GameObject based on it
		if (_draggingFinger != null && counterFing < 10) {

			isTouch = true;

			//CmdMotion();
			frameDelta = _draggingFinger.DeltaScreenPosition;
			CmdMotion (frameDelta);

			counterFing++;
			frameDelta = Vector2.zero;
		} else {
			counterFing = 0;
			isTouch = false;
		}
	}

	void CmdMotion (Vector2 deltaPosition)
	{
		Camera cam;
		cam = Camera.main;
		//Vector3 nouvVelocity;
		Vector3 herePosition;
		var screenPosition = cam.WorldToScreenPoint (this.GetComponent<Rigidbody2D>().transform.position);
		// Modify screen position
		screenPosition += (Vector3)deltaPosition;

		// Write new world position
		herePosition = cam.ScreenToWorldPoint (screenPosition);
		this.GetComponent<Rigidbody2D>().MovePosition (herePosition);

		//nouvVelocity =0.07f*(cam.ScreenToWorldPoint(screenPosition)-(Vector3)physics.bufferPositionPlayer1[physics.bufferPositionPlayer1.Count-2])*Time.fixedTime;
		//rb.AddForce(herePosition*Time.deltaTime*0.7f);
	}

	protected virtual void OnEnable ()
	{
		// Hook into the OnFingerDown event
		Lean.LeanTouch.OnFingerDown += OnFingerDown;
		// Hook into the OnFingerUp event
		Lean.LeanTouch.OnFingerUp += OnFingerUp;
	}

	protected virtual void OnDisable ()
	{
		// Unhook the OnFingerDown event
		Lean.LeanTouch.OnFingerDown -= OnFingerDown;

		// Unhook the OnFingerUp event
		Lean.LeanTouch.OnFingerUp -= OnFingerUp;

		isTouch = false;
		this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
	}

	void OnFingerDown (Lean.LeanFinger finger)
	{
		// Raycast information
		var ray = finger.GetRay ();
		//var hit = default(RaycastHit);

		//Vector2 rayPos = finger.ScreenPosition;
		RaycastHit2D[] hit = Physics2D.RaycastAll (ray.origin, ray.direction, 0f);
	
		// Was that collider this one?
		if (hit.Length > 0) {
			// Set the current finger to this one
			_draggingFinger = finger;
		}

		// Was this finger pressed down on a collider?
		if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true)
		{
			// Was that collider this one?
			if (hit.collider.gameObject == gameObject)
			{
				// Set the current finger to this one
				draggingFinger = finger;
			}
		}
	}

	void OnFingerUp (Lean.LeanFinger finger)
	{
		// Was the current finger lifted from the screen?
		if (finger == _draggingFinger) {

			// Unset the current finger
			_draggingFinger = null;
		}
	}
*/

	Vector3 GetDragPosition(float zDistanceToCamera)
	{
		return new Vector3 (Input.mousePosition.x, Input.mousePosition.y, zDistanceToCamera);
	}

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		Debug.Log ("OnBeginDragCalled");
		if (!isLocalPlayer)
			return;
		Debug.Log ("IsLocalPLayer");
		DraggedInstance = this.gameObject;
		_startPosition = this.GetComponent<Rigidbody2D> ().position;
		_zDistanceToCamera = Mathf.Abs (_startPosition.z - Camera.main.transform.position.z);
		_offset = _startPosition - Camera.main.ScreenToWorldPoint (GetDragPosition (this._zDistanceToCamera));
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		Debug.Log ("OnDragCalled");
		if (!isLocalPlayer)
			return;
		Debug.Log ("IsLocalPlayer");
		this.GetComponent<Rigidbody2D> ().MovePosition (Camera.main.ScreenToWorldPoint (
				GetDragPosition (this._zDistanceToCamera)
			) + _offset);
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		if (!isLocalPlayer)
			return;
		DraggedInstance = null;
		_offset = Vector3.zero;
	}

	#endregion

	#region IPlayerMotion implementation

	public void Move ()
	{
		
	}

	public bool IsActivated (int sliderValue)
	{
		return true;
	}

	#endregion
}