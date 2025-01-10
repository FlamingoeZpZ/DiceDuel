using Cysharp.Threading.Tasks;
using Game.Battle.ScriptableObjects;
using UnityEngine;

namespace Game.Battle.Interfaces
{
    public interface IWarrior
    {
        public bool IsDefeated();

        public void Init(bool isLeftSide);
        
        
        public void PayStamina(int cost);
        public void TakeDamage(int amount, bool canBeBlocked);
        public void GainShield(int amount);
        void BeginRound();
        public UniTask ChooseAttacks();
        public UniTask<AbilityData[]> RollDice();
        void EndRound();
        string GetName();
    }

    public struct AbilityData
    {
        public readonly AbilityBaseStats Ability;
        public readonly int Value;
        public readonly IWarrior MyWarrior;

        public AbilityData( IWarrior myWarrior,AbilityBaseStats ability, int value)
        {
            Ability = ability;
            Value = value;
            MyWarrior = myWarrior;
        }
    }
}
