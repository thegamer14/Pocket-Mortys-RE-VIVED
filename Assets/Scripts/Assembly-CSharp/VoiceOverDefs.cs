using UnityEngine;

public class VoiceOverDefs
{
	public static AudioSource Play(string resource, float volume, float pan = 0f, float delay = 0f)
	{
		return App.sound.PlaySFX(resource, volume, false, 1f, 0f, delay, true, null, pan);
	}

	public static AudioSource PlayRandom(string resource, float volume, float pan = 0f, float delay = 0f)
	{
		return Play(string.Format(resource, GetRandomVOForResource(resource)), volume, pan, delay);
	}

	public static AudioSource PlayAvatarRandom(string resource, string avatarCategory, float volume, float pan = 0f, float delay = 0f)
	{
		if (string.IsNullOrEmpty(avatarCategory))
		{
			App.sound.DebugLog("VO - PlayAvatarRandom: No category supplied", true);
			return null;
		}
		return Play(string.Format(resource, avatarCategory, GetRandomVOForResource(resource)), volume, pan, delay);
	}

	private static int GetRandomVOForResource(string resource)
	{
		switch (resource)
		{
		default:
			return 0;
		case "Audio/VO/Morty - Hurt/{0}":
			return Random.Range(0, 19);
		case "Audio/VO/Morty - Moan/{0}":
			return Random.Range(0, 12);
		case "Audio/VO/Morty - Buff/{0}":
			return Random.Range(0, 2);
		case "Audio/VO/Morty - Debuff/{0}":
			return Random.Range(0, 5);
		case "Audio/VO/Morty - Worry/{0}":
			return Random.Range(0, 8);
		case "Audio/VO/Trainer - Send Morty Out/{0}/{1}":
			return Random.Range(0, 21);
		case "Audio/VO/Morty - Dazed/{0}":
			return Random.Range(0, 9);
		case "Audio/VO/Trainer - Complement/{0}/{1}":
			return Random.Range(0, 20);
		case "Audio/VO/Trainer - Morty Put Down/{0}/{1}":
			return Random.Range(0, 19);
		case "Audio/VO/Trainer - Lose Trainer/{0}/{1}":
			return Random.Range(0, 14);
		case "Audio/VO/Trainer - Win/{0}/{1}":
			return Random.Range(0, 17);
		case "Audio/VO/Trainer - Win Against Rick/{0}/{1}":
			return Random.Range(0, 3);
		case "Audio/VO/Trainer - Happy/{0}/{1}":
			return Random.Range(0, 20);
		case "Audio/VO/Trainer - Moan/{0}/{1}":
			return Random.Range(0, 10);
		case "Audio/VO/Trainer - Engage/{0}/{1}":
			return Random.Range(0, 12);
		case "Audio/VO/Trainer - Engage Against Rick/{0}/{1}":
			return Random.Range(0, 11);
		case "Audio/VO/Trainer - Engage Against Rick Dimension/{0}/{1}":
			return Random.Range(0, 5);
		case "Audio/VO/Trainer - Random/{0}/{1}":
			return Random.Range(0, 10);
		case "Audio/VO/Trainer - Random In Segmented/{0}/{1}":
			return Random.Range(0, 2);
		case "Audio/VO/Trainer - Blackout/{0}/{1}":
			return Random.Range(0, 4);
		case "Audio/VO/Birdperson Save/{0}":
			return Random.Range(0, 3);
		}
	}
}
