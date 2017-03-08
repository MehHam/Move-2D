using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

/// <summary>
/// A movement module manages the drag type of movement for the player
/// </summary>
public class PlayerDragMove : NetworkBehaviour, IPlayerMotion, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public static GameObject DraggedInstance;

	public float speed = 50.0f;

	Vector3 _startPosition;
	Vector3 _offset;
	float _zDistanceToCamera;
	float _moveHorizontal;

	public override void OnStartLocalPlayer ()
	{
		var dir = Vector2.zero - (Vector2)this.transform.position;
		float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
		_moveHorizontal = angle;
	}

	Vector3 GetDragPosition(float zDistanceToCamera)
	{
		return new Vector3 (Input.mousePosition.x, Input.mousePosition.y, zDistanceToCamera);
	}

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		if (!isLocalPlayer)
			return;
		DraggedInstance = this.gameObject;
		_startPosition = this.transform.position;
		_zDistanceToCamera = Mathf.Abs (_startPosition.z - Camera.main.transform.position.z);
		_offset = _startPosition - Camera.main.ScreenToWorldPoint (GetDragPosition (this._zDistanceToCamera));
		this.GetComponent<PlayerMoveManager>().enabl
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		if (!isLocalPlayer)
			return;
		var dragPosition = Camera.main.ScreenToWorldPoint (GetDragPosition (this._zDistanceToCamera)) + _offset;
		_moveHorizontal -= Input.GetAxis("Mouse X") * speed * 0.02f;

		Quaternion rotation = Quaternion.Euler(_moveHorizontal, 270.0f, 0.0f);
		var position = rotation * new Vector3 (0.0f, 0.0f, 16.0f);

		this.transform.position = position;
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