using Cysharp.Threading.Tasks;
using Game.Battle.Attacks;
using Game.Battle.Interfaces;
using Game.Battle.ScriptableObjects;
using Managers;
using UI;
using UnityEngine;
using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace Game.Battle.Character
{
    [SelectionBase]

    public abstract class BaseCharacter : MonoBehaviour, IWarrior
    {
        private static readonly int Block = Animator.StringToHash("Block");
        private static readonly int OnHurt = Animator.StringToHash("OnHurt");
        [SerializeField] private GameHud diceHUD;
        [SerializeField] private SliderText healthBar;
        [SerializeField] private SliderText staminaBar;
        [SerializeField] private CharacterStats baseStats;
        [SerializeField] protected AbilityBaseStats[] defaultAbilities;
        
        //Accessible to our child classes
        protected AbilityBaseStats[] abilities;
        protected Weapon weapon;
        private int _currentHealth;
        private int _currentStamina;
        protected int _currentShield;
        private Vector3 _startLocation;
        
        
        //Auto integration:
        protected int CurrentHealth
        {
            get => _currentHealth;
            set
            {
                healthBar.UpdateCurrent(value);
                _currentHealth = value;
            }
        }
        
        protected int CurrentStamina
        {
            get => _currentStamina;
            set
            {
                staminaBar.UpdateCurrent(value);
                _currentStamina = value;
            }
        }


        //Needed for visual purposes.
        private Animator _characterAnimator;
        bool _isLeftSide;
        
        public abstract UniTask<AbilityBaseStats> ChooseAttack();

        public void Init(bool isLeftSide)
        {
            _isLeftSide = isLeftSide;
        }

        //We make this a separate function in case we need to bind a weapon from somewhere else at some point...
        public void BindWeapon(Weapon newWeapon)
        {
            weapon = newWeapon;
        }

        public void PayStamina(int cost)
        {
            CurrentStamina = Mathf.Clamp(CurrentStamina -cost, 0, baseStats.MaxStamina);
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
            _currentShield = Mathf.Max(_currentShield + amount, 0);
        }

        public void BeginRound()
        {
            diceHUD.Hide();
        }

        public void EndRound()
        {
            transform.position = _startLocation;
        }

        public string GetName()
        {
            return name;
        }

        private void Awake()
        {
            BindWeapon(GetComponentInChildren<Weapon>(false));
            _startLocation = transform.position;

            healthBar.UpdateMax(baseStats.MaxHealth);
            staminaBar.UpdateMax(baseStats.MaxStamina);

            CurrentHealth = baseStats.MaxHealth;
            CurrentStamina = baseStats.MaxStamina;

            //If we're throwing right, we must be on the left side
            _isLeftSide = (Vector2.zero - (Vector2)transform.position).x > 0;
            _characterAnimator = GetComponentInChildren<Animator>();

            int endOne = defaultAbilities.Length;
            int endTwo = endOne + weapon.Stats.Attacks.Length;

            abilities = new AbilityBaseStats[endTwo];

            for (int i = 0; i < endOne; i++)
            {
                abilities[i] = defaultAbilities[i];
            }

            for (int i = endOne; i < endTwo; i++)
            {
                abilities[i] = weapon.Stats.Attacks[i - endOne];
            }

            diceHUD.SetColor(GetTeamColor());
        }

        public virtual bool IsDefeated()
        {
            return CurrentHealth <= 0;
        }

        public async UniTask<int> RollDice(AbilityBaseStats ability)
        {
            int sum = ability.BaseValue;
            await diceHUD.Display(sum);
            
            //Cache variables to reduce CPU load, and make code more readable.
            EDiceType[] dice = ability.Dice;
            Color cacheColor = GetTeamColor();
            EffectManager.instance.PlayDiceDeploySound();
            
            //Create an array to store each dice roll process required
            UniTask<int>[] tasks = new UniTask<int>[dice.Length];
            
            for (int index = 0; index < dice.Length; index++)
            {
                //Create and roll the dice
                Dice createdDice = DiceManager.Instance.CreateDice(dice[index], _isLeftSide);
                
                tasks[index] = createdDice.Roll(cacheColor, createdDice.transform.forward);

                createdDice.OnDiceRolled += OnMyDiceRolled;
                
                //wait 0.2 seconds before throwing the next dice
                await UniTask.Delay(200);
            }
            
            //Wait for each dice roll to end
            int [] results = await UniTask.WhenAll(tasks);
            

            
            //Students should be confident enough to do this solo
            foreach (var result in results)
            {
                //Sum the values from the roles
                sum += result;
            }

            await diceHUD.AllDisabled();
            
            //Return the result
            return sum;
        }

        private void OnMyDiceRolled(Dice dice)
        {
            diceHUD.GeneratePopup(dice.CurrentValue, dice.transform.position);
            dice.OnDiceRolled -= OnMyDiceRolled; // Auto unsubscribe
        }
        
        //We can get the color for the player from the save info,
        //We can get the color for the AI through the AiPool?
        public abstract Color GetTeamColor();
        
    }
}