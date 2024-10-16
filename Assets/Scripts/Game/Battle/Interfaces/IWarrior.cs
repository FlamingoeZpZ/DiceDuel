using System.Collections.Generic;

namespace Game.Battle.Interfaces
{
    public interface IWarrior
    {
        public IEnumerator<IAttack> ChooseAttack();
        public bool IsDefeated();
        
        public int RollDiceFor(EActionType action);
        
    }

    public enum EActionType
    {
        Speed,
        Damage,
        Accuracy,
    }
}
