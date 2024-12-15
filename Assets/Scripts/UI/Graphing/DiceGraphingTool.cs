using System;
using System.Collections.Generic;
using Game.Battle.Attacks;
using Game.Battle.Interfaces;
using Managers;

namespace UI.Graphing
{
    public class DiceGraphingTool : GraphingTool
    {
       
        private void Awake()
        {
            Dice.OnDiceRolled += OnDiceRolled;
        }

        private void OnDestroy()
        {
            Dice.OnDiceRolled -= OnDiceRolled;
        }

        private void OnDiceRolled(EDiceType arg1, Dice arg2)
        {
            //When a dice is rolled.
            
            //Determine which side the dice belongs to.
            
            //We somehow need to know what the min and max range will be...
            //We can do this if we know what all the targetted dice will be,
            //Or if we cache all the rolls first.
            //If we know what the dice are directly, then we would have to expose ourselves and that's not ideal...
            //If we're somehow given an IWarrior, we could determine our dice from that.
            //On the other hand,
            
            
            AddValue(arg2.CurrentValue);
        }

        public void BindDice(IWarrior dice)
        {
            //dice
        }
    }
}
