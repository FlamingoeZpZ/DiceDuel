using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers.Core
{
    [Serializable]
    public enum EDiceType
    {
        Four,
        Six,
        Eight,
        Ten,
        Twenty
    }
    
    [DefaultExecutionOrder(-1000)]
    public class DataManager : MonoBehaviour
    {
        [SerializeField] private Sprite[] diceSprites;
        [SerializeField] private Sprite missingIcon;

        
        public Sprite[] DiceSprites => diceSprites;
        public Sprite MissingIcon => missingIcon;
        public static DataManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        public static readonly Dictionary<EDiceType, DiceValue> DiceValues = new Dictionary<EDiceType, DiceValue>()
        {
            { EDiceType.Four, new DiceValue(1, 4) },
            { EDiceType.Six, new DiceValue(1, 6) },
            { EDiceType.Eight, new DiceValue(1, 8) },
            { EDiceType.Ten, new DiceValue(1, 10) },
            { EDiceType.Twenty, new DiceValue(1, 20) },
        };

        public readonly struct DiceValue
        {
            public readonly int Low;
            public readonly int High;

            public DiceValue(int low, int high)
            {
                Low = low;
                High = high;
            }
        }
    }
}
