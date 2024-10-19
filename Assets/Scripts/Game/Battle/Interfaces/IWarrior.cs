using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Game.Battle.Interfaces
{
    public interface IWarrior
    {
        public IEnumerator<IAttack> ChooseAttack();
        public bool IsDefeated();
        
        public UniTask<int> RollDiceFor(EActionType action);
        
    }

    public enum EActionType
    {
        Speed,
        Damage,
        Accuracy,
    }
}
