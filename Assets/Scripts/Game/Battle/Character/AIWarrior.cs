using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using UnityEngine;

namespace Game.Battle.Character
{
    [SelectionBase]
    public class AIWarrior : BaseCharacter
    {
        public SwordAttack temp;

        public override async UniTask<IAttack> ChooseAttack()
        {
            await UniTask.Delay(100);
            return temp;
        }
    }
}
