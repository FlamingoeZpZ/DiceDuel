using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using Game.Battle.ScriptableObjects;
using Managers;
using UnityEngine;
using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace Game.Battle.Character
{
    public abstract class BaseCharacter : MonoBehaviour, IWarrior
    {
        //These are our stats, base and upgraded
        [SerializeField] private CharacterStats baseStats;
        [SerializeField] private CharacterStatsData myStats;

        //These are our current values
        private float _currentHealth;
        private float _currentMana;

        private Color _combatColor;
        private Vector2 _combatDirection;
        
        //These store our dicesets
        
        private readonly Dictionary<EActionType, List<EDiceType>> _diceSet = new Dictionary<EActionType, List<EDiceType>>();
        
        
        public abstract UniTask<IAttack> ChooseAttack();

        public TemporaryDice[] dice;

        private void Awake()
        {
            foreach (TemporaryDice d in dice)
            {
                if (!_diceSet.TryGetValue(d.Type, out List<EDiceType> list))
                {
                    list  = new List<EDiceType>();
                    _diceSet.Add(d.Type, list);
                }
                list.Add(d.DiceType);
            }

            _currentHealth = baseStats.GetHealth(myStats);
        }

        public virtual bool IsDefeated()
        {
            return _currentHealth <= 0;
        }

        public virtual async UniTask<int> RollDiceFor(EActionType action)
        {
            //We use TryGetValue because it's the most optimized way to both check if we're allowed and get the list.
            if (!_diceSet.TryGetValue(action, out List<EDiceType> die))
            {
                Debug.LogError("The item does not exist");
                return 0;
            }

            //Create an array to store each dice roll process required
            UniTask<int>[] tasks = new UniTask<int>[die.Count];
            for (int index = 0; index < die.Count; index++)
            {
                EDiceType dice = die[index];
                //Create and roll the dice
                tasks[index] = DiceManager.CreateDice(dice, _combatColor, _combatDirection);
            }
            
            //Wait for each dice roll to end
            int [] results = await UniTask.WhenAll(tasks);

            int sum = 0;
            
            //Students should be confident enough to do this solo
            foreach (var result in results)
            {
                //Sum the values from the roles
                sum += result;
            }

            //Return the result
            return sum;
        }
    }

    [Serializable]
    public struct TemporaryDice
    {
        public EActionType Type;
        public EDiceType DiceType;
    }
}