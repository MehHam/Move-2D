using UnityEngine;
using System.Collections;
/// <summary>
/// Set the variables sortingLayerName and sortingOrder for a mesh renderer
/// </summary>
public class RendererSorter : MonoBehaviour {
	public string sortingLayerName = "Default";
	public int sortingOrder = 0;

	void Start () {
		this.gameObject.GetComponent<Renderer> ().sortingLayerName = sortingLayerName;
		this.gameObject.GetComponent<Renderer> ().sortingOrder = sortingOrder;
	}
}