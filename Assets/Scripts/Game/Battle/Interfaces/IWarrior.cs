using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Battle.Interfaces
{
    public interface IWarrior
    {
        public IEnumerator<IAttack> ChooseAttack();
        public bool IsDefeated();
        
        public Task<int> RollDiceFor(EActionType action);
        
    }

    public enum EActionType
    {
        Speed,
        Damage,
        Accuracy,
    }
}
