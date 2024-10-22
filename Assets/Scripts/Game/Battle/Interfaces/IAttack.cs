using System.Collections;
using Cysharp.Threading.Tasks;
using Game.Battle.ScriptableObjects;

namespace Game.Battle.Interfaces
{
    public interface IAttack
    {
        AttackStats GetAttackStats();
        UniTask PlayAttack(IWarrior user, IWarrior opponent);
    }
}
