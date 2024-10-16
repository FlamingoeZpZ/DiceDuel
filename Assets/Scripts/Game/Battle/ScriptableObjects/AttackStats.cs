using UnityEngine;

namespace Game.Battle.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AttackStats", menuName = "Scriptable Objects/AttackStats")]
    public class AttackStats : ScriptableObject
    {
        [SerializeField] private float baseSpeed;
        
        public float BaseSpeed => baseSpeed;
    }
}
