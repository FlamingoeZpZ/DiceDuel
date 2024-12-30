using UnityEngine;

namespace Game.Battle.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CharacterStats", menuName = "Scriptable Objects/CharacterStats")]
    public class CharacterStats : ScriptableObject
    {
        [SerializeField] private int maxStamina;
        [SerializeField] private int maxHealth;
        
        //Simple getter variables, you should never modify a scriptable object
        public int MaxStamina => maxStamina;
        public int MaxHealth => maxHealth;
        
    }
}