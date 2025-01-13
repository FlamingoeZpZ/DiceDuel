using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using Game.Battle.ScriptableObjects;
using Game.Battle.UI;
using TMPro;
using UI.DragAndDrop;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Battle.Character
{
    public class AbilityController : MonoBehaviour
    {

        [SerializeField] private Transform diceHolder;
        [SerializeField] private Transform abilityHolder;
        [SerializeField] private Button readyUp;

        [SerializeField] private DragAndDropObject dicePrefab;
        [SerializeField] private AbilityUI abilityUIPrefab;

        [SerializeField] private Sprite[] diceSprites;
        [SerializeField] private TextMeshProUGUI staminaCostText;
        
        private Transform[] _diceObjects;
        private AbilityUI[] _abilityObjects;

        private bool _isReady;
        private int _staminaCapCost;

        private int StaminaCapCost
        {
            get => _staminaCapCost;
            set
            {
                _staminaCapCost = value;
                if (value <= 0) staminaCostText.text = "";
                else staminaCostText.text = "Max Stamina: " + _myWarrior.CurrentStaminaCap + " => " + Mathf.Max(1,_myWarrior.CurrentStaminaCap - _staminaCapCost);
                
            }
        }
        
        
        private IWarrior _myWarrior;

        private void Awake()
        {
            readyUp.onClick.AddListener(() =>
            {
                _isReady = true;
                readyUp.gameObject.SetActive(false);
            });
            StaminaCapCost = 0;
        }

        public void ConstructAbilityController(IWarrior controller,AbilityBaseStats[] abilities, EDiceType[] dice)
        {
            _myWarrior = controller;
            
            _abilityObjects = new AbilityUI[abilities.Length];
            for (int i = 0; i < abilities.Length; i++)
            {
                AbilityUI abilityUI = Instantiate(abilityUIPrefab, abilityHolder);
                abilityUI.Bind(abilities[i]);
                _abilityObjects[i] = abilityUI;
            }
            
            _diceObjects = new Transform[dice.Length];
            for (int i = 0; i < dice.Length; i++)
            {
                DragAndDropObject spawnedDice = Instantiate(dicePrefab, diceHolder);
                spawnedDice.GetComponent<Image>().sprite = diceSprites[(int)dice[i]];
                _diceObjects[i] = spawnedDice.transform;

                //Mildly unfortunate... This is a fairly expensive function 
                EDiceType type = dice[i];
                spawnedDice.onApplyNewTarget.AddListener((previous, current) =>
                    ApplyNewTarget(previous, current, type));
            }
        }
        
        //This function is the key, it's the only function that knows the dice value, and the abilityUI...
        //What if instead of denying the input, we just disable the button to begin the fight.
        //If both of them are Ability UI, then we don't care.
        //If 
        /*
            TODO:
            * Create the AI to use dice, 3 different AI strategies? Depends on how complicated we want the AI to be.
            * Dice costs



            * When we place dice onto a section, the value of the dice should be deducted from the stamina bar.
            * If we don't have enough stamina, then we shouldn't accept the dice
            * If we don't have enough stamina, we should blink the stamina bar red to indicate



            We need a way to prevent new dice from being added when the player has spent to much stamina
            We also want to indicate to the player that they've spent too much stamina
            We also want the attack ability to take away from the stamina cap.


            1) So therefore, we know we must be making a new object specifically for the AttackAbility types 

            Problem: What if we're just moving it between abilities? The cost should be the same.
            Should we modify a Drag and Drop Zone to ask if we can be accepted? Then how can we check... Do we use a func?
            2) All the AbilityUI's must know who the player is so they can tell the dice to go back?
         */
        
        private void ApplyNewTarget(DragAndDropZone previous, DragAndDropZone current, EDiceType value)
        {
            int diceValue = (int)value + 1;
            
            if (previous.transform.parent.TryGetComponent(out AbilityUI oldUI))
            {
                oldUI.RemoveDice(value);
                //Give the stamina back
                _myWarrior.CurrentStamina += diceValue;
                
                if (oldUI.AbilityBaseStats.CostsMaxStamina)
                {
                    //We don't want to commit this just yet, we want to hold onto the value.
                    StaminaCapCost -= 1;
                }
            }

            if (current.transform.parent.TryGetComponent(out AbilityUI newUI))
            {
                newUI.AddDice(value);
                //Take the stamina away
                _myWarrior.CurrentStamina -= diceValue;

                if (newUI.AbilityBaseStats.CostsMaxStamina)
                {
                    StaminaCapCost += 1;
                }
                
            }

            readyUp.interactable = _myWarrior.CurrentStamina >= 0;
        }

        //Compiles the dice values
        public EDiceType[][] DiceValues()
        {
            _myWarrior.CurrentStaminaCap -= StaminaCapCost;
            
            
            EDiceType[][] diceValues = new EDiceType[_abilityObjects.Length][];
            for (int i = 0; i < _abilityObjects.Length; i++)
            {
                diceValues[i] = _abilityObjects[i].GetDice();
            }
            return diceValues;
        }


        public async void DisableDice()
        {
            foreach (Transform dice in _diceObjects)
            {
                dice.gameObject.SetActive(false); //Assume all dice are turned off
                await UniTask.Delay(10);
            }
        }


        public async UniTask ReturnDice()
        {
            _isReady = false;
            readyUp.gameObject.SetActive(true);
            foreach (Transform dice in _diceObjects)
            {
                dice.gameObject.SetActive(true); //Assume all dice are turned off
                dice.SetParent(diceHolder);
                await UniTask.Delay(10);
            }


            foreach (AbilityUI abilityUI in _abilityObjects)
            {
                abilityUI.ResetDice();
            }
            StaminaCapCost = 0;
        }

        public bool IsReady()
        {
            return _isReady;
        }
    }
}
