using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using Managers;
using UnityEngine;

    public abstract class AbilityBaseStats : ScriptableObject
    {
        [SerializeField] private int staminaCost;
        [SerializeField] private int baseValue;
        [SerializeField] private EDiceType[] dice;
        [SerializeField] private Sprite icon;
        
        [SerializeField] private bool canBeNegative;
        [SerializeField] private bool canBeBlocked; //When an animation is played, we call the take damage function

        //This data will be saved (SerializeField), but it will not be visible
        [SerializeField, HideInInspector] protected int minRollValue;
        [SerializeField, HideInInspector] protected int maxRollValue;
        
        public int StaminaCost => staminaCost;
        public Sprite Icon => icon;
        public Color GetColor => AttackColors[(int)AbilityType()];
        
        public int MinRollValue => minRollValue;
        public int MaxRollValue => maxRollValue;
        
        private void OnValidate()
        {
            CalculateMinMax();
        }

        //Optional optimization quirk due to unity editor safety. Will make editor slightly slower, but prevent weird issues.
        #if UNITY_EDITOR
        private void OnEnable()
        {
            CalculateMinMax();
        }
        #endif

        protected virtual void CalculateMinMax()
        {
            minRollValue = baseValue;
            maxRollValue = baseValue;
            
            foreach (EDiceType d in dice)
            {
                DiceManager.DiceValue values = DiceManager.DiceValues[d];
                minRollValue += values.Low;
                maxRollValue += values.High;
            }
        }

        //static readonly is effectively run-time constant. Not as good as constant,
        //but still good overall
        private static readonly Color[] AttackColors =
        {
            Color.red,
            Color.blue,
            Color.green,
        };
        
        public abstract EAbilityType AbilityType();
        public abstract UniTask StartAbility(IWarrior user, int diceValue, IWarrior opponent);





    }
 
    

    //Offensive plays after defensive
    //Support plays after offensive
    //Rest doesn't matter
    public enum EAbilityType
    {
        Defensive,
        Offensive,
        Support,
    }

