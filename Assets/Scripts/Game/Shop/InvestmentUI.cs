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
        [SerializeField] private Image missingImage;
        
        private float _rate;
        private int _days;


        private void OnEnable()
        {
            infiniteDiceHolder.OnDiceAdded += UpdateText;
            infiniteDiceHolder.OnDiceRemoved += UpdateText;
        }
        
        private void OnDisable()
        {
            infiniteDiceHolder.OnDiceAdded += UpdateText;
            infiniteDiceHolder.OnDiceRemoved += UpdateText;
        }

        private void UpdateText()
        {

            if (infiniteDiceHolder.CurrentAmount == 0)
            {
                resultText.text = "0";
                lockButton.interactable = false;
                missingImage.gameObject.SetActive(true);
            }
            else
            {
                resultText.text = infiniteDiceHolder.CurrentAmount + " => " + Investment.CalculateInvestmentValue(infiniteDiceHolder.CurrentAmount, _days, _rate);
                lockButton.interactable = true;
                missingImage.gameObject.SetActive(false);
            }
            Debug.Log(infiniteDiceHolder.CurrentAmount);
            daysText.text = _days + " battles"; 
            efficiencyText.text = _rate.ToString("P");
        }

    
        
        

        public void SetCurrentInvestment(Investment investment)
        {
            missingImage.sprite = DataManager.Instance.DiceSprites[(int)investment.DiceType];
            resultText.text = investment.NumDice + " => " + investment.OutputDice;
            efficiencyText.text = investment.InterestRate.ToString("P");
            daysText.text = investment.RemainingDays + " battles";
            
            
        }

        public void Lock()
        {
            enabled = false;
            lockButton.interactable = false;
            missingImage.sprite = DataManager.Instance.DiceSprites[(int)infiniteDiceHolder.DiceType];
            missingImage.gameObject.SetActive(true);
            Destroy(infiniteDiceHolder.gameObject);
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

