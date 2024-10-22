using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using Game.Battle.ScriptableObjects;using UnityEngine;

public class SwordAttack : MonoBehaviour, IAttack
{
    public AttackStats stats;
    public AttackStats GetAttackStats()
    {
        return stats;
    }

    public UniTask PlayAttack(IWarrior user, IWarrior opponent)
    {
        return UniTask.CompletedTask;
    }
}
