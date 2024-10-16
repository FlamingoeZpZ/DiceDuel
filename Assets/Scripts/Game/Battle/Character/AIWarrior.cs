using System.Collections.Generic;
using Game.Battle.Interfaces;
using UnityEngine;

namespace Game.Battle.Character
{
    [SelectionBase]
    public class AIWarrior : BaseCharacter
    {
        public override IEnumerator<IAttack> ChooseAttack()
        {
            throw new System.NotImplementedException();
        }
    }
}
