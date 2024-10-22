using UnityEngine;

namespace Game.Battle.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AttackStats", menuName = "Scriptable Objects/AttackStats")]
    public class AttackStats : ScriptableObject
    {
        [SerializeField] private int baseSpeed;
        
        public int BaseSpeed => baseSpeed;
    }
}
