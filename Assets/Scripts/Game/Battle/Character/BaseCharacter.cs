using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using Game.Battle.ScriptableObjects;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace Game.Battle.Character
{
    public abstract class BaseCharacter : MonoBehaviour, IWarrior
    {
        //These are our stats, base and upgraded
        [SerializeField] private CharacterStats baseStats;
        [SerializeField] private CharacterStatsData myStats;
        [SerializeField, ColorUsage(false, true)] private Color combatColor;

        //These are our current values
        private float _currentHealth;
        private float _currentMana;

        
        //These store our dicesets
        private readonly Dictionary<EActionType, List<EDiceType>> _diceSet = new Dictionary<EActionType, List<EDiceType>>();
        bool _isLeftSide;
        
        public abstract UniTask<IAttack> ChooseAttack();
        
        

        public TemporaryDice[] dice;

        private void Awake()
        {
            foreach (TemporaryDice d in dice)
            {
                if (!_diceSet.TryGetValue(d.type, out List<EDiceType> list))
                {
                    list  = new List<EDiceType>();
                    _diceSet.Add(d.type, list);
                }
                list.Add(d.diceType);
            }

            _currentHealth = baseStats.GetHealth(myStats);
            
            //If we're throwing right, we must be on the left side
            _isLeftSide = (Vector2.zero-(Vector2)transform.position).x > 0;
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
                EffectManager.instance.PlayDiceDeploySound();
                
                EDiceType eDiceType = die[index];
                //Create and roll the dice
                tasks[index] = DiceManager.CreateDice(eDiceType, GetTeamColor(), _isLeftSide);

                //wait 0.1 seconds before throwing the next dice
                await UniTask.Delay(200);

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

        public Color GetTeamColor()
        {
            return combatColor;
        }
    }

    [Serializable]
    public struct TemporaryDice
    {
        public EActionType type;
        public EDiceType diceType;
    }
}