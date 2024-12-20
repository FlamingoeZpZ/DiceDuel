using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using Game.Battle.ScriptableObjects;
using Managers;
using UnityEngine;
using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace Game.Battle.Character
{
    [SelectionBase]

    public abstract class BaseCharacter : MonoBehaviour, IWarrior
    {
        [SerializeField] private CharacterStats baseStats;
        
        //These are our current values
        private float _currentHealth;
        private float _currentStamina;
        
        //These store our dicesets
        
        bool _isLeftSide;
        
        public abstract UniTask<IAbility> ChooseAttack();

        public void Init(bool isLeftSide)
        {
            _isLeftSide = isLeftSide;
        }

        public bool CanUseAbility(int staminaCost) => staminaCost < _currentStamina;
        
        

        [SerializeField] protected AbilityStats[] defaultAbilities;

        private void Awake()
        {
            _currentHealth = baseStats.MaxHealth;
            _currentStamina = baseStats.MaxStamina;
            
            //If we're throwing right, we must be on the left side
            _isLeftSide = (Vector2.zero-(Vector2)transform.position).x > 0;
        }

        public virtual bool IsDefeated()
        {
            return _currentHealth <= 0;
        }

        public virtual async UniTask<int> RollDice(IAbility ability)
        {
            //Cache variables to reduce CPU load, and make code more readable.
            EDiceType[] dice = ability.GetAttackStats().Dice;
            Color cacheColor = GetTeamColor();
            EffectManager.instance.PlayDiceDeploySound();
            
            //Create an array to store each dice roll process required
            UniTask<int>[] tasks = new UniTask<int>[dice.Length];
            
            for (int index = 0; index < dice.Length; index++)
            {
                //Create and roll the dice
                tasks[index] = DiceManager.CreateDice(dice[index], cacheColor, _isLeftSide);

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

        //We can get the color for the player from the save info,
        //We can get the color for the AI through the AiPool?
        public abstract Color GetTeamColor();
        

        
    }
}