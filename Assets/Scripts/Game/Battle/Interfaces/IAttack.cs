using System.Collections;
using Game.Battle.ScriptableObjects;

namespace Game.Battle.Interfaces
{
    public interface IAttack
    {
        AttackStats GetAttackStats();
        IEnumerator PlayAttack(IWarrior user, IWarrior opponent);
    }
}
