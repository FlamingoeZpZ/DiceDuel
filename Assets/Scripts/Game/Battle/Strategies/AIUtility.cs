using Game.Battle.Character;
using UnityEngine;

public static class AIUtility
{
    private static int CalculateWantedValue(int totalValue, int fallOff, int push, int start, int step)
    {
        // Generate a value on a pseudo-random curve
        int random = CalculateRandomCurve(totalValue, fallOff, push);
        if (random > start) return (int)EDiceType.Twenty;
        if (random > start - step) return (int)EDiceType.Ten;
        if (random > start - step * 2) return (int)EDiceType.Eight;
        if (random > start - step * 3) return (int)EDiceType.Six;
        return (int)EDiceType.Four;
    }

    private static int CalculateRandomCurve(int totalValue, int fallOff, int push)
    {
        int random1 = Random.Range(0, totalValue) + push;
        int random2 = Random.Range(0, totalValue) + push;
        return (random1 * random2) / fallOff;
    }
    
    public static EDiceType[] GenerateDice(int numDice, int totalValue, int fallOff, int push, int start, int step)
    {
        EDiceType[] diceSet = new EDiceType[numDice];
        for(int i = 0; i < numDice; i++)
        {
            if (totalValue <= 0) break; //All remaining dice will just be 4's, because an enum is just another way to say a number.
            
            int wantedValue = CalculateWantedValue(totalValue, fallOff, push, start, step);
            int diceValue = Mathf.Min(totalValue, wantedValue);
            diceSet[i] = (EDiceType)diceValue;
            totalValue -= (diceValue + 1) * 2; //10, 8, 6, 4, 2 pts. Effectively limit the max number of D20S to 10
        }

        return diceSet;
    }
}
