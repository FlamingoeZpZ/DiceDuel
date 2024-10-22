using System.Collections;
using Game.Battle;
using Game.Battle.Interfaces;
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
        private readonly IWarrior _warriorA;
        private readonly IWarrior _warriorB;
        
        public BattleManager(IWarrior warriorA, IWarrior warriorB)
        {
            _warriorA = warriorA;
            _warriorB = warriorB;
        }
        
        public async void StartBattle()
        {
            Debug.Log("Starting Battle");
            while (!HasFightConcluded())
            {
                //First we choose the attacks
                Debug.Log("Player A has fight");
                IAttack attackA = await  _warriorA.ChooseAttack();
                Debug.Log("Player B has fight");
                IAttack attackB = await _warriorB.ChooseAttack();
                
                //Then we must decide which attack to execute first,
                //We must roll for speed 
                //We should ideally roll both at the same time and wait for the numbers to Accumlate instead
                Debug.Log("Player A Roll");
                int aSpeed = (await _warriorA.RollDiceFor(EActionType.Speed)) + attackA.GetAttackStats().BaseSpeed;
                Debug.Log("Player B Roll");
                int bSpeed = (await _warriorB.RollDiceFor(EActionType.Speed)) + attackB.GetAttackStats().BaseSpeed;
                
                if (aSpeed >= bSpeed)
                {
                    await attackA.PlayAttack(_warriorA, _warriorB);
                    if (!HasFightConcluded()) await attackB.PlayAttack(_warriorB, _warriorA);
                }
                else
                {
                    await attackB.PlayAttack(_warriorB, _warriorA);
                    if (!HasFightConcluded()) await attackA.PlayAttack(_warriorA, _warriorB);
                }
                
                //We should only play the attack if we are still fighting
               
            }
            Debug.Log("Concluding Battle");
        }

        private bool HasFightConcluded()
        {
            return _warriorB.IsDefeated() || _warriorA.IsDefeated();
        }

    }
}
