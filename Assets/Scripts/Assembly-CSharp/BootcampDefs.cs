using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BootcampDefs
{
	public delegate void BootcampDefsBootcampStateChangedDelgate(int bootcampIndex, eBootcampState state);

	public const int kMaxMortiesInBootcamp = 5;

	public const int kMaxBootcamps = 3;

	private static List<long> _bootcampDurations = new List<long> { 86400L, 172800L, 259200L };

	private static List<int> _bootcampLevels = new List<int> { 10, 25, 50 };

	private static List<int> _bootcampXP = new List<int> { 350, 2000, 7000 };

	private static List<int> _bootcampXPRandRange = new List<int> { 15, 100, 300 };

	public static event BootcampDefsBootcampStateChangedDelgate BootcampStateChangedEvent;

	public static long GetBootcampDuration(int bootcampIndex)
	{
		if (bootcampIndex < _bootcampDurations.Count)
		{
			return _bootcampDurations[bootcampIndex];
		}
		return -1L;
	}

	public static int GetBootcampLevel(int bootcampIndex)
	{
		if (bootcampIndex < _bootcampLevels.Count)
		{
			return _bootcampLevels[bootcampIndex];
		}
		return -1;
	}

	public static int GetXPRewardForBootcamp(int bootcampIndex, bool randomize = false)
	{
		if (bootcampIndex < _bootcampXP.Count)
		{
			if (randomize)
			{
				return _bootcampXP[bootcampIndex] + UnityEngine.Random.Range(-_bootcampXPRandRange[bootcampIndex], _bootcampXPRandRange[bootcampIndex]);
			}
			return _bootcampXP[bootcampIndex];
		}
		return -1;
	}

	public static int BootcampCount()
	{
		return App.saveData.player.bootcampMorties.Count;
	}

	public static void AddMortyToBootcamp(MortyData mortyData, int bootcampIndex)
	{
		if (bootcampIndex < App.saveData.player.bootcampMorties.Count && App.saveData.player.bootcampMorties[bootcampIndex].Count < 5)
		{
			App.saveData.player.bootcampMorties[bootcampIndex].Add(mortyData.GetGuid());
		}
	}

	public static void RemoveMortyFromBootcamp(MortyData mortyData, int bootcampIndex)
	{
		if (bootcampIndex < App.saveData.player.bootcampMorties.Count)
		{
			App.saveData.player.bootcampMorties[bootcampIndex].Remove(mortyData.GetGuid());
		}
	}

	public static void RemoveMortyFromAllBootcamps(MortyData mortyData)
	{
		for (int i = 0; i < 3; i++)
		{
			RemoveMortyFromBootcamp(mortyData, i);
		}
	}

	public static List<MortyData> GetMortiesForBootcamp(int bootcampIndex)
	{
		if (bootcampIndex >= App.saveData.player.bootcampMorties.Count)
		{
			return null;
		}
		List<MortyData> list = new List<MortyData>();
		List<MortyData> allPlayerMorties = PlayerDefs.GetAllPlayerMorties();
		foreach (string guid in App.saveData.player.bootcampMorties[bootcampIndex])
		{
			MortyData mortyData = allPlayerMorties.Find((MortyData m) => m.GetGuid().Equals(guid));
			if (mortyData != null)
			{
				list.Add(mortyData);
			}
		}
		return list;
	}

	public static List<MortyData> GetOwnedMortiesNotInBootcamp(int bootcampIndex)
	{
		if (bootcampIndex >= App.saveData.player.bootcampMorties.Count)
		{
			return null;
		}
		List<MortyData> allPlayerMorties = PlayerDefs.GetAllPlayerMorties();
		List<MortyData> mortiesForBootcamp = GetMortiesForBootcamp(bootcampIndex);
		return allPlayerMorties.Except(mortiesForBootcamp).ToList();
	}

	public static bool CanBootcampBegin(int bootcampIndex, out string failMessage)
	{
		List<MortyData> mortiesForBootcamp = GetMortiesForBootcamp(bootcampIndex);
		if (mortiesForBootcamp.Count == 0)
		{
			failMessage = TextDefs.MORTY_STORAGE_BOOTCAMP_BEGIN_FAIL_NO_MORTIES;
			return false;
		}
		if (AreAnyMortiesOnBootamp(mortiesForBootcamp))
		{
			failMessage = TextDefs.MORTY_STORAGE_BOOTCAMP_BEGIN_FAIL_ON_BOOTCAMP;
			return false;
		}
		if (PlayerDefs.AreAnyMortiesInParty(mortiesForBootcamp))
		{
			failMessage = TextDefs.MORTY_STORAGE_BOOTCAMP_BEGIN_FAIL_OUT;
			return false;
		}
		failMessage = null;
		return true;
	}

	public static bool AreAnyMortiesOnBootamp(List<MortyData> morties)
	{
		for (int i = 0; i < 3; i++)
		{
			if ((GetBootcampState(i) == eBootcampState.PROGRESS || GetBootcampState(i) == eBootcampState.RESULT_AVAILABLE) && GetMortiesForBootcamp(i).Any((MortyData x) => morties.Any((MortyData y) => y.Equals(x))))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsMortyOnBootcamp(MortyData data)
	{
		for (int i = 0; i < 3; i++)
		{
			if ((GetBootcampState(i) == eBootcampState.PROGRESS || GetBootcampState(i) == eBootcampState.RESULT_AVAILABLE) && GetMortiesForBootcamp(i).Contains(data))
			{
				return true;
			}
		}
		return false;
	}

	public static void BeginBootcamp(int bootcampIndex, double timestamp)
	{
		if (bootcampIndex < App.saveData.player.bootcampMorties.Count)
		{
			App.saveData.player.bootcampTimestamps[bootcampIndex] = timestamp;
			if (BootcampDefs.BootcampStateChangedEvent != null)
			{
				BootcampDefs.BootcampStateChangedEvent(bootcampIndex, eBootcampState.PROGRESS);
			}
		}
	}

	public static void SetBootcampIdle(int bootcampIndex)
	{
		if (bootcampIndex < App.saveData.player.bootcampMorties.Count)
		{
			App.saveData.player.bootcampTimestamps[bootcampIndex] = -1.0;
			if (BootcampDefs.BootcampStateChangedEvent != null)
			{
				BootcampDefs.BootcampStateChangedEvent(bootcampIndex, eBootcampState.IDLE);
			}
		}
	}

	public static void SetBootcampComplete(int bootcampIndex)
	{
		if (bootcampIndex < App.saveData.player.bootcampMorties.Count)
		{
			App.saveData.player.bootcampTimestamps[bootcampIndex] = 0.0;
			if (BootcampDefs.BootcampStateChangedEvent != null)
			{
				BootcampDefs.BootcampStateChangedEvent(bootcampIndex, eBootcampState.RESULT_AVAILABLE);
			}
		}
	}

	public static double GetBootcampTimeLeft(int bootcampIndex)
	{
		if (bootcampIndex >= App.saveData.player.bootcampMorties.Count)
		{
			return -1.0;
		}
		if (App.saveData.player.bootcampTimestamps[bootcampIndex] > 0.0)
		{
			return Math.Max(0.0, App.saveData.player.bootcampTimestamps[bootcampIndex] + (double)GetBootcampDuration(bootcampIndex) - TimeServer.GetLocalUnixTime());
		}
		return App.saveData.player.bootcampTimestamps[bootcampIndex];
	}

	public static eBootcampState GetBootcampState(int bootcampIndex)
	{
		double bootcampTimeLeft = GetBootcampTimeLeft(bootcampIndex);
		if (bootcampTimeLeft > 0.0)
		{
			return eBootcampState.PROGRESS;
		}
		if (bootcampTimeLeft == 0.0)
		{
			return eBootcampState.RESULT_AVAILABLE;
		}
		return eBootcampState.IDLE;
	}

	public static int GetBootcampResultsCount()
	{
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			if (GetBootcampState(i) == eBootcampState.RESULT_AVAILABLE)
			{
				num++;
			}
		}
		return num;
	}

	public static void DebugBootcamps()
	{
		for (int i = 0; i < App.saveData.player.bootcampTimestamps.Count; i++)
		{
			string empty = string.Empty;
			string text = empty;
			empty = text + "Bootcamp " + i + "\n";
			text = empty;
			empty = string.Concat(text, "state: ", GetBootcampState(i), "\n");
			text = empty;
			empty = text + "total duration: " + GetBootcampDuration(i) + "\n";
			text = empty;
			empty = text + "time left: " + GetBootcampTimeLeft(i) + "\n";
			text = empty;
			empty = text + "begun timstamp: " + App.saveData.player.bootcampTimestamps[i] + "\n";
			text = empty;
			empty = text + "now: " + TimeServer.GetLocalUnixTime() + "\n";
		}
	}
}
