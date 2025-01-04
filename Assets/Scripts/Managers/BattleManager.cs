using Cysharp.Threading.Tasks;
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
        private readonly IWarrior _leftWarrior;
        private readonly IWarrior _rightWarrior;
        
        public BattleManager(IWarrior leftWarrior, IWarrior rightWarrior)
        {
            _leftWarrior = leftWarrior;
            _rightWarrior = rightWarrior;
        }
        
        public async void StartBattle()
        {
            
            _leftWarrior.Init(true);
            _rightWarrior.Init(false);
            
            Debug.Log("Starting Battle");
            while (!HasFightConcluded())
            {
                //We can add another one for "Begin round"
                _leftWarrior.BeginRound();
                _rightWarrior.BeginRound();
                
                //Reset any old hud information
                
                //Both players / AI will choose attacks
                (AbilityBaseStats abilityA, AbilityBaseStats abilityB) = await  UniTask.WhenAll(_leftWarrior.ChooseAttack(),  _rightWarrior.ChooseAttack());
                
                //let's move this into another function for ease of reading.
                await ResolveAbilities(abilityA, abilityB);
                
                _leftWarrior.EndRound();
                _rightWarrior.EndRound();
            }
            Debug.Log("Concluding Battle, left lost " +_leftWarrior.IsDefeated() + " , right lost " + _rightWarrior.IsDefeated() + " !");
        }

        private async UniTask ResolveAbilities(AbilityBaseStats leftAbility, AbilityBaseStats rightAbility)
        {
            //Get their typing information
            EAbilityType atkA = leftAbility.AbilityType();
            EAbilityType atkB = rightAbility.AbilityType();
            
            //Charge the players for using the ability
            _leftWarrior.PayStamina(leftAbility.StaminaCost);
            _rightWarrior.PayStamina(rightAbility.StaminaCost);
            
            //Next, we know at this point we want to roll our dice.
            //Because rolling the dice takes a long time, and we want to physically roll the dice
            //We should respect our users time and only roll dice once per ability instead of per each action
            
            //Wait for each attack to process
            (int leftValue, int rightValue) = await UniTask.WhenAll(_leftWarrior.RollDice(leftAbility), _rightWarrior.RollDice(rightAbility));
                
            if (atkA == atkB)// If both actions are the same
            {
                //Optional rule (Who should go first), the user with the lowest value:
                if (leftValue < rightValue)
                {
                    await ProcessAttack(leftAbility, _leftWarrior, leftValue, rightAbility, _rightWarrior,rightValue);
                }
                else
                {
                    await ProcessAttack(rightAbility, _rightWarrior,rightValue, leftAbility, _leftWarrior, leftValue);
                }
            }
            else if (atkA < atkB) //Defensive is 0, Offensive is 1, Support is 2. If the left player is blocking (0), and the right player is attacking (1), let player 1 go first.
            {
                await ProcessAttack(leftAbility, _leftWarrior, leftValue, rightAbility, _rightWarrior,rightValue);
            }
            else //if the right player is blocking (0) and the left is attacking(1) let player 2 go first.
            {
                await ProcessAttack(rightAbility, _rightWarrior,rightValue, leftAbility, _leftWarrior, leftValue);
            }
        }

        private async UniTask ProcessAttack(AbilityBaseStats firstAbility, IWarrior firstWarrior, int firstRoll, AbilityBaseStats secondAbility, IWarrior secondWarrior, int secondRoll)
        {
            /*
             * How will playing an ability work?
             * We want to cue up an animation of course. But animations need to send trigger when they do things.
             * That's as easy as just placing a timeline node in the animator and a correlating receiver function in the base class.
             * But the problem is, how do we handle blocking? How do we handle special abilities?
             * Our Base character directly must need to know if they're blocking because they're directly responsible for the damage they're taking.
             * Therefore we need a way to communicate when to START and STOP doing an action because we shouldn't remain blocked.
             */
            await firstAbility.StartAbility(firstWarrior, firstRoll, secondWarrior);

            
            
            if (!HasFightConcluded())
            {
                await secondAbility.StartAbility(secondWarrior, secondRoll, firstWarrior);
            }
        }


        private bool HasFightConcluded()
        {
            return _rightWarrior.IsDefeated() || _leftWarrior.IsDefeated();
        }

    }
}
