using Cysharp.Threading.Tasks;
using Game.Battle.Attacks;
using Game.Battle.Interfaces;
using Managers;
using UnityEngine;

    public abstract class AbilityBaseStats : ScriptableObject
    {
        [SerializeField] private int staminaCost;
        [SerializeField] private int baseValue;
        [SerializeField] private EDiceType[] dice;
        [SerializeField] private Sprite icon;
        
        [SerializeField] protected bool canBeNegative;
        [SerializeField] protected bool canBeBlocked; //When an animation is played, we call the take damage function

        //This data will be saved (SerializeField), but it will not be visible
        //protected internal :/ because we need to edit them.
        [SerializeField, HideInInspector] private int minRollValue;
        [SerializeField, HideInInspector] private  int maxRollValue;
        
        [SerializeField, HideInInspector] protected internal int summativeMinRollValue;
        [SerializeField, HideInInspector] protected internal int summativeMaxRollValue;
        
        public int StaminaCost => staminaCost;
        public int BaseValue => baseValue;
        public  int SummativeMinRollValue => summativeMinRollValue;
        public int SummativeMaxRollValue => summativeMaxRollValue;
        public Sprite Icon => icon;
        public EDiceType[] Dice => dice;
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
            minRollValue = 0;
            maxRollValue = 0;
            
            
            foreach (EDiceType d in dice)
            {
                DiceManager.DiceValue values = DiceManager.DiceValues[d];
                minRollValue += values.Low;
                maxRollValue += values.High;
            }

            summativeMinRollValue = minRollValue + baseValue;
            summativeMaxRollValue = maxRollValue + baseValue;
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
        protected abstract UniTask StartAbilityImplementation(IWarrior user, int diceValue, IWarrior opponent);

        public UniTask StartAbility(IWarrior user, int diceValue, IWarrior opponent)
        {
            Debug.Log("Dice value: " + diceValue + ", Adding BaseValue " + (diceValue + BaseValue));
            //Drive logic that MUST ALWAYS happen... So we can do some data tracking here, or play some sounds.
            GraphManager.Instance.RegisterRoll(this, diceValue);
            return StartAbilityImplementation(user, diceValue , opponent);
        }

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

