using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Battle.Interfaces
{
    public interface IWarrior
    {
        public UniTask<IAbility> ChooseAttack();
        public bool IsDefeated();
        public UniTask<int> RollDice(IAbility ability);

        public Color GetTeamColor();

        public void Init(bool isLeftSide);
    }
}
