using Cysharp.Threading.Tasks;
using Game.Battle.ScriptableObjects;
using UnityEngine;

namespace Game.Battle.Character
{
    [SelectionBase]
    public class AIWarrior : BaseCharacter
    {
        [SerializeField] private EAIType aiType;
        
        public SwingAttackAbilityBase temp;
        
        public override async UniTask<AbilityBaseStats> ChooseAttack()
        {
            await UniTask.Delay(100);
            return temp;
        }

        //Will be generated directly from the AIPool in the future.
        public override Color GetTeamColor()
        {
            return Color.white;
        }
    }
    
    public enum EAIType
    {
        Aggressive,
        Defensive,
        Balanced
    }
}
