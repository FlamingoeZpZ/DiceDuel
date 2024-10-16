using System.Collections;
using Game.Battle;
using Game.Battle.Interfaces;

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

        public IEnumerator StartBattle()
        {
            while (!HasFightConcluded())
            {
                //First we choose the attacks
                yield return _warriorA.ChooseAttack();
                yield return _warriorB.ChooseAttack();
                
                IAttack attackA = _warriorA.ChooseAttack().Current;
                IAttack attackB = _warriorB.ChooseAttack().Current;
                
                //Then we must decide which attack to execute first,
                //We must roll for speed 
                if (attackA.GetAttackStats().BaseSpeed >= attackB.GetAttackStats().BaseSpeed)
                {
                    yield return attackA.PlayAttack(_warriorA, _warriorB);
                    if (!HasFightConcluded()) yield return attackB.PlayAttack(_warriorB, _warriorA);
                }
                else
                {
                    yield return attackB.PlayAttack(_warriorB, _warriorA);
                    if (!HasFightConcluded()) yield return attackA.PlayAttack(_warriorA, _warriorB);
                }


                //We should only play the attack if we are still fighting
               
            }
        }

        private bool HasFightConcluded()
        {
            return _warriorB.IsDefeated() || _warriorA.IsDefeated();
        }

    }
}
