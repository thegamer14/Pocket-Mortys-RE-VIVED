using System;
using System.Collections.Generic;
using UnityEngine;

public class InvestmentDefs : MonoBehaviour
{
	public delegate void InvestmentStateChangedDelgate(int investmentIndex, eInvestmentState state);

	public const int kEnableAtBadgeLevel = 2;

	public const int kMaxInvestments = 3;

	private static List<long> _investmentDurations = new List<long> { 10800L, 21600L, 86400L };

	private static List<int> _investmentDeposits = new List<int> { 100, 100, 100 };

	private static List<int> _investmentReturns = new List<int> { 150, 225, 700 };

	public static event InvestmentStateChangedDelgate InvestmentStateChangedEvent;

	public static long GetInvestmentDuration(int investmentIndex)
	{
		if (investmentIndex < _investmentDurations.Count)
		{
			return _investmentDurations[investmentIndex];
		}
		return -1L;
	}

	public static int GetInvestmentDeposit(int investmentIndex)
	{
		if (investmentIndex < _investmentDeposits.Count)
		{
			return _investmentDeposits[investmentIndex];
		}
		return -1;
	}

	public static int GetInvestmentReturn(int investmentIndex)
	{
		if (investmentIndex < _investmentReturns.Count)
		{
			return _investmentReturns[investmentIndex];
		}
		return -1;
	}

	public static void BeginInvestment(int investmentIndex, double timestamp)
	{
		if (investmentIndex < App.saveData.player.investmentTimestamps.Count)
		{
			PlayerDefs.RemoveCoin(GetInvestmentDeposit(investmentIndex));
			App.saveData.player.investmentTimestamps[investmentIndex] = timestamp;
			if (InvestmentDefs.InvestmentStateChangedEvent != null)
			{
				InvestmentDefs.InvestmentStateChangedEvent(investmentIndex, eInvestmentState.PROGRESS);
			}
		}
	}

	public static void SetInvestmentIdle(int investmentIndex)
	{
		if (investmentIndex < App.saveData.player.investmentTimestamps.Count)
		{
			App.saveData.player.investmentTimestamps[investmentIndex] = -1.0;
			if (InvestmentDefs.InvestmentStateChangedEvent != null)
			{
				InvestmentDefs.InvestmentStateChangedEvent(investmentIndex, eInvestmentState.IDLE);
			}
		}
	}

	public static double GetInvestmentTimeLeft(int investmentIndex)
	{
		if (investmentIndex >= App.saveData.player.investmentTimestamps.Count)
		{
			return -1.0;
		}
		if (App.saveData.player.investmentTimestamps[investmentIndex] > 0.0)
		{
			return Math.Max(0.0, App.saveData.player.investmentTimestamps[investmentIndex] + (double)GetInvestmentDuration(investmentIndex) - TimeServer.GetLocalUnixTime());
		}
		return App.saveData.player.investmentTimestamps[investmentIndex];
	}

	public static eInvestmentState GetInvestmentState(int investmentIndex)
	{
		double investmentTimeLeft = GetInvestmentTimeLeft(investmentIndex);
		if (investmentTimeLeft > 0.0)
		{
			return eInvestmentState.PROGRESS;
		}
		if (investmentTimeLeft == 0.0)
		{
			return eInvestmentState.RESULT_AVAILABLE;
		}
		return eInvestmentState.IDLE;
	}

	public static int GetInvestmentResultsCount()
	{
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			if (GetInvestmentState(i) == eInvestmentState.RESULT_AVAILABLE)
			{
				num++;
			}
		}
		return num;
	}

	public static int CollectInvestment(int investmentIndex)
	{
		if (GetInvestmentState(investmentIndex) != eInvestmentState.RESULT_AVAILABLE)
		{
			return 0;
		}
		int investmentReturn = GetInvestmentReturn(investmentIndex);
		int resultAmount;
		string resultText;
		PlayerDefs.AddCoin(investmentReturn, out resultAmount, out resultText);
		SetInvestmentIdle(investmentIndex);
		return investmentReturn;
	}
}
