using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Managers.Core
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct SaveData
    {
        
        public int Day;
        public  string WarriorName;
        public  List<Investment> Investments;
        public  List<EDiceType> BattleDice;
        public  int[] StoredDice;
        
        
        public SaveData(string warriorName)
        {
            WarriorName = warriorName;
            Day = 1;
            
            Investments = new List<Investment>(); //There are 5 investments
            BattleDice = new List<EDiceType>()
            {
                EDiceType.Ten,
                EDiceType.Eight,
                EDiceType.Six,
                EDiceType.Six,
                EDiceType.Six,
                EDiceType.Four,
                EDiceType.Four,
            }; // There are 12 dice in hand
            StoredDice = new int[5] // Can technically delete the part below, but leave it open for future changes
            {
                0,
                0,
                0,
                0,
                0,
            }; //There are 5 dice types
        }

        public void AddInvestment(Investment investment)
        {
            Investments.Add(investment);
        }

        //Setters
        public async UniTask IncreaseDay()
        {
            SaveData s =  SaveManager.CurrentSave;
            s.Day++;
            Debug.Log("Day has been increased, day is now: " + Day);
            for (int i = s.Investments.Count - 1; i >= 0; i--)
            {
                Debug.Log("Expected to end on: " + s.Investments[i].EndDay);
                if (  s.Investments[i].RemainingDays <= 1)
                {
                    Debug.Log("An investment has finished: " + s.Investments[i].DiceType +", " + s.Investments[i].OutputDice);
                    StoredDice[(int)s.Investments[i].DiceType] += s.Investments[i].OutputDice;
                    s.Investments.RemoveAt(i);
                }
            }

            SaveManager.CurrentSave = s;
            await SaveManager.CurrentSave.SaveGame();
        }

        public void SetBattleDice(List<EDiceType> newDice)
        {
            BattleDice = newDice;   
        }

        public void AddDiceToStorage(EDiceType type, int amount)
        {
            StoredDice[(int)type] += amount;
        }

        public void RemoveStoredDice(EDiceType type, int amount)
        {
            StoredDice[(int)type] -= amount;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Investment
    {
        public int EndDay;
        public int RemainingDays =>  EndDay-SaveManager.CurrentSave.Day;
        public int NumDice;
        public int OutputDice;
        public float InterestRate;
        public EDiceType DiceType;

        public Investment(EDiceType type, int initialBalance,  int numDays, float interestRate)
        {
            NumDice = initialBalance;
            DiceType = type;
            InterestRate  = interestRate;
            EndDay = numDays + SaveManager.CurrentSave.Day;
            OutputDice = CalculateInvestmentValue(initialBalance, numDays, interestRate);
        }
        public static int CalculateInvestmentValue(int numDice, int numDays, float interest)
        {
            return Mathf.CeilToInt(numDice * Mathf.Pow(1 + interest, numDays));
        }
    }
}
