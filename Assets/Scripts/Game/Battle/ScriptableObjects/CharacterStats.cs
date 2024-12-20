using UnityEngine;

namespace Game.Battle.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CharacterStats", menuName = "Scriptable Objects/CharacterStats")]
    public class CharacterStats : ScriptableObject
    {
        [SerializeField] private float maxStamina;
        [SerializeField] private float maxHealth;
        
        //Simple getter variables, you should never modify a scriptable object
        public float MaxStamina => maxStamina;
        public float MaxHealth => maxHealth;
        
    }
}