using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Battle.Strategies;
using Managers.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Battle.Character
{
    public class AIWarrior : BaseCharacter
    {
        private EaiType _aiType;
        private int _staminaWanted;
        private int _totalDiceCost;
        private EDiceType[] _myDice;
        private int _lowestDiceValue;


        protected override void Awake()
        {
            base.Awake();
            _aiType = (EaiType)Random.Range(0, 3);
            
            
            int difficulty = Mathf.Min(100,(SaveManager.CurrentSave.Day) * Random.Range(1, 8) * 2);
            name = "AI difficulty: " + difficulty +", Day: " + (SaveManager.CurrentSave.Day);
            Debug.Log("Difficulty: "+ difficulty);
            _myDice = GenerateDiceSet(difficulty, _aiType);
            
            Array.Sort(_myDice);
            
            _totalDiceCost = _myDice.Length; //Faster for the computer than doing +1 12x times.
            
            foreach (EDiceType d in _myDice)
            {
                _totalDiceCost += (int)d;
                _lowestDiceValue = Mathf.Min(_lowestDiceValue, (int)d);
            }

            _lowestDiceValue += 1;
            if (_aiType is EaiType.Aggressive) _staminaWanted = (int)Mathf.Min(_totalDiceCost * 0.25f, MaxStaminaCap);
            else if (_aiType is EaiType.Defensive) _staminaWanted = (int)Mathf.Min(_totalDiceCost * 0.35f, MaxStaminaCap);
            else _staminaWanted =  (int)Mathf.Min(_totalDiceCost * 0.3f, MaxStaminaCap);
        }


        //Based on difficulty
        private EDiceType[] GenerateDiceSet(int difficulty, EaiType ai)
        {
            //Let's say there's a max difficulty of 100. What does that look like?
            //The AI should have 16D20, 4D4, 2D6, 1D8, 1D10 
            //Let's say the minimum difficulty is 10. What does that look like?
            //Let's double the AI's difficulty and use that as our value.
            
            //An aggressive AI will take higher value dice, a defensive AI will take more lower value dice.
            //How do we calculate these formulae?
            //For both AI types, start by choosing a reasonable number of dice.
          
            //When difficulty is over 80 the AI will have all 24 dice. When the difficulty is 40, the AI will have ~13.
            //We then add random, which means that at difficulty 50/70 --> 18, 50/80 --> 16, 50/90 --> 14
            //Essentially as difficulty increases there is more variety in what the AI will have
            int numDice = (int)Mathf.Lerp(3,12,Mathf.Clamp01(difficulty/Random.Range(60f,80f)));
            
            //We also want to reduce our operations given that all items are arbitrary.
            if(ai is EaiType.Aggressive) return AIUtility.GenerateDice(numDice, difficulty, 20, 10, 40, 10);
            if(ai is EaiType.Defensive) return AIUtility.GenerateDice(numDice, difficulty, 30, 15, 50, 15);
            return AIUtility.GenerateDice(numDice, difficulty, 20, 15, 70, 20);
        }
        
        
        public override async UniTask ChooseAttacks()
        {
            List<EDiceType> firstAbility = new();
            List<EDiceType> secondAbility = new();
            List<EDiceType> thirdAbility = new();
            int backIndex = _myDice.Length - 1;


            float firstStep = 0.5f;
            if (_aiType == EaiType.Balanced) firstStep -= 0.1f;
            float secondStep = firstStep + 0.3f;
            
            while (CurrentStamina >= _staminaWanted && backIndex >= 0)
            {
                //Allocate dice to either Attacking or Defending. 30% chance to put a dice in the opposite
                float chanceToDoOpposite = Random.Range(0, 1f);
                EDiceType lastDice;

                await UniTask.Delay(100);

                int cost;
                do
                {
                    lastDice = _myDice[backIndex--];
                    cost = (int)lastDice + 1;
                    if (CurrentStamina > cost) break; //Choose this item
                }
                while (backIndex >= 0) ;


                CurrentStamina -= cost;

                if (chanceToDoOpposite < firstStep) firstAbility.Add(lastDice);
                else if(chanceToDoOpposite < secondStep) secondAbility.Add(lastDice);
                else if (CurrentStaminaCap < _totalDiceCost) thirdAbility.Add(lastDice);
            }
            //Allocate the rest of the dice to gaining stamina

            for (int i = 0; i <= backIndex && CurrentStamina > 0; i++)
            {
                int cost = (int)_myDice[i] + 1;
                
                await UniTask.Delay(100);
                
                if (CurrentStamina - cost >= 0)
                {
                    CurrentStamina -= cost;
                    thirdAbility.Add(_myDice[i]);
                }
            }

        
            //Swap via deconstruction (Just a swap function really)
            if (_aiType is EaiType.Defensive)
            {
                (firstAbility, secondAbility) = (secondAbility, firstAbility);
            }
            
            
            //Before we commit to our ability, we need to make sure the attack is valid.
            int invalidDiceCount = CurrentStaminaCap - firstAbility.Count; // 12 - 12 // (all 1s)

            if (invalidDiceCount < _lowestDiceValue) // 0 < 1
            {
                //Then we must move all the dice (We can assume from left to right)
                //To a different ability.
                for (int i = _lowestDiceValue - 1; i >= 0; --i)
                {
                    thirdAbility.Add(firstAbility[i]);
                    firstAbility.RemoveAt(i);
                }
            }
            
            DiceSets = new EDiceType[][]
            {
                firstAbility.ToArray(),
                secondAbility.ToArray(),
                thirdAbility.ToArray()
            };
            

            Debug.Log("I have selected my dice.");
        }

        public override Color GetTeamColor()
        {
            return Color.white;
        }

        
        protected override void OnDefeated()
        {
            base.OnDefeated();
            SaveManager.CurrentSave.AddDiceToStorage(EDiceType.Four, Random.Range(1,3)); // 1-2 dice
            SaveManager.CurrentSave.AddDiceToStorage(EDiceType.Six, Random.Range(0,2)); // 0-1 dice
            if(SaveManager.CurrentSave.Day > 3) SaveManager.CurrentSave.AddDiceToStorage(EDiceType.Eight, Random.Range(0,2)); // 0-1 dice
            if(SaveManager.CurrentSave.Day > 7) SaveManager.CurrentSave.AddDiceToStorage(EDiceType.Ten, Random.Range(0,2)); // 0-1 dice
            if(SaveManager.CurrentSave.Day > 10) SaveManager.CurrentSave.AddDiceToStorage(EDiceType.Twenty, Random.Range(0,2)); // 0-1 dice
        }
    }
    
    public enum EaiType
    {
        Aggressive,
        Defensive,
        Balanced
    }
}
