using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Battle.Character
{
    [SelectionBase]
    public class AIWarrior : BaseCharacter
    {
        [SerializeField] private EAIType aiType;
        
        public override async UniTask ChooseAttacks()
        {
            await UniTask.Delay(100);

            for (int i = 0; i < DiceSets.Length; i++)
            {
                DiceSets[i] = new EDiceType[]
                {
                    EDiceType.Six,
                    EDiceType.Six
                };
            }
            Debug.LogWarning("AIWarrior ChooseAttacks needs to be completed");
        }

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
