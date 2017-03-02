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

	public float speed = 50.0f;

	Vector3 _startPosition;
	Vector3 _offset;
	float _zDistanceToCamera;

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
		this.GetComponent<Rigidbody2D> ().MovePosition (
			Vector3.MoveTowards(
				this.GetComponent<Rigidbody2D>().position,
				Camera.main.ScreenToWorldPoint (
				GetDragPosition (this._zDistanceToCamera)
				) + _offset,
				speed * Time.deltaTime
			)
		);
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