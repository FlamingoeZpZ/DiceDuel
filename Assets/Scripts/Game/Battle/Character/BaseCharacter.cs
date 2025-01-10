using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using Game.Battle.ScriptableObjects;
using Game.Battle.UI;
using Managers;
using UI;
using UnityEngine;
using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace Game.Battle.Character
{
    [SelectionBase]

    public abstract class BaseCharacter : MonoBehaviour, IWarrior
    {
        [SerializeField] private Transform diceHudHolder;
        [SerializeField] private GameHud diceHudPrefab;
        
        private static readonly int Block = Animator.StringToHash("Block");
        private static readonly int OnHurt = Animator.StringToHash("OnHurt");
        [SerializeField] private SliderText healthBar;
        [SerializeField] private SliderText staminaBar;
        [SerializeField] private SliderText staminaCap;
        [SerializeField] protected CharacterStats baseStats;
        [SerializeField] protected AbilityBaseStats[] abilities;
        private GameHud[] _diceHuds;
        
        //Accessible to our child classes
        private int _currentHealth;
        private int _currentStamina;
        private int _currentStaminaCap;
        private int _currentShield;
        private Vector3 _startLocation;

        protected EDiceType[][] DiceSets;
        
        
        //Auto integration:
        public int CurrentHealth
        {
            get => _currentHealth;
            set
            {
                healthBar.UpdateCurrent(value);
                _currentHealth = value;
            }
        }
        
        public int CurrentStamina
        {
            get => _currentStamina;
            set
            {
                _currentStamina = Mathf.Clamp(value, 0, CurrentStaminaCap);
                staminaBar.UpdateCurrent(_currentStamina);
            }
        }
        
        public int CurrentStaminaCap
        {
            get => _currentStaminaCap;
            set
            {
                _currentStaminaCap = Mathf.Clamp(value, 0, baseStats.MaxStamina);
                staminaCap.UpdateCurrent(_currentStaminaCap);
            }
        }

        public int CurrentDefense
        {
            get => _currentShield;
            set => _currentShield = Mathf.Max(value, 0);
        }


        //Needed for visual purposes.
        private Animator _characterAnimator;
        private bool _isLeftSide;
        
        public void Init(bool isLeftSide)
        {
            _isLeftSide = isLeftSide;

        }
        
        public void TakeDamage(int amount, bool canBeBlocked)
        {
            Debug.Log("I've been hit for" + amount +". I have shields: " + _currentShield +". Is the attack blockable? " + canBeBlocked, gameObject);
            int remainder = amount;
            if (canBeBlocked)
            {
                if (_currentShield > 0)
                {
                    remainder = amount - _currentShield;
                    _currentShield = Mathf.Max(0, _currentShield - amount); //Can't drop below 0
                    EffectManager.instance.PlayBlockNoise(0.1f);
                    EffectManager.instance.PlaySparks(transform.position,
                        Quaternion.LookRotation(transform.right, Vector3.up), GetTeamColor());
                }
                if (remainder < 0)
                {
                    _characterAnimator.SetTrigger(Block);
                }
                else
                {
                    CurrentHealth -= remainder;
                    _characterAnimator.SetTrigger(OnHurt);
                }
            }
            else
            {
                CurrentHealth -= remainder;
                _characterAnimator.SetTrigger(OnHurt);
            }
        }

        public void GainShield(int amount)
        {
            //Never go below 0
            
        }

        public void BeginRound()
        {
            CurrentStamina = CurrentStaminaCap;
        }
        public virtual void EndRound()
        {
            transform.position = _startLocation;
        }

        public abstract UniTask ChooseAttacks();

        public async UniTask<AbilityData[]> RollDice()
        {
            List<AbilityData> abilityDatas  = new  List<AbilityData>();

            //Display all huds
            UniTask[] awakens = new UniTask[_diceHuds.Length];
            for (int i = 0; i < _diceHuds.Length; i++)
            {
                awakens[i] = _diceHuds[i].Display(0);
            }
            await  UniTask.WhenAll(awakens);

            //Roll the dice for each ability
            for (int i = 0; i < abilities.Length; i++)
            {
                //Cache variables to reduce CPU load, and make code more readable.
                //the dice array that we want is bound to the ability
                EDiceType[] dice = DiceSets[i];
                
                if( dice.Length == 0) continue;
                
                
                Color cacheColor = GetTeamColor();
                EffectManager.instance.PlayDiceDeploySound();

                //Create an array to store each dice roll process required
                UniTask<int>[] tasks = new UniTask<int>[dice.Length];

                
                for (int j = 0; j < dice.Length; j++)
                {
                    //Create and roll the dice
                    Dice createdDice = DiceManager.Instance.CreateDice(dice[j], _isLeftSide, abilities[i].GetColor, cacheColor);

                    tasks[j] = createdDice.Roll(createdDice.transform.forward);

                    //TODO: !!! Bind to proper ability number
                    
                    //We need to make a copy of this because I is changing.
                    int iCopy = i;
                    createdDice.OnDiceRolled = d => OnMyDiceRolled(d, iCopy);

                    //wait 0.25 seconds before throwing the next dice
                    await UniTask.Delay(250 - j * 10); // Every subsequent roll goes a little bit faster
                }
                //Wait for each dice roll to end
                int [] results = await UniTask.WhenAll(tasks);

                int sum = 0;
                
                //Students should be confident enough to do this solo
                foreach (var result in results)
                {
                    //Sum the values from the roles
                    sum += result;
                }
                
                GraphManager.Instance.RegisterRoll(dice, sum);
                
                abilityDatas.Add(new AbilityData(this, abilities[i], sum));
            }
            
            for (int i = 0; i < _diceHuds.Length; i++)
            {
                awakens[i] = _diceHuds[i].AllDisabled();
            }
            await  UniTask.WhenAll(awakens);
            
            

            return abilityDatas.ToArray();
        }
        
        public string GetName()
        {
            return name;
        }

    

        protected virtual void Awake()
        {
            _startLocation = transform.position;

            healthBar.UpdateMax(baseStats.MaxHealth);
            staminaCap.UpdateMax(baseStats.MaxStamina);
            staminaBar.UpdateMax(baseStats.MaxStamina);
            
            CurrentHealth = baseStats.MaxHealth;
            CurrentStaminaCap = 3;
            CurrentStamina = CurrentStaminaCap;

            //If we're throwing right, we must be on the left side
            _isLeftSide = (Vector2.zero - (Vector2)transform.position).x > 0;
            _characterAnimator = GetComponentInChildren<Animator>();

            _diceHuds = new GameHud[abilities.Length];
            for (int i = 0; i < abilities.Length; i++)
            {
                _diceHuds[i] = Instantiate(diceHudPrefab,diceHudHolder);
               _diceHuds[i].SetIcon(abilities[i].Icon);
            }
            
            DiceSets = new EDiceType[abilities.Length][];
        }

        public virtual bool IsDefeated()
        {
            return CurrentHealth <= 0;
        }


        private void OnMyDiceRolled(Dice dice, int hudIndex)
        {
            _diceHuds[hudIndex].GeneratePopup(dice.CurrentValue, dice.transform.position);
        }
        
        //We can get the color for the player from the save info,
        //We can get the color for the AI through the AiPool?
        public abstract Color GetTeamColor();
        
    }
}