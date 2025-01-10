using Cysharp.Threading.Tasks;
using Game.Battle.Character;
using Game.Battle.Interfaces;
using UnityEngine;

namespace Game.Battle.ScriptableObjects.AbilityObjects
{
    [CreateAssetMenu(fileName = "Fireball Ability", menuName = "Abilities/Fireball Ability")]
    public class FireballAbility : AbilityBaseStats
    {
        [SerializeField] private Transform fireballPrefab;
        public override EAbilityType AbilityType() => EAbilityType.Offensive;

        protected override async UniTask StartAbilityImplementation(IWarrior user, int diceValue, IWarrior opponent)
        {
            if (user is not BaseCharacter userCharacter || opponent is not BaseCharacter opponentCharacter)
            {
                opponent.TakeDamage(diceValue, false);
                return;
            }
        
            //Want to spawn a fireball that moves towards the other side...
            Transform myFireball = Instantiate(fireballPrefab, userCharacter.transform.position, Quaternion.identity);

            myFireball.localScale *= Mathf.Abs(diceValue / 10f);
        
            await MoveToAndExplode(myFireball, opponentCharacter.transform.position, Mathf.Max(diceValue / 3f, 1));

            EffectManager.instance.PlaySparks(myFireball.position,
                Quaternion.LookRotation(userCharacter.transform.position - opponentCharacter.transform.position, 
                    Vector3.forward), myFireball.GetComponentInChildren<ParticleSystem>().main.startColor.color);
        

            Destroy(myFireball.gameObject);
        
            opponent.TakeDamage(diceValue, false);
        }

        private async UniTask MoveToAndExplode(Transform transform, Vector3 end, float speed)
        {
            Vector3 start = transform.position;
            float timeLimit = 10;
            float time;
            do
            {
                time = Time.deltaTime;
                Vector3 direction = (end - start).normalized;
                transform.position += direction * time * speed;
                timeLimit -= time;
                await UniTask.Yield();
            } while (Vector3.Distance(transform.position, end) > speed * time && timeLimit > 0);
        }
    }
}
