using System;
using UnityEngine;

namespace Managers
{
    public class DataManager : MonoBehaviour
    {
        [SerializeField] private Sprite[] diceSprites;
        
        public Sprite[] DiceSprites => diceSprites;
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
        
        
        
    }
}
