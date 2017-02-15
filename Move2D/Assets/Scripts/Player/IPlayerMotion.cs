using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public interface IPlayerMotion
{
	void Move();
	bool IsActivated (int sliderValue);
}

