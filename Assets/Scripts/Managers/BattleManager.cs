using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using Managers.Core;
using UnityEngine;

namespace Managers
{
    ///The purpose of the BattleManager is to encapsulate the battle behavior of the player and the AI.
    /// We will use dependency Injection to hide who our actors are
    ///
    /// Each Actor will have the ability to act, so something like **Attack ExecuteAttack()** In this section they will decide what attack
    /// An AI will use some pre-set logic and math while the player will press buttons
    /// We almost need to yield until both actors have made their decision
    /// 
    /// The BattleManager is responsible for executing the attacks... Whatever that may mean... What are some attacks?
    /// Fireball
    /// Block
    /// Recharge Mana
    /// Slash
    /// Heal
    /// 
    /// The BattleManager can then communicate any information it needs to from the attack to the UI manager so the character never knows about it?
    /// 
    /// The BattleManager will run this loop until the fight ends, so we need a way to tell if either of the characters **is defeated.**
    ///
    /// To recap the functions we need are ExecuteAttack and IsDefeated, Execute attack will run infinitely once two players exist until IsDefeated occurs.
    ///
    /// This creates the next question, how do we get our two warriors into this file?
    /// They'll probably be known in the GameManager, and then passed down to here.
    public class BattleManager
    {
        private readonly IWarrior _leftWarrior;
        private readonly IWarrior _rightWarrior;
        
        public BattleManager(IWarrior leftWarrior, IWarrior rightWarrior)
        {
            _leftWarrior = leftWarrior;
            _rightWarrior = rightWarrior;
        }
        
        public async UniTask StartBattle()
        {
            int dayValue = Mathf.Min(SaveManager.CurrentSave.Day, 10);
            _leftWarrior.Init(true, dayValue, dayValue * 10);
            _rightWarrior.Init(false, dayValue, dayValue * 10);
            
            Debug.Log("Starting Battle");
            while (!HasFightConcluded())
            {
                //We can add another one for "Begin round"
                _leftWarrior.BeginRound();
                _rightWarrior.BeginRound();
                
                //Both players / AI will choose attacks
                await UniTask.WhenAll(_leftWarrior.ChooseAttacks(),  _rightWarrior.ChooseAttacks());
                
                //Both players will now roll their dice.
                 (AbilityData[]leftAbilities,  AbilityData[]rightAbilities) = await UniTask.WhenAll(_leftWarrior.RollDice(), _rightWarrior.RollDice());
                
                //Now we want to sort the abilities first by their priority, then by their value (lower first)
                //Start by creating a list 
                List<AbilityData> abilities = new List<AbilityData>();
                
                abilities.AddRange(leftAbilities);
                abilities.AddRange(rightAbilities);
                
                //Let's do a bubble sort, for simplicity
                //We can assume that the first number is already sorted, so we can do -1
                for (int i = 0; i < abilities.Count - 1; i++)
                {
                    //We can assume that every number we passed (i) is already sorted,
                    //and we do -1 again so we can read the future
                    for (int j = 0; j < abilities.Count - 1 - i; j++)
                    {
                        // Compare adjacent elements
                        AbilityData currentAbility = abilities[j];
                        AbilityData nextAbility = abilities[j + 1];

                        if (currentAbility.Ability.AbilityType() > nextAbility.Ability.AbilityType() || // If the type is greater
                            (currentAbility.Ability.AbilityType() == nextAbility.Ability.AbilityType() && // Or same type but lower value
                             currentAbility.Value > nextAbility.Value))
                        {
                            // Swap adjacent elements
                            abilities[j] = nextAbility;
                            abilities[j + 1] = currentAbility;
                        }
                    }
                }

                foreach (AbilityData ability in abilities)
                {
                    if(ability.Value <= 0 || ability.MyWarrior.IsDefeated()) continue;
                    await ability.Ability.StartAbility(ability.MyWarrior, ability, ability.MyWarrior == _leftWarrior?_rightWarrior:_leftWarrior);
                }
            }
            Debug.Log("Concluding Battle, left lost " +_leftWarrior.IsDefeated() + " , right lost " + _rightWarrior.IsDefeated() + " !");
        }
        
        private bool HasFightConcluded()
        {
            return _rightWarrior.IsDefeated() || _leftWarrior.IsDefeated();
        }

    }
}
