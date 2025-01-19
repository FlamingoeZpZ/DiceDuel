using System;
using Managers.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
    public class InvestmentUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private TextMeshProUGUI efficiencyText;
        [SerializeField] private TextMeshProUGUI daysText;
        
        [SerializeField] private InfiniteDiceHolder infiniteDiceHolder;
        [SerializeField] private Button lockButton;

        private float _rate;
        private int _days;


        private void OnEnable()
        {
            infiniteDiceHolder.OnDiceAdded += OnAdded;
            infiniteDiceHolder.OnDiceRemoved += OnRemoved;
        }

        private void OnRemoved()
        {
            if (infiniteDiceHolder.CurrentAmount == 0)
            {
                //diceImage.sprite = DataManager.Instance.MissingIcon;
            }
        }

        private void OnAdded()
        {
           // diceImage.sprite = DataManager.Instance.DiceSprites[(int)infiniteDiceHolder.DiceType];
            UpdateText();
        }

        private void UpdateText()
        {

            if (infiniteDiceHolder.CurrentAmount == 0) resultText.text = "0";
            else resultText.text = infiniteDiceHolder.CurrentAmount + " => " + Investment.CalculateInvestmentValue(infiniteDiceHolder.CurrentAmount, _days, _rate);
            Debug.Log(infiniteDiceHolder.CurrentAmount);
            daysText.text = _days + " battles"; 
            efficiencyText.text = _rate.ToString("P");
        }

        private void OnDisable()
        {
            infiniteDiceHolder.OnDiceAdded += OnAdded;
            infiniteDiceHolder.OnDiceRemoved += OnRemoved;
        }
        
        

        public void SetCurrentInvestment(Investment investment)
        {
            //diceImage.sprite = DataManager.Instance.DiceSprites[(int)investment.DiceType];
            resultText.text = investment.NumDice + " => " + investment.OutputDice;
            efficiencyText.text = investment.InterestRate.ToString("P");
            daysText.text = investment.RemainingDays + " battles";
        }

        public void Lock()
        {
            infiniteDiceHolder.enabled = false;
            enabled = false;
            lockButton.interactable = false;
        }

        public void SetEmptyInvestment(float efficiency, int days)
        {
            //diceImage.sprite = DataManager.Instance.MissingIcon;
            _rate = efficiency;
            _days = days;
            
            UpdateText();
        }

        public Investment GetInvestment()
        {
            return new Investment(infiniteDiceHolder.DiceType, infiniteDiceHolder.CurrentAmount, _days, _rate);
        }
        
    }
}

