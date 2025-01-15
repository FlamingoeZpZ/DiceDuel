using System;
using Game.Battle.Character;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
    public class DiceValue : MonoBehaviour
    {
        public EDiceType DiceType { get; private set; }

        private Image _img;
        private void Awake()
        {
            _img = GetComponent<Image>();
        }

        public void SetType(EDiceType diceType)
        {
            DiceType = diceType;
            _img.sprite = DataManager.Instance.DiceSprites[(int)diceType];
        }
        
        public void SetType(int diceType)
        {
            DiceType = (EDiceType)diceType;
            _img.sprite = DataManager.Instance.DiceSprites[diceType];
        }
    }
}
