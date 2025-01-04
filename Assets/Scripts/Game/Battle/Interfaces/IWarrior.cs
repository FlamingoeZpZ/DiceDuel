using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Battle.Interfaces
{
    public interface IWarrior
    {
        public UniTask<AbilityBaseStats> ChooseAttack();
        public bool IsDefeated();
        
        //NOTE: We're oversharing here, truthfully we should seperate the object into smaller components, one for business one for appearance.
        public UniTask<int> RollDice(AbilityBaseStats ability);

        public Color GetTeamColor();

        public void Init(bool isLeftSide);
        
        
        public void PayStamina(int cost);
        public void TakeDamage(int amount, bool canBeBlocked);
        public void GainShield(int amount);
        void BeginRound();
        void EndRound();
    }
}
