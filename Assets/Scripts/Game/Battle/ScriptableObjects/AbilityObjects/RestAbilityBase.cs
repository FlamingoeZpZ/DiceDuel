using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using Game.Battle.ScriptableObjects;
using UnityEngine;

[CreateAssetMenu(fileName = "Rest Ability", menuName = "Abilities/Rest Ability", order = 0)]
public class RestAbilityBase : AbilityBaseStats
{
    public override EAbilityType AbilityType() => EAbilityType.Support;
    public override UniTask StartAbility(IWarrior user,  int diceValue, IWarrior opponent)
    {
        throw new System.NotImplementedException();
    }
}
