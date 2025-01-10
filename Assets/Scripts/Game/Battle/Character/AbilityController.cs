using Cysharp.Threading.Tasks;
using Game.Battle.ScriptableObjects;
using Game.Battle.UI;
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

        private Transform[] _diceObjects;
        private AbilityUI[] _abilityObjects;

        private bool _isReady;

        private void Awake()
        {
            readyUp.onClick.AddListener(() =>
            {
                _isReady = true;
                readyUp.interactable = false;
            });
        }

        public void BuildDiceSet(EDiceType[] dice)
        {
            _diceObjects = new Transform[dice.Length];
            for (int i = 0; i < dice.Length; i++)
            {
                DragAndDropObject spawnedDice = Instantiate(dicePrefab, diceHolder);
                spawnedDice.GetComponent<Image>().sprite = diceSprites[(int)dice[i]];
                _diceObjects[i] = spawnedDice.transform;

                //Mildly unfortunate... This is a fairly expensive function 
                int iCopy = i;
                spawnedDice.onApplyNewTarget.AddListener((previous, current) =>
                    ApplyNewTarget(previous, current, dice[iCopy]));

            }
        }

        private void ApplyNewTarget(DragAndDropZone previous, DragAndDropZone current, EDiceType value)
        {
            //A bit hard-coded but oh well..
            if (previous.transform.parent.TryGetComponent(out AbilityUI oldUI)) oldUI.RemoveDice(value);
            if (current.transform.parent.TryGetComponent(out AbilityUI newUI)) newUI.AddDice(value);
        }

        //Compiles the dice values
        public EDiceType[][] DiceValues()
        {
            EDiceType[][] diceValues = new EDiceType[_abilityObjects.Length][];
            for (int i = 0; i < _abilityObjects.Length; i++)
            {
                diceValues[i] = _abilityObjects[i].GetDice();
            }
            return diceValues;
        }


    public void SetAbilities(AbilityBaseStats[] abilities)
        {
            _abilityObjects = new AbilityUI[abilities.Length];
            for (int i = 0; i < abilities.Length; i++)
            {
                AbilityUI abilityUI = Instantiate(abilityUIPrefab, abilityHolder);
                abilityUI.Bind(abilities[i]);
                _abilityObjects[i] = abilityUI;
            }
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
            readyUp.interactable = true;

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
            
        }

        public bool IsReady()
        {
            return _isReady;
        }
    }
}
