using Managers.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Shop
{
    public class InvestmentManager : MonoBehaviour
    {

        [SerializeField] private float minRate = 0.2f;
        [SerializeField] private float maxRate = 0.8f;

        [SerializeField] private int minDays = 2;
        [SerializeField] private int maxDays = 5;
        
        private void Start()
        {
            Debug.Log("Building Investment Manager");
            InvestmentUI[] diceHolders = GetComponentsInChildren<InvestmentUI>();
            for (int index = 0; index < diceHolders.Length; index++)
            {
                if (SaveManager.CurrentSave.Investments.Count <= index)
                {
                    diceHolders[index].SetEmptyInvestment( Random.Range(minRate, maxRate), Random.Range(minDays,maxDays));
                    continue;
                }

                Investment investment = SaveManager.CurrentSave.Investments[index];
                diceHolders[index].SetCurrentInvestment(investment);
                diceHolders[index].Lock();
            }
        }

        public void CommitInvestment(InvestmentUI investment)
        {
            SaveManager.CurrentSave.AddInvestment(investment.GetInvestment());
            investment.Lock();
        }
    }
}
