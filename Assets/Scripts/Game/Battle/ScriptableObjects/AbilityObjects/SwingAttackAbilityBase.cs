using Cysharp.Threading.Tasks;
using Game.Battle.Character;
using Game.Battle.Interfaces;
using UnityEngine;

[CreateAssetMenu(fileName = "Swing Attack Ability", menuName = "Abilities/Swing Attack Ability", order = 10)]
public class SwingAttackAbilityBase : AbilityBaseStats
{
    [SerializeField] private string animationID = "Attack1";
    [SerializeField] private int eventIndex =  2;
    [SerializeField] private AnimationClip referencedClip;
    [SerializeField] private AbilityBaseStats attackCombo;

    protected override void CalculateMinMax()
    {
        base.CalculateMinMax();
        // Optionally force them to recompile... Requires making protected internal.
        //attackCombo.CalculateMinMax();
        minRollValue += attackCombo.MinRollValue;
        maxRollValue += attackCombo.MaxRollValue;
    }

    public override EAbilityType AbilityType() => EAbilityType.Offensive;
    
    //There is no nice way of doing this. The proper way would take signifigantly longer, and could require making stuff like a decorator, and pre-loading data
    public override async UniTask StartAbility(IWarrior user,  int diceValue, IWarrior opponent)
    {
        //Specifically if we're running unit tests
        if (user is not BaseCharacter characterObject || opponent is not BaseCharacter opponentCharacter){
            opponent.TakeDamage(diceValue);   
            return; // Pattern match
        }
        
        Animator animator = characterObject.GetComponentInChildren<Animator>();

        referencedClip.events[eventIndex].objectReferenceParameter = opponentCharacter;
        referencedClip.events[eventIndex].intParameter = diceValue;
        
        animator.SetTrigger(animationID);
        await UniTask.Delay((int)(referencedClip.length * 1000));
    }


}
