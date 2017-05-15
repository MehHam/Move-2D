using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanelUI : MonoBehaviour {
	enum MainPanelState {
		Default = 0,
		MatchMaker = 1,
		DirectPlay = 2,
	}

	public GameObject directPlayBody;
	public GameObject matchMakerBody;

	Vector3 _directPlayAnchoredPosition;
	Vector2 _directPlaySizeDelta;
	Vector3 _matchMakerAnchoredPosition;
	Vector2 _matchMakerSizeDelta;

	void Awake()
	{
		_directPlayAnchoredPosition = directPlayBody.GetComponent<RectTransform> ().anchoredPosition;
		_directPlaySizeDelta = directPlayBody.GetComponent<RectTransform> ().sizeDelta;
		_matchMakerAnchoredPosition = matchMakerBody.GetComponent<RectTransform> ().anchoredPosition;
		_matchMakerSizeDelta = matchMakerBody.GetComponent<RectTransform> ().sizeDelta;
	}

	public void OnMatchMakerClick()
	{
		this.GetComponent<Animator>().SetInteger ("State", this.GetComponent<Animator>().GetInteger("State") == (int)MainPanelState.MatchMaker
			? (int)MainPanelState.Default
			: (int)MainPanelState.MatchMaker);
	}

	public void OnDirectPlayClick()
	{
		this.GetComponent<Animator>().SetInteger ("State", this.GetComponent<Animator>().GetInteger("State") == (int)MainPanelState.DirectPlay
			? (int)MainPanelState.Default
			: (int)MainPanelState.DirectPlay);
	}

	void OnDisable()
	{
		directPlayBody.GetComponent<RectTransform> ().anchoredPosition = _directPlayAnchoredPosition;
		directPlayBody.GetComponent<RectTransform> ().sizeDelta = _directPlaySizeDelta;
		directPlayBody.GetComponentInChildren<CanvasGroup> ().alpha = 0;
		directPlayBody.GetComponentInChildren<CanvasGroup> ().interactable = false;
		directPlayBody.GetComponentInChildren<CanvasGroup> ().blocksRaycasts = false;
		matchMakerBody.GetComponent<RectTransform> ().anchoredPosition = _matchMakerAnchoredPosition;
		matchMakerBody.GetComponent<RectTransform> ().sizeDelta = _matchMakerSizeDelta;
		matchMakerBody.GetComponentInChildren<CanvasGroup> ().alpha = 0;
		matchMakerBody.GetComponentInChildren<CanvasGroup> ().interactable = false;
		matchMakerBody.GetComponentInChildren<CanvasGroup> ().blocksRaycasts = false;
	}
}
