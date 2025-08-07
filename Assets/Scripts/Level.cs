using System;
using UnityEngine;

[Serializable]
public class Level
{
	[HideInInspector]
	public int MissionID;

	[HideInInspector]
	public int ID;

	[HideInInspector]
	public bool isClear;

	[HideInInspector]
	public bool isLocked;

	[Range(4f, 10f)]
	public int height;

	[Range(4f, 20f)]
	public int width;

	[HideInInspector]
	public MoveMode moveMode;
}
