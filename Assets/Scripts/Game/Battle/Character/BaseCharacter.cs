using Cysharp.Threading.Tasks;
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
        [SerializeField] private GameHud diceHUD;
        [SerializeField] private CharacterStats baseStats;
        [SerializeField] protected AbilityBaseStats[] defaultAbilities;
        
        //Accessible to our child classes
        protected AbilityBaseStats[] abilities;
        protected Weapon weapon;
        protected int _currentHealth;
        protected int _currentStamina;
        protected int _currentShield;

        //Needed for visual purposes.
        private Animator _characterAnimator;
        bool _isLeftSide;
        
        public abstract UniTask<AbilityBaseStats> ChooseAttack();

        public void Init(bool isLeftSide)
        {
            _isLeftSide = isLeftSide;
            BindWeapon(GetComponentInChildren<Weapon>());
        }

        //We make this a separate function in case we need to bind a weapon from somewhere else at some point...
        public void BindWeapon(Weapon newWeapon)
        {
            weapon = newWeapon;
        }

        public void PayStamina(int cost)
        {
            _currentStamina = Mathf.Clamp(_currentStamina +-cost, 0, baseStats.MaxStamina); 
        }

        public void TakeDamage(int amount)
        {
            
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

        private void Awake()
        {
            _currentHealth = baseStats.MaxHealth;
            _currentStamina = baseStats.MaxStamina;
            
            //If we're throwing right, we must be on the left side
            _isLeftSide = (Vector2.zero-(Vector2)transform.position).x > 0;
            _characterAnimator = GetComponent<Animator>();
            
            int endOne = defaultAbilities.Length;
            int endTwo = endOne + weapon.Stats.Attacks.Length;
            
            abilities = new AbilityBaseStats[endTwo];
           
            for (int i = 0; i < endOne; i++)
            {
                abilities[i] = defaultAbilities[i];
            }

            for (int i = endOne; i < endTwo; i++)
            {
                abilities[i] = weapon.Stats.Attacks[i];
            }
        }

        public virtual bool IsDefeated()
        {
            return _currentHealth <= 0;
        }

        public async UniTask<int> RollDice(AbilityBaseStats ability)
        {
            diceHUD.SetDisplay(ability);
            
            //Cache variables to reduce CPU load, and make code more readable.
            EDiceType[] dice = ability.Dice;
            Color cacheColor = GetTeamColor();
            EffectManager.instance.PlayDiceDeploySound();
            
            //Create an array to store each dice roll process required
            UniTask<int>[] tasks = new UniTask<int>[dice.Length];
            
            for (int index = 0; index < dice.Length; index++)
            {
                //Create and roll the dice
                tasks[index] = DiceManager.CreateDice(dice[index], cacheColor, _isLeftSide);

                //wait 0.1 seconds before throwing the next dice
                await UniTask.Delay(200);
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

            //Return the result
            return sum;
        }

        //We can get the color for the player from the save info,
        //We can get the color for the AI through the AiPool?
        public abstract Color GetTeamColor();
        
    }
}