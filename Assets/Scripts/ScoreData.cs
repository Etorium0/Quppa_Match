using System;

[Serializable]
public class ScoreData
{
	public string dateTime
	{
		get;
		set;
	}

	public int score
	{
		get;
		set;
	}

	public int number
	{
		get;
		set;
	}

	public ScoreData(string date, int score, int number)
	{
		dateTime = date;
		this.score = score;
		this.number = number;
	}
}
