using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using UI;
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
            
            _leftWarrior.Init(true);
            _rightWarrior.Init(false);
        }
        
        public async void StartBattle()
        {
            Debug.Log("Starting Battle");
            while (!HasFightConcluded())
            {
                //Reset any old hud information
                GameHud.ResetHUD();
                
                //Both players / AI will choose attacks
                IAbility abilityA = await  _leftWarrior.ChooseAttack();
                IAbility abilityB = await _rightWarrior.ChooseAttack();
                
                //Set the default display information
                GameHud.DisplayDefaults(_leftWarrior.GetTeamColor(), abilityA.GetAttackStats().BaseValue, _rightWarrior.GetTeamColor(), abilityB.GetAttackStats().BaseValue);

                //Wait for each attack to process
                (int leftValue, int rightValue) = await UniTask.WhenAll(_leftWarrior.RollDice(abilityA), _rightWarrior.RollDice(abilityB));
                leftValue += abilityA.GetAttackStats().BaseValue;
                rightValue += abilityB.GetAttackStats().BaseValue;
                
                //Wait for the numbers to be tallied
                await GameHud.SendDice();

                int atkA = (int)abilityA.GetAttackStats().AttackType;
                int atkB = (int)abilityB.GetAttackStats().AttackType;
                
                if (atkA == atkB) //if this is true, whoever got the lower roll should go first. [Optional rule]
                {
                    if (rightValue < leftValue)
                    {
                        Debug.Log("Player B moves first");
                        await abilityB.PlayAttack(_rightWarrior, _leftWarrior);
                        if (!HasFightConcluded()) await abilityA.PlayAttack(_leftWarrior, _rightWarrior);
                    }
                    else
                    {
                        Debug.Log("Player A moves first");
                        await abilityA.PlayAttack(_leftWarrior, _rightWarrior);
                        if (!HasFightConcluded()) await abilityB.PlayAttack(_rightWarrior, _leftWarrior);
                    }
                }
                else if (atkA < atkB)   //Defensive is 0, Offensive is 1, Support is 2, if this is true let player 2 go first
                {
                    Debug.Log("Player B moves first");
                    await abilityB.PlayAttack(_rightWarrior, _leftWarrior);
                    if (!HasFightConcluded()) await abilityA.PlayAttack(_leftWarrior, _rightWarrior);
                }
                else //Either order doesn't matter, or player 1 should go first.
                {
                    Debug.Log("Player A moves first");
                    await abilityA.PlayAttack(_leftWarrior, _rightWarrior);
                    if (!HasFightConcluded()) await abilityB.PlayAttack(_rightWarrior, _leftWarrior);
                }
                
                //TODO: Temporary delay to repeat the battle forever once the loop ends
                await UniTask.Delay(5000);

            }
            Debug.Log("Concluding Battle");
        }

        private bool HasFightConcluded()
        {
            return _rightWarrior.IsDefeated() || _leftWarrior.IsDefeated();
        }

    }
}
