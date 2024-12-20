using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using UnityEngine;

namespace Game.Battle.Character
{
    [SelectionBase]
    public class AIWarrior : BaseCharacter
    {
        [SerializeField] private EAIType aiType;
        
        public SwordAbility temp;
        
        public override async UniTask<IAbility> ChooseAttack()
        {
            await UniTask.Delay(100);
            return temp;
        }

        public override Color GetTeamColor()
        {
            return 
        }
    }
    
    public enum EAIType
    {
        Aggressive,
        Defensive,
        Balanced
    }
}
