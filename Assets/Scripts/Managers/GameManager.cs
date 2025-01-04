using Cysharp.Threading.Tasks;
using Game.Battle.Character;
using Game.Battle.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        [SerializeField] private Button endButton;

        [ContextMenu("Test Battle")]
        public void TestBattle()
        {
            Debug.LogError("Make sure to assign left and right characters, AIpool, and player");
            BeginBattle(_leftWarrior, _rightWarrior);
        }

        private void Start()
        {
            TestBattle();
            endButton.onClick.AddListener(() => SceneManager.LoadScene(0));
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

            InterpolateAlpha(endText, 0.5f);
            InterpolateAlpha(endButton.targetGraphic, 1.2f);
        }

        private async void InterpolateAlpha(Graphic text, float time)
        {
            Color initialColor = text.color;
            float elapsedTime = 0f;

            while (elapsedTime < time)
            {
                float t = elapsedTime / time;

                Color newColor = initialColor;
                newColor.a = Mathf.Lerp(0, 1, t);
                text.color = newColor;

                elapsedTime += Time.deltaTime;

                await UniTask.Yield();
            }
            // Ensure the final alpha value is set to 0
            Color finalColor = text.color;
            finalColor.a = 1;
            text.color = finalColor;
        }
    }
}
