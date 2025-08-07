using System;
using System.Collections.Generic;

[Serializable]
public class Mission
{
	public string missionName;

	public int ID;

	public bool isLocked;

	public bool isClear;

	public List<Level> levels;

	public Mission()
	{
		levels = new List<Level>();
	}
}
