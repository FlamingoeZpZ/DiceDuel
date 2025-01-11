using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using UnityEngine;

namespace Game.Battle.ScriptableObjects
{
    public abstract class AbilityBaseStats : ScriptableObject
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private bool costsMaxStamina;
        public Sprite Icon => icon;
        public Color GetColor => AttackColors[(int)AbilityType()];
        public bool CostsMaxStamina => costsMaxStamina;

        //static readonly is effectively run-time constant. Not as good as constant,
        //but still good overall
        private static readonly Color[] AttackColors =
        {
            new Color(0.19785434f,5.99215651f,1.6855806f),
            new Color(0.0358290523f,0,9.73428822f,1),
            new Color(8,0.916910887f,0,1),
        };
        
        public abstract EAbilityType AbilityType();
        protected abstract UniTask StartAbilityImplementation(IWarrior user, int diceValue, IWarrior opponent);

        public UniTask StartAbility(IWarrior user, int diceValue, IWarrior opponent)
        {
            //Drive logic that MUST ALWAYS happen... So we can do some data tracking here, or play some sounds.
            //GraphManager.Instance.RegisterRoll(this, diceValue);
            return StartAbilityImplementation(user, diceValue , opponent);
        }

    }
    
 
    

    //Offensive plays after defensive
    //Support plays after offensive
    //Rest doesn't matter
    public enum EAbilityType
    {
        Support,
        Defensive,
        Offensive,
    }
}