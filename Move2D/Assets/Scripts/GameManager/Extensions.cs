using System;
using UnityEngine;

namespace Move2D
{
	public static class Extensions
	{
		public static string ToString (this GameManager.Difficulty difficulty)
		{
			string res = "";

			switch (difficulty) {
			case GameManager.Difficulty.Beginner:
				res = "Beginner";
				break;
			case GameManager.Difficulty.Intermediate:
				res = "Intermediate";
				break;
			case GameManager.Difficulty.Expert:
				res = "Expert";
				break;
			}
			return res;
		}

		public static string ToStringColor (this GameManager.Difficulty difficulty)
		{
			string res = "";

			switch (difficulty) {
			case GameManager.Difficulty.Beginner:
				res = "<color=#b8ffa0>Beginner</color>";
				break;
			case GameManager.Difficulty.Intermediate:
				res = "<color=#a0fffd>Intermediate</color>";
				break;
			case GameManager.Difficulty.Expert:
				res = "<color=#ffa0a0>Expert</color>";
				break;
			}
			return res;
		}
	}

	public static class GameObjectExtension
	{
		public static T AddComponentWithInit<T> (this GameObject obj, System.Action<T> onInit) where T : Component
		{
			bool oldState = obj.activeSelf;
			obj.SetActive (false);
			T comp = obj.AddComponent<T> ();
			if (onInit != null)
				onInit (comp);
			obj.SetActive (oldState);
			return comp;
		}
	}
}