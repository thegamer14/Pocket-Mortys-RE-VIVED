using System;

[Flags]
public enum eSaveDataType
{
	NONE = 0,
	ALL = 2,
	SETTINGS = 4,
	PLAYER = 8,
	SEGMENTED_WORLD = 0x10,
	TOURNAMENT = 0x20,
	MULTIPLAYER = 0x40
}
