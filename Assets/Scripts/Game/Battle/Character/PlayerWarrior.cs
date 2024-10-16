using System.Collections.Generic;
using Game.Battle.Interfaces;
using UnityEngine;

namespace Game.Battle.Character
{
    [SelectionBase]
    public class PlayerWarrior : BaseCharacter // A player is a warrior
    {
       //What does a player have?
       
       //A player has a set of attacks
       private IAttack[] _attacks;

       public override IEnumerator<IAttack> ChooseAttack()
       {
           throw new System.NotImplementedException();
       }
    }
}
