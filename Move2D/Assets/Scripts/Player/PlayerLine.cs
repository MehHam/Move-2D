using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Move2D
{
	[RequireComponent (typeof(LineRenderer))]
	public class PlayerLine : MonoBehaviour
	{
		/// <summary>
		/// The first player object
		/// </summary>
		[Tooltip ("The first player object")]
		public GameObject object1;
		/// <summary>
		/// The second player object
		/// </summary>
		[Tooltip ("The second player object")]
		public GameObject object2;
		/// <summary>
		/// The alpha of the line
		/// </summary>
		[Tooltip ("The alpha of the line")]
		public float alpha;
		/// <summary>
		/// The number of points in the line
		/// </summary>
		public int numberOfPoints;

		private float _currentAlpha;

		public SpherePhysics spherePhysics;

		LineRenderer _lineRenderer;

		float _randomOffset;
		int _randomDirection;

		void Start ()
		{
			_lineRenderer = GetComponent<LineRenderer> ();
			_randomOffset = Random.Range (0.0f, 5.0f);
			_randomDirection = Random.Range (0.0f, 1.0f) > 0.5f ? -1 : 1;
		}

		void SetColors ()
		{
			var startColor = object1.GetComponent<ILineObject> ().GetColor ();
			var endColor = object2.GetComponent<ILineObject> ().GetColor ();
			var startMass = object1.GetComponent<ILineObject> ().GetMass ();
			var endMass = object2.GetComponent<ILineObject> ().GetMass ();
			var colorGradient = new Gradient ();
			var widthCurve = new AnimationCurve ();
			var gradientColorKeys = new GradientColorKey[2];
			var gradientAlphaKeys = new GradientAlphaKey[2];

			gradientColorKeys [0].color = startColor;
			gradientColorKeys [0].time = 0.0f;
			gradientColorKeys [1].color = endColor;
			gradientColorKeys [1].time = 1.0f;

			gradientAlphaKeys [0].alpha = Mathf.Min (startColor.a, _currentAlpha);
			gradientAlphaKeys [0].time = 0.0f;
			gradientAlphaKeys [1].alpha = Mathf.Min (endColor.a, _currentAlpha);
			gradientAlphaKeys [1].time = 1.0f;

			colorGradient.SetKeys (gradientColorKeys, gradientAlphaKeys);

			widthCurve.AddKey (new Keyframe (0.0f, startMass));
			widthCurve.AddKey (new Keyframe (1.0f, endMass));

			_lineRenderer.colorGradient = colorGradient;
			_lineRenderer.widthCurve = widthCurve;
			_lineRenderer.material.mainTextureOffset = new Vector2 (_randomOffset + _randomDirection * Time.time * 3, 0.0f);
		}

		void SetPositions ()
		{
			if (numberOfPoints >= 2) {
				var positions = new Vector3[numberOfPoints];
				var startPosition = object1.transform.position;
				var endPosition = object2.transform.position;
				var heading = (endPosition - startPosition);

				for (int i = 0; i < numberOfPoints; i++) {
					positions [i] = ((float)i / (float)(numberOfPoints - 1)) * heading + startPosition;
				}
				_lineRenderer.numPositions = positions.Length;
				_lineRenderer.SetPositions (positions);
			}
		}

		void Update ()
		{
			if (spherePhysics.hasMoved || Input.GetKey(KeyCode.Tab))
				_currentAlpha = Mathf.Min (_currentAlpha + 0.05f, alpha);
			else
				_currentAlpha = Mathf.Max (_currentAlpha - 0.05f, 0.0f);
			SetColors ();
			SetPositions ();
		}
	}
}