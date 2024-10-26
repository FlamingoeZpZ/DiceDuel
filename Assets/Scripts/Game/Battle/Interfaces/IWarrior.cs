using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Battle.Interfaces
{
    public interface IWarrior
    {
        public UniTask<IAttack> ChooseAttack();
        public bool IsDefeated();
        public UniTask<int> RollDiceFor(EActionType action);

        public Color GetTeamColor();

    }

    public enum EActionType
    {
        Speed,
        Damage,
        Accuracy,
    }
}
