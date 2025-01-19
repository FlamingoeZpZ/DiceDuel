using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

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
            StoredDice = new int[]
            {
                3,
                2,
                1,
                0,
                0,
            }; //There are 5 dice types
        }

        public async void AddInvestment(Investment investment)
        {

            //StoredDice[(int)investment.DiceType] -= investment.NumDice;

            //Debug.Log($"Remaining of type: {investment.DiceType}" + StoredDice[(int)investment.DiceType]);
            
            Investments.Add(investment);
            
            await SaveManager.CurrentSave.SaveGame();

        }

        public async void Merge(EDiceType oldType, EDiceType newType)
        {
            StoredDice[(int)oldType] -= 2;
            StoredDice[(int)newType] += 1;
            
            Debug.Log($"Remaining of type: {oldType}" + StoredDice[(int)oldType]);
            Debug.Log($"Remaining of type: {newType}" + StoredDice[(int)newType]);
            
            await SaveManager.CurrentSave.SaveGame();
        }

        //Setters
        public void IncreaseDay()
        {
            Day += 1;

            for (int i = Investments.Count - 1; i >= 0; i--)
            {
                if (Investments[i].Progress())
                {
                    Debug.Log("An investment has finished: ");
                    StoredDice[(int)Investments[i].DiceType] += Investments[i].OutputDice;
                }
            }
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Investment
    {
        public int RemainingDays;
        public int NumDice;
        public int OutputDice;
        public float InterestRate;
        public EDiceType DiceType;

        public Investment(EDiceType type, int initialBalance,  int numDays, float interestRate)
        {
            NumDice = initialBalance;
            DiceType = type;
            InterestRate  = interestRate;
            RemainingDays = numDays;
            OutputDice = CalculateInvestmentValue(initialBalance, numDays, interestRate);
        }

        public bool Progress()
        {
            RemainingDays -= 1;
            return RemainingDays <= 0;
        }

        public static int CalculateInvestmentValue(int numDice, int numDays, float interest)
        {
            return Mathf.CeilToInt(numDice * Mathf.Pow(1 + interest, numDays));
        }
    }
}
