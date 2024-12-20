using Cysharp.Threading.Tasks;
using Game.Battle.ScriptableObjects;

namespace Game.Battle.Interfaces
{
    public interface IAbility
    {
        AbilityStats GetAttackStats();
        UniTask PlayAttack(IWarrior user, IWarrior opponent);
    }
}
