using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Battle.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CharacterStats", menuName = "Scriptable Objects/CharacterStats")]
    public class CharacterStats : ScriptableObject
    {
        //Make the data retrievable
        [SerializeField] private CharacterStatsData stats;

        public float GetMana(CharacterStatsData characterStats)
        {
            return stats.mana + characterStats.mana; 
        }
        
        public float GetStamina(CharacterStatsData characterStats)
        {
            return stats.stamina + characterStats.stamina; 
        }
        
        public float GetHealth(CharacterStatsData characterStats)
        {
            return stats.maxHealth + characterStats.maxHealth; 
        }
        
        public float GetSpeed(CharacterStatsData characterStats)
        {
            return stats.speed + characterStats.speed; 
        }
    }

    //Make a data structure accessible outside. This way we can modify stats without affecting the base stats.
    [Serializable]
    public struct CharacterStatsData
    {
        public float mana;
        public float stamina;
        public float maxHealth;
        public float speed;
    }
}