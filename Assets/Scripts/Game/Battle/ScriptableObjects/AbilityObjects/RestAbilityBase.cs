using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using UnityEngine;

namespace Game.Battle.ScriptableObjects.AbilityObjects
{
    [CreateAssetMenu(fileName = "Rest Ability", menuName = "Abilities/Rest Ability", order = 0)]
    public class RestAbilityBase : AbilityBaseStats
    {
        [SerializeField] private ParticleSystem particle;
        public override EAbilityType AbilityType() => EAbilityType.Support;
        protected override UniTask StartAbilityImplementation(IWarrior user,  int diceValue, IWarrior opponent)
        {
            user.CurrentStaminaCap += diceValue;
            //let's only create a particle if we need to.
            if (user is MonoBehaviour characterObject)
            {
                //Create and clean up a particle at the users position
                ParticleSystem ps = Instantiate(particle, characterObject.transform.position, Quaternion.identity);
                Destroy(ps.gameObject,ps.main.duration + ps.main.startLifetime.constantMax);
                return UniTask.Delay((int)(ps.main.duration * 1000)); // Expects milliseconds.
            
            }
            //If we didn't create a particle we may be running a test, and therefore we should just move on.
            return UniTask.CompletedTask;
        }
    }
}
