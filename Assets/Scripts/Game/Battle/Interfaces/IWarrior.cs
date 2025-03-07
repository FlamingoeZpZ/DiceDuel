using Cysharp.Threading.Tasks;
using Game.Battle.ScriptableObjects;
using Managers.Core;

namespace Game.Battle.Interfaces
{
    public interface IWarrior
    {
        public bool IsDefeated();

        public void Init(bool isLeftSide, int startStamina, int startHealth);
        
        public void TakeDamage(int amount, bool canBeBlocked);
        public void BeginRound();
        public UniTask ChooseAttacks();
        public UniTask<AbilityData[]> RollDice();
        public string GetName();
        
        public int CurrentHealth { get; set; }
        public int CurrentStamina { get; set; }
        public int CurrentStaminaCap { get; set; }
        public int CurrentDefense { get; set; }
        
    }

    public struct AbilityData
    {
        public readonly AbilityBaseStats Ability;
        public readonly int Value;
        public readonly IWarrior MyWarrior;
        public readonly EDiceType[] DiceRolled;

        public AbilityData( IWarrior myWarrior,AbilityBaseStats ability, int value, EDiceType[] diceRolled)
        {
            Ability = ability;
            Value = value;
            DiceRolled = diceRolled;
            MyWarrior = myWarrior;
        }
    }
}
