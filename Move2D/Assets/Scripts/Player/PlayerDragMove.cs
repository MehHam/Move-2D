using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

namespace Move2D
{
	/// <summary>
	/// A movement module manages the drag type of movement for the player
	/// </summary>
	public class PlayerDragMove : MonoBehaviour, IPlayerMotion, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public static GameObject DraggedInstance;

		public float speed = 50.0f;

		Vector3 _startPosition;
		float _zDistanceToCamera;
		bool _isMoving = false;

		Vector3 GetDragPosition (float zDistanceToCamera)
		{
			return new Vector3 (Input.mousePosition.x, Input.mousePosition.y, zDistanceToCamera);
		}

		#region IBeginDragHandler implementation

		public void OnBeginDrag (PointerEventData eventData)
		{
			if (!GameManager.singleton.isPlaying)
				return;
			this._startPosition = this.transform.position;
			_zDistanceToCamera = Mathf.Abs (_startPosition.z - Camera.main.transform.position.z);
			_isMoving = true;
		}

		#endregion

		#region IDragHandler implementation

		public void OnDrag (PointerEventData eventData)
		{
			if (!GameManager.singleton.isPlaying)
				return;
			var diff = (Camera.main.ScreenToWorldPoint (GetDragPosition (this._zDistanceToCamera))) - transform.position;
			var direction = diff / diff.magnitude;

			float rot_z = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Lerp (this.transform.rotation, Quaternion.Euler (0f, 0f, rot_z - 90.0f), speed * Time.deltaTime);
		}

		#endregion

		#region IEndDragHandler implementation

		public void OnEndDrag (PointerEventData eventData)
		{
			if (!GameManager.singleton.isPlaying)
				return;
			_isMoving = false;
		}

		#endregion

		#region IPlayerMotion implementation

		public bool Move ()
		{
			return _isMoving;
		}

		public bool IsActivated (int sliderValue)
		{
			return true;
		}

		#endregion
	}
}