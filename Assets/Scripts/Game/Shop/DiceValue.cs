using Game.Battle.Character;
using UnityEngine;

namespace Game.Shop
{
    public class DiceValue : MonoBehaviour
    {
        public EDiceType DiceType { get; private set; }

        public void SetType(EDiceType diceType)
        {
            DiceType = diceType;
        }
    }
}
