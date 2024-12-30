using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using Game.Battle.ScriptableObjects;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball Ability", menuName = "Abilities/Fireball Ability", order = 10)]
public class FireballAbilityBase : AbilityBaseStats
{
    public override EAbilityType AbilityType() => EAbilityType.Offensive;

    public override UniTask StartAbility(IWarrior user,  int diceValue, IWarrior opponent)
    {
        throw new System.NotImplementedException();
    }

}
