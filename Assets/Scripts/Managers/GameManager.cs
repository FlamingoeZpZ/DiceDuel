using Game.Battle.Character;
using Game.Battle.Interfaces;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private BattleManager battleManager;

        [SerializeField] private BaseCharacter testA;
        [SerializeField] private BaseCharacter testB;

        [ContextMenu("Test Battle")]
        public void TestBattle()
        {
            BeginBattle(testA, testB);
        }

        public void BeginBattle(IWarrior a, IWarrior b)
        {
            battleManager = new BattleManager(a, b);
        
            battleManager.StartBattle();
        }

    }
}
