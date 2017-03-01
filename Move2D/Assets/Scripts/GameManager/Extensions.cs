using System;

public static class Extensions
{
	public static string ToString(this GameManager.Difficulty difficulty)
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

	public static string ToStringColor(this GameManager.Difficulty difficulty)
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
