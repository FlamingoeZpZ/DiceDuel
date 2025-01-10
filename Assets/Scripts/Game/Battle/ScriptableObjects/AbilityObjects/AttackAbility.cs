using Cysharp.Threading.Tasks;
using Game.Battle.Character;
using Game.Battle.Interfaces;
using UnityEngine;

namespace Game.Battle.ScriptableObjects.AbilityObjects
{
    [CreateAssetMenu(fileName = "Swing Attack Ability", menuName = "Abilities/Swing Attack Ability", order = 10)]
    public class AttackAbility : AbilityBaseStats
    {
        [SerializeField] private string animationID = "Attack1";
        [SerializeField] private int eventIndex =  1;
        [SerializeField] private int comboThreshold;
        [SerializeField] private AbilityBaseStats attackCombo;
        [SerializeField] private AnimationClip referencedClip;

        public override EAbilityType AbilityType() => EAbilityType.Offensive;
    
        //There is no nice way of doing this. The proper way would take signifigantly longer, and could require making stuff like a decorator, and pre-loading data
        protected override async UniTask StartAbilityImplementation(IWarrior user,  int diceValue, IWarrior opponent)
        {

            //If we have a combo
            if (attackCombo && diceValue > comboThreshold)
            {
                //We should then execute that ability 
                await attackCombo.StartAbility(user, diceValue - comboThreshold, opponent);
                diceValue = comboThreshold;
            }

            //Specifically if we're running unit tests
            if (user is not BaseCharacter characterObject || opponent is not BaseCharacter opponentCharacter){
                opponent.TakeDamage(diceValue, true);   
                return; // Pattern match
            }
        
            Vector3 userLocation = characterObject.transform.position;
            Vector3 targetLocation = opponentCharacter.transform.position;
        
            //Could be more optimized, but let's just do it.
            Vector3 direction = (targetLocation - userLocation).normalized;
        
            //We should only play this on the last combo
            if (!attackCombo)
            {
                await MoveTo(characterObject.transform, targetLocation - direction * 0.5f, 0.2f);
            }
        
            Animator animator = characterObject.GetComponentInChildren<Animator>();
        
        
            animator.SetTrigger(animationID);

            int myTime = (int)(referencedClip.events[eventIndex].time * 1000);
            int totalTime = (int)(referencedClip.length * 1000);

            await UniTask.Delay(myTime);
        
            opponent.TakeDamage(diceValue, true);
            
            await UniTask.Delay(totalTime - myTime - 100);
        
        }
    
    
    
        private async UniTask MoveTo(Transform transform, Vector3 endLocation, float duration = 1)
        {
            Vector3 start = transform.position;
            if (start == endLocation) return;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration; // Normalize time value
                transform.position = Vector3.Lerp(start, endLocation, t); // Interpolate position
        
                elapsedTime += Time.deltaTime; // Increment elapsed time
                await UniTask.Yield(); // Yield to the next frame
            }

            transform.position = endLocation; // Ensure the final position is exactly the target
        }
    }
}
