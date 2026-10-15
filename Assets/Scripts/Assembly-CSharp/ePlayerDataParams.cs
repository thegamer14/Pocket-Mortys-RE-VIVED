using System;

[Flags]
public enum ePlayerDataParams
{
	AVATARS = 1,
	CURRENCY = 2,
	MORTIES = 4,
	MORTY_ATTACKS = 8,
	DECKS = 0x10,
	MORTYDEX = 0x20,
	ITEMS = 0x40,
	RECIPES = 0x80,
	CHALLENGE = 0x100,
	ALL = 0x1FF,
	POST_BATTLE = 0x5E,
	DONATED_MORTY = 0x1C
}
