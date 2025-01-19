using Game.Battle.Character;
using Game.Battle.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Managers
{
    //Problem, how do we persist our player across scenes? 
    public class GameManager : MonoBehaviour
    {
        private BattleManager _battleManager;
        [SerializeField] private BaseCharacter _leftWarrior;
        [SerializeField] private BaseCharacter _rightWarrior;

        [SerializeField] private GameObject endGameHUD;
        [SerializeField] private TextMeshProUGUI endText;

        [ContextMenu("Test Battle")]
        public void TestBattle()
        {
            Debug.LogError("Choose random AI");
            BeginBattle(_leftWarrior, _rightWarrior);
        }

        private void Start()
        {
            TestBattle();
        }

        private async void BeginBattle(IWarrior a, IWarrior b)
        {
            endGameHUD.SetActive(false);
            _battleManager = new BattleManager(a, b);
        
            await _battleManager.StartBattle();
            OnBattleEnd();
        }

        private void OnBattleEnd()
        {
            endGameHUD.SetActive(true);
            IWarrior winner; 
            IWarrior loser;
            if (_leftWarrior.IsDefeated())
            {
                winner = _rightWarrior;
                loser = _leftWarrior;
            }
            else
            {
                winner = _leftWarrior;
                loser = _rightWarrior;

            }

            if (winner is PlayerWarrior)
            {
                endText.text = "VICTORY\n" + loser.GetName() + " has been defeated";
            }
            else if (loser is PlayerWarrior) //If we lost then we should delete the save
            {
                endText.text = "DEFEAT\nDefeated By " + winner.GetName();
            }
            else
            {
                endText.text = winner.GetName() + " has defeated " + loser.GetName();
            }

            Graphic[] graphics = GetComponentsInChildren<Graphic>();

            foreach (Graphic g in graphics)
            {
                g.InterpolateAlpha(0.5f);
            }
        }


    }
}
