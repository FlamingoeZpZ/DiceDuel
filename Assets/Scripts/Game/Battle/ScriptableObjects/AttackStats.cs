using UnityEngine;

namespace Game.Battle.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AttackStats", menuName = "Scriptable Objects/AttackStats")]
    public class AttackStats : ScriptableObject
    {
        [SerializeField] private int baseSpeed;
        [SerializeField] private Sprite icon;
        [SerializeField] private EAttackType attackType;
        
        public int BaseSpeed => baseSpeed;
        public Sprite Icon => icon;
        public EAttackType AttackType => attackType;

        private static readonly Color[] AttackColors =
        {
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow
        };
        
        public Color GetColor()
        {
            return AttackColors[(int)attackType];
        }
    }

    public enum EAttackType
    {
        Offensive,
        Defensive,
        Support,
        Special,
    }
}
