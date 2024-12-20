using Game.Battle.Character;
using Game.Battle.Interfaces;
using UnityEngine;

namespace Managers
{
    //Problem, how do we persist our player across scenes? 
    public class GameManager : MonoBehaviour
    {
        private BattleManager _battleManager;
        private BaseCharacter _leftWarrior;
        private BaseCharacter _rightWarrior;

        [ContextMenu("Test Battle")]
        public void TestBattle()
        {
            Debug.LogError("Make sure to assign left and right characters, AIpool, and player");
            BeginBattle(_leftWarrior, _rightWarrior);
        }

        public void BeginBattle(IWarrior a, IWarrior b)
        {
            _battleManager = new BattleManager(a, b);
        
            _battleManager.StartBattle();
        }
    }
}
