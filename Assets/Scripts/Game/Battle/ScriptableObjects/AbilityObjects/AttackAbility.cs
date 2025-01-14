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
        [SerializeField] private AttackAbility attackCombo;
        [SerializeField] private AnimationClip referencedClip;

        public override EAbilityType AbilityType() => EAbilityType.Offensive;
    
        //There is no nice way of doing this. The proper way would take signifigantly longer, and could require making stuff like a decorator, and pre-loading data
        
        //Okay so we need some kind of wrapper or driver, a way to catch us on the entrance and exit so we can move back and forth correctly.
        
        protected override async UniTask StartAbilityImplementation(IWarrior user,  int diceValue, IWarrior opponent)
        {
            
            //Specifically if we're running unit tests
            if (user is not BaseCharacter characterObject || opponent is not BaseCharacter opponentCharacter){
                opponent.TakeDamage(diceValue, true);   
                return; // Pattern match
            }
            
            Animator animator = characterObject.GetComponentInChildren<Animator>();

            if (animator == null)
            {
                opponent.TakeDamage(diceValue, true);
                return;
            }
            
            Vector2 userLocation = characterObject.transform.position;
            Vector2 targetLocation = opponentCharacter.transform.position;
            Vector2 direction = (targetLocation - userLocation).normalized;
            
            await MoveTo(characterObject.transform, targetLocation - direction, 0.2f);
            
            await ComboAttack(animator, diceValue, opponent);
            
            await UniTask.Delay(500);
            
            await MoveTo(characterObject.transform, userLocation, 0.2f);

        }

        protected async UniTask ComboAttack(Animator animator,  int diceValue, IWarrior opponent)
        {
            //If we have a combo
            //Then we should do an animation
            animator.SetTrigger(animationID);

            int myTime = (int)(referencedClip.events[eventIndex].time * 1000);
            int totalTime = (int)(referencedClip.length * 1000);

            await UniTask.Delay(myTime);

            int damage = diceValue;
            if(attackCombo) damage = Mathf.Min(diceValue,comboThreshold); // Only do this if we have a combo
            
            opponent.TakeDamage(damage, true); 
            
            await UniTask.Delay(totalTime - myTime + 300);
            
            
            if (attackCombo && diceValue >= comboThreshold) 
            {
                //We should then execute that ability 
                await attackCombo.ComboAttack(animator, diceValue - comboThreshold, opponent);
            }
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
