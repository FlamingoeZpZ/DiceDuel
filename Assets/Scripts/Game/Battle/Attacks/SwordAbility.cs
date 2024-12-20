using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using Game.Battle.ScriptableObjects;using UnityEngine;

public class SwordAbility : MonoBehaviour, IAbility
{
    public AbilityStats stats;
    public AbilityStats GetAttackStats()
    {
        return stats;
    }

    public UniTask PlayAttack(IWarrior user, IWarrior opponent)
    {
        return UniTask.CompletedTask;
    }
}
